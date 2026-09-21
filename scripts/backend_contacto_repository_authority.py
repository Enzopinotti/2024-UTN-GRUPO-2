from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"

controller = (BACKEND / "Controllers" / "ContactoController.cs").read_text(encoding="utf-8-sig")
service = (BACKEND / "Services" / "ContactoService.cs").read_text(encoding="utf-8-sig")
repository = (BACKEND / "Repositories" / "ContactoRepository.cs").read_text(encoding="utf-8-sig")
interface = (BACKEND / "Repositories" / "IContactoRepository.cs").read_text(encoding="utf-8-sig")
iunit = (BACKEND / "Repositories" / "IUnitOfWork.cs").read_text(encoding="utf-8-sig")
unit = (BACKEND / "Repositories" / "UnitOfWork.cs").read_text(encoding="utf-8-sig")
program = (BACKEND / "Program.cs").read_text(encoding="utf-8-sig")

failures: list[str] = []

if "AppDbContext" in controller or "_context" in controller:
    failures.append("ContactoController regained direct AppDbContext access")
if "IContactoService contactoService" not in controller:
    failures.append("ContactoController service constructor dependency missing")
for contract in (
    "_contactoService.CreateAsync(contacto)",
    "_contactoService.GetAllAsync()",
    "_contactoService.GetByIdAsync(id)",
):
    if contract not in controller:
        failures.append(f"ContactoController service delegation missing: {contract}")

for contract in (
    '[Authorize(Roles = "Admin")]',
    "[AllowAnonymous]",
    "[HttpPost]",
    "[HttpGet]",
    '[HttpGet("{id}")]',
    "CreatedAtAction(nameof(GetContacto)",
):
    if contract not in controller:
        failures.append(f"ContactoController HTTP/auth contract missing: {contract}")

if "AppDbContext" in service or "_context" in service:
    failures.append("ContactoService regained direct AppDbContext access")
if "contacto.Fecha = DateTime.Now;" not in service:
    failures.append("ContactoService historical Fecha assignment missing")
for contract in (
    "_unitOfWork.Contactos.AddAsync(contacto)",
    "_unitOfWork.Contactos.GetAllAsync()",
    "_unitOfWork.Contactos.GetByIdAsync(id)",
):
    if contract not in service:
        failures.append(f"ContactoService repository delegation missing: {contract}")

for contract in (
    "Task<Contacto> AddAsync(Contacto contacto);",
    "Task<List<Contacto>> GetAllAsync();",
    "Task<Contacto?> GetByIdAsync(int id);",
):
    if contract not in interface:
        failures.append(f"IContactoRepository contract missing: {contract}")

if repository.count("_context.SaveChangesAsync()") != 1:
    failures.append("ContactoRepository must own exactly one mutation save site")
if repository.count("_context.Contactos") != 3:
    failures.append("ContactoRepository must own exactly three Contactos data-access paths")
if "IContactoRepository Contactos { get; }" not in iunit:
    failures.append("IUnitOfWork Contactos surface missing")
if "public IContactoRepository Contactos => _contactoRepository ??= new ContactoRepository(_context);" not in unit:
    failures.append("UnitOfWork ContactoRepository lazy ownership missing")
if "AddScoped<IContactoRepository" in program:
    failures.append("ContactoRepository should remain UnitOfWork-owned, not directly registered")
if "builder.Services.AddScoped<IContactoService, ContactoService>();" not in program:
    failures.append("ContactoService DI registration missing")

print("contactocontroller-direct-context=absent")
print("contactocontroller-http-contract=preserved")
print("contactoservice-repository-authority=IUnitOfWork.Contactos")
print("contactorepository-save-site-count=1")
print("contactorepository-owner=UnitOfWork")
print("contacto-fecha-authority=service-DateTime.Now")

if failures:
    print("Backend Contacto repository authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-contacto-repository-authority=clean")
