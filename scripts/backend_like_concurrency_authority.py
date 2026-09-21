from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"
TESTS = ROOT / "Backend" / "antigal.server.Tests"
MIGRATION_ID = "20260921141500_LikeConcurrencyIntegrity"

context = (BACKEND / "Data" / "AppDbContext.cs").read_text(encoding="utf-8-sig")
repository = (BACKEND / "Repositories" / "LikeRepository.cs").read_text(encoding="utf-8-sig")
snapshot = (BACKEND / "Migrations" / "AppDbContextModelSnapshot.cs").read_text(encoding="utf-8-sig")
migration = (BACKEND / "Migrations" / f"{MIGRATION_ID}.cs").read_text(encoding="utf-8-sig")
designer = (BACKEND / "Migrations" / f"{MIGRATION_ID}.Designer.cs").read_text(encoding="utf-8-sig")
tests_project = (TESTS / "antigal.server.Tests.csproj").read_text(encoding="utf-8-sig")
tests = (TESTS / "LikeRepositoryConcurrencyTests.cs").read_text(encoding="utf-8-sig")

failures: list[str] = []

for source_name, source in (
    ("AppDbContext", context),
    ("model snapshot", snapshot),
):
    for contract in (
        'HasIndex(l => new { l.UserId, l.ProductoId })' if source_name == "AppDbContext" else 'HasIndex("UserId", "ProductoId")',
        ".IsUnique()",
        '.HasFilter("[UserId] IS NOT NULL")',
    ):
        if contract not in source:
            failures.append(f"{source_name} Like uniqueness contract missing: {contract}")

dedupe_contracts = (
    "ROW_NUMBER() OVER",
    "PARTITION BY [UserId], [ProductoId]",
    "ORDER BY [Id]",
    "WHERE [UserId] IS NOT NULL",
    "DELETE FROM RankedLikes",
    "WHERE [RowNumber] > 1",
)
for contract in dedupe_contracts:
    if contract not in migration:
        failures.append(f"Likes deduplication migration contract missing: {contract}")

dedupe_position = migration.find("DELETE FROM RankedLikes")
index_position = migration.find('name: "IX_Likes_UserId_ProductoId"')
if dedupe_position < 0 or index_position < 0 or dedupe_position > index_position:
    failures.append("Likes migration must deduplicate before creating the unique index")

for contract in (
    'columns: new[] { "UserId", "ProductoId" }',
    "unique: true",
    'filter: "[UserId] IS NOT NULL"',
    'name: "IX_Likes_UserId_ProductoId"',
):
    if contract not in migration:
        failures.append(f"Likes unique-index migration contract missing: {contract}")

if migration.count('name: "IX_Likes_UserId_ProductoId"') != 2:
    failures.append("Likes migration must create and drop exactly the maintained unique index")

for contract in (
    f'[Migration("{MIGRATION_ID}")]',
    'HasIndex("UserId", "ProductoId")',
    ".IsUnique()",
    '.HasFilter("[UserId] IS NOT NULL")',
):
    if contract not in designer:
        failures.append(f"Likes migration designer contract missing: {contract}")

for contract in (
    "_context.Likes.Add(like);",
    "catch (DbUpdateException)",
    "_context.Entry(like).State = EntityState.Detached;",
    ".AsNoTracking()",
    ".AnyAsync(l => l.UserId == userId && l.ProductoId == productoId)",
    "if (duplicateExists)",
    "return false;",
    "throw;",
):
    if contract not in repository:
        failures.append(f"LikeRepository concurrency conflict contract missing: {contract}")

add_start = repository.find("public async Task<bool> AddLikeAsync")
remove_start = repository.find("public async Task<bool> RemoveLikeAsync")
if add_start < 0 or remove_start < 0:
    failures.append("LikeRepository method boundaries missing")
else:
    add_method = repository[add_start:remove_start]
    if "FirstOrDefaultAsync" in add_method:
        failures.append("AddLikeAsync regained check-then-insert race")
    if add_method.count("_context.SaveChangesAsync()") != 1:
        failures.append("AddLikeAsync must have exactly one insert save attempt")

for contract in (
    '<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.12" />',
):
    if contract not in tests_project:
        failures.append(f"relational Like test dependency missing: {contract}")

for contract in (
    "Model_HasUniqueFilteredUserProductIndex",
    "AddLikeAsync_ExistingPair_ReturnsFalseAfterUniqueConflict",
    "AddLikeAsync_UnrelatedDatabaseFailure_IsRethrown",
    "UniqueFilter_PreservesHistoricalNullUserRows",
    "Foreign Keys=True",
):
    if contract not in tests:
        failures.append(f"Like concurrency relational test contract missing: {contract}")

print("like-unique-key=UserId,ProductoId")
print("like-unique-filter=UserId-IS-NOT-NULL")
print("like-migration-deduplication=oldest-Id-kept")
print("like-migration-deduplication-order=before-unique-index")
print("like-add-authority=insert-then-classify-db-conflict")
print("like-duplicate-conflict-result=false")
print("like-unrelated-db-failure=rethrow")
print("like-null-user-historical-rows=unconstrained")

if failures:
    print("Backend Like concurrency authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-like-concurrency-authority=clean")
