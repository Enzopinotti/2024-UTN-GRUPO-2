from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"
TESTS = ROOT / "Backend" / "antigal.server.Tests"

interface = (BACKEND / "Repositories" / "IUnitOfWork.cs").read_text(encoding="utf-8-sig")
implementation = (BACKEND / "Repositories" / "UnitOfWork.cs").read_text(encoding="utf-8-sig")

production_files = [
    p for p in sorted(BACKEND.rglob("*.cs"))
    if not any(part in {"bin", "obj"} for part in p.parts)
]
test_files = [
    p for p in sorted(TESTS.rglob("*.cs"))
    if not any(part in {"bin", "obj"} for part in p.parts)
]

failures: list[str] = []

if "SaveChangesAsync" in interface:
    failures.append("IUnitOfWork regained SaveChangesAsync")
if "public async Task<int> SaveChangesAsync()" in implementation:
    failures.append("UnitOfWork regained SaveChangesAsync implementation")

production_unitofwork_save_calls: list[str] = []
direct_context_save_count = 0
for path in production_files:
    source = path.read_text(encoding="utf-8-sig")
    relative = path.relative_to(ROOT).as_posix()
    if "_unitOfWork.SaveChangesAsync()" in source:
        production_unitofwork_save_calls.append(relative)
    direct_context_save_count += source.count("_context.SaveChangesAsync()")

test_unitofwork_save_surface: list[str] = []
for path in test_files:
    source = path.read_text(encoding="utf-8-sig")
    relative = path.relative_to(ROOT).as_posix()
    if ": IUnitOfWork" in source and "SaveChangesAsync" in source:
        test_unitofwork_save_surface.append(relative)

if production_unitofwork_save_calls:
    failures.append(
        "production UnitOfWork SaveChanges calls returned: "
        + ", ".join(production_unitofwork_save_calls)
    )
if test_unitofwork_save_surface:
    failures.append(
        "test IUnitOfWork SaveChanges surface returned: "
        + ", ".join(test_unitofwork_save_surface)
    )

transaction_contracts = (
    "Task<IDbContextTransaction> BeginTransactionAsync();",
    "public async Task<IDbContextTransaction> BeginTransactionAsync()",
    "_context.Database.BeginTransactionAsync()",
)
for contract in transaction_contracts:
    source = interface if contract.endswith(";") else implementation
    if contract not in source:
        failures.append(f"UnitOfWork transaction contract missing: {contract}")

if direct_context_save_count == 0:
    failures.append("direct AppDbContext persistence authority unexpectedly absent")

print("iunitofwork-savechanges-contract=absent")
print("unitofwork-savechanges-implementation=absent")
print(f"production-unitofwork-save-call-count={len(production_unitofwork_save_calls)}")
print(f"test-unitofwork-save-surface-count={len(test_unitofwork_save_surface)}")
print(f"production-direct-context-save-call-count={direct_context_save_count}")
print("unitofwork-transaction-authority=preserved")
print("unitofwork-authority=repositories-plus-transactions")

if failures:
    print("Backend UnitOfWork contract authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-unitofwork-contract-authority=clean")
