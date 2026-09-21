from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"

service = (BACKEND / "Services" / "LikeService.cs").read_text(encoding="utf-8-sig")
repository = (BACKEND / "Repositories" / "LikeRepository.cs").read_text(encoding="utf-8-sig")
interface = (BACKEND / "Repositories" / "ILikeRepository.cs").read_text(encoding="utf-8-sig")
iunit = (BACKEND / "Repositories" / "IUnitOfWork.cs").read_text(encoding="utf-8-sig")
unit = (BACKEND / "Repositories" / "UnitOfWork.cs").read_text(encoding="utf-8-sig")
program = (BACKEND / "Program.cs").read_text(encoding="utf-8-sig")
snapshot = (BACKEND / "Migrations" / "AppDbContextModelSnapshot.cs").read_text(encoding="utf-8-sig")

failures: list[str] = []

if "AppDbContext" in service or "_context" in service:
    failures.append("LikeService regained direct AppDbContext access")
if "AddScoped<ILikeRepository" in program:
    failures.append("LikeRepository should remain UnitOfWork-owned, not directly registered")

for contract in (
    "_unitOfWork.Likes.AddLikeAsync(userId, productoId)",
    "_unitOfWork.Likes.RemoveLikeAsync(userId, productoId)",
    "_unitOfWork.Likes.GetUserLikesAsync(userId)",
):
    if contract not in service:
        failures.append(f"LikeService repository delegation missing: {contract}")

for contract in (
    "Task<bool> AddLikeAsync(string userId, int productoId);",
    "Task<bool> RemoveLikeAsync(string userId, int productoId);",
    "Task<List<Producto>> GetUserLikesAsync(string userId);",
):
    if contract not in interface:
        failures.append(f"ILikeRepository contract missing: {contract}")

if repository.count("_context.SaveChangesAsync()") != 2:
    failures.append("LikeRepository must own exactly two mutation save sites")
if repository.count("_context.Likes") < 3:
    failures.append("LikeRepository Like data-access paths missing")
if "_context.Productos" not in repository:
    failures.append("LikeRepository product read path missing")
if "ILikeRepository Likes { get; }" not in iunit:
    failures.append("IUnitOfWork Likes surface missing")
if "public ILikeRepository Likes => _likeRepository ??= new LikeRepository(_context);" not in unit:
    failures.append("UnitOfWork LikeRepository lazy ownership missing")

like_block = re.search(
    r'modelBuilder\.Entity\("antigal\.server\.Models\.Like", b =>\s*\{.*?\n\s*\}\);',
    snapshot,
    flags=re.S,
)
compound_unique = False
if like_block:
    block = like_block.group(0)
    compound_unique = 'HasIndex("UserId", "ProductoId")' in block and ".IsUnique()" in block

print("likeservice-direct-context=absent")
print("likeservice-repository-authority=IUnitOfWork.Likes")
print("likerepository-save-site-count=2")
print("likerepository-owner=UnitOfWork")
print(f"like-user-product-unique-index={compound_unique}")
print("like-concurrency-unique-constraint-status=not-addressed-in-b32")

if failures:
    print("Backend Like repository authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-like-repository-authority=clean")
