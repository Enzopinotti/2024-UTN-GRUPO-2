from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"

files = [
    p for p in sorted(BACKEND.rglob("*.cs"))
    if not any(part in {"bin", "obj"} for part in p.parts)
]

failures: list[str] = []

outside_context_consumers: set[str] = set()
outside_save_distribution: dict[str, int] = {}

for path in files:
    source = path.read_text(encoding="utf-8-sig")
    relative = path.relative_to(ROOT).as_posix()
    is_repository = "/Repositories/" in relative

    if not is_repository:
        constructor_with_context = re.search(
            r"public\s+\w+\([^)]*AppDbContext\s+context[^)]*\)",
            source,
            flags=re.S,
        )
        if constructor_with_context:
            outside_context_consumers.add(relative)

        count = source.count("_context.SaveChangesAsync()")
        if count:
            outside_save_distribution[relative] = count

expected_consumers = {
    "Backend/antigal.server/Controllers/ContactoController.cs",
    "Backend/antigal.server/Services/ImageService.cs",
}
expected_saves = {
    "Backend/antigal.server/Controllers/ContactoController.cs": 1,
    "Backend/antigal.server/Services/ImageService.cs": 3,
}

if outside_context_consumers != expected_consumers:
    failures.append(
        "outside-repository AppDbContext consumer allowlist changed: "
        + str(sorted(outside_context_consumers))
    )
if outside_save_distribution != expected_saves:
    failures.append(
        "outside-repository SaveChanges distribution changed: "
        + str(outside_save_distribution)
    )

image_path = BACKEND / "Services" / "ImageService.cs"
image = image_path.read_text(encoding="utf-8-sig")

methods = {
    "upload": r"public async Task<Imagen> UploadImageAsync\(.*?\n        \}",
    "delete-id": r"public async Task<bool> DeleteImageAsync\(.*?\n        \}",
    "delete-url": r"public async Task<bool> DeleteImageByUrlAsync\(.*?\n        \}",
}
for name, pattern in methods.items():
    match = re.search(pattern, image, flags=re.S)
    if not match:
        failures.append(f"ImageService method missing: {name}")
        continue
    count = match.group(0).count("_context.SaveChangesAsync()")
    if count != 1:
        failures.append(
            f"ImageService {name} persistence must have exactly one save site, got {count}"
        )

upload = re.search(methods["upload"], image, flags=re.S)
if upload:
    upload_source = upload.group(0)
    required = (
        "_context.Imagenes.Add(nuevaImagen);",
        "producto.ImagenUrls.Add(nuevaImagen.Url);",
        "usuario.ImagenUrl = nuevaImagen.Url;",
        "categoria.ImagenUrl = nuevaImagen.Url;",
        "await _context.SaveChangesAsync();",
    )
    for contract in required:
        if contract not in upload_source:
            failures.append(f"ImageService upload contract missing: {contract}")

    save_index = upload_source.find("await _context.SaveChangesAsync();")
    for mutation in required[:-1]:
        if upload_source.find(mutation) > save_index:
            failures.append(
                "ImageService upload related mutation occurs after persistence flush: "
                + mutation
            )

outside_save_count = sum(outside_save_distribution.values())

print(f"outside-repository-context-consumer-count={len(outside_context_consumers)}")
print("outside-repository-context-consumers=" + ",".join(sorted(outside_context_consumers)))
print(f"outside-repository-save-site-count={outside_save_count}")
print("outside-repository-save-distribution=" + ",".join(
    f"{path}:{count}" for path, count in sorted(outside_save_distribution.items())
))
print("image-service-upload-save-site-count=1")
print("image-service-delete-id-save-site-count=1")
print("image-service-delete-url-save-site-count=1")
print("image-service-db-flush-authority=one-per-mutating-operation")

if failures:
    print("Backend direct-context persistence authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-direct-context-persistence-authority=clean")
