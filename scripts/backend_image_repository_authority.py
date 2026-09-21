from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"

service = (BACKEND / "Services" / "ImageService.cs").read_text(encoding="utf-8-sig")
repository = (BACKEND / "Repositories" / "ImageRepository.cs").read_text(encoding="utf-8-sig")
interface = (BACKEND / "Repositories" / "IImageRepository.cs").read_text(encoding="utf-8-sig")
iunit = (BACKEND / "Repositories" / "IUnitOfWork.cs").read_text(encoding="utf-8-sig")
unit = (BACKEND / "Repositories" / "UnitOfWork.cs").read_text(encoding="utf-8-sig")
program = (BACKEND / "Program.cs").read_text(encoding="utf-8-sig")

failures: list[str] = []

if "AppDbContext" in service or "_context" in service:
    failures.append("ImageService regained direct AppDbContext access")
if "IUnitOfWork _unitOfWork" not in service:
    failures.append("ImageService UnitOfWork state missing")
if "ImageService(Cloudinary cloudinary, IUnitOfWork unitOfWork)" not in service:
    failures.append("ImageService constructor authority changed")

for contract in (
    "_unitOfWork.Images.AddAsync(nuevaImagen)",
    "_unitOfWork.Images.GetByIdAsync(imageId)",
    "_unitOfWork.Images.GetByUrlAsync(imageUrl)",
    "_unitOfWork.Images.DeleteAsync(image)",
):
    if contract not in service:
        failures.append(f"ImageService repository delegation missing: {contract}")

upload_start = service.find("public async Task<Imagen> UploadImageAsync")
delete_id_start = service.find("public async Task<bool> DeleteImageAsync")
delete_url_start = service.find("public async Task<bool> DeleteImageByUrlAsync")

if min(upload_start, delete_id_start, delete_url_start) < 0:
    failures.append("ImageService public operation boundaries missing")
else:
    upload = service[upload_start:delete_id_start]
    delete_id = service[delete_id_start:delete_url_start]
    delete_url = service[delete_url_start:]

    upload_cloud = upload.find("_cloudinary.UploadAsync")
    upload_db = upload.find("_unitOfWork.Images.AddAsync")
    if min(upload_cloud, upload_db) < 0 or upload_cloud > upload_db:
        failures.append("Upload must preserve Cloudinary-before-database ordering")

    id_read = delete_id.find("_unitOfWork.Images.GetByIdAsync")
    id_cloud = delete_id.find("_cloudinary.DestroyAsync")
    id_delete = delete_id.find("_unitOfWork.Images.DeleteAsync")
    if min(id_read, id_cloud, id_delete) < 0 or not (id_read < id_cloud < id_delete):
        failures.append("Delete-by-id external/database ordering changed")

    url_cloud = delete_url.find("_cloudinary.DestroyAsync")
    url_read = delete_url.find("_unitOfWork.Images.GetByUrlAsync")
    url_delete = delete_url.find("_unitOfWork.Images.DeleteAsync")
    if min(url_cloud, url_read, url_delete) < 0 or not (url_cloud < url_read < url_delete):
        failures.append("Delete-by-url external/database ordering changed")

for contract in (
    "Task<Imagen> AddAsync(Imagen image);",
    "Task<Imagen?> GetByIdAsync(int imageId);",
    "Task<Imagen?> GetByUrlAsync(string imageUrl);",
    "Task DeleteAsync(Imagen image);",
):
    if contract not in interface:
        failures.append(f"IImageRepository contract missing: {contract}")

if repository.count("_context.SaveChangesAsync()") != 2:
    failures.append("ImageRepository must own exactly two mutation save sites")

for contract in (
    "_context.Imagenes.Add(image);",
    "_context.Imagenes.FindAsync(imageId)",
    "_context.Imagenes.FirstOrDefaultAsync(image => image.Url == imageUrl)",
    "_context.Imagenes.Remove(image);",
    "_context.Productos.FindAsync(image.ProductoId.Value)",
    "_context.Users.FindAsync(image.UsuarioId)",
    "_context.Categorias.FindAsync(image.CategoriaId.Value)",
):
    if contract not in repository:
        failures.append(f"ImageRepository persistence/association contract missing: {contract}")

if "IImageRepository Images { get; }" not in iunit:
    failures.append("IUnitOfWork Images surface missing")
if "public IImageRepository Images => _imageRepository ??= new ImageRepository(_context);" not in unit:
    failures.append("UnitOfWork ImageRepository lazy ownership missing")
if "AddScoped<IImageRepository" in program:
    failures.append("ImageRepository should remain UnitOfWork-owned, not directly registered")

print("imageservice-direct-context=absent")
print("imageservice-cloudinary-authority=service")
print("imagerepository-persistence-authority=IUnitOfWork.Images")
print("imagerepository-save-site-count=2")
print("image-association-authority=Producto,User,Categoria")
print("image-upload-order=Cloudinary-then-database")
print("image-delete-id-order=database-read-then-Cloudinary-then-database-delete")
print("image-delete-url-order=Cloudinary-then-database-read-delete")

if failures:
    print("Backend image repository authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-image-repository-authority=clean")
