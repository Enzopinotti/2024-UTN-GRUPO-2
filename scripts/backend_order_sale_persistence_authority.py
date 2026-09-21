from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"

order_service = (BACKEND / "Services" / "OrderService.cs").read_text(encoding="utf-8-sig")
order_repo = (BACKEND / "Repositories" / "OrderRepository.cs").read_text(encoding="utf-8-sig")
sale_repo = (BACKEND / "Repositories" / "SaleRepository.cs").read_text(encoding="utf-8-sig")
product_repo = (BACKEND / "Repositories" / "ProductRepository.cs").read_text(encoding="utf-8-sig")

failures: list[str] = []

def method(source: str, signature: str) -> str:
    match = re.search(signature + r".*?\n        \}", source, flags=re.S)
    if not match:
        failures.append(f"method missing for pattern: {signature}")
        return ""
    return match.group(0)

confirm = method(order_service, r"public async Task ConfirmOrder\(")
update_order_repo = method(order_repo, r"public async Task<bool> UpdateOrderStatusAsync\(")
create_sale_repo = method(sale_repo, r"public async Task<Sale\?> CreateSaleAsync\(")
update_product_repo = method(product_repo, r"public async Task<bool> UpdateProductAsync\(")

order_service_save_count = confirm.count("_unitOfWork.SaveChangesAsync()")
if order_service_save_count:
    failures.append(f"OrderService ConfirmOrder regained final UnitOfWork save: {order_service_save_count}")

for contract in (
    "_unitOfWork.BeginTransactionAsync()",
    "transaction.CommitAsync()",
    "transaction.RollbackAsync()",
):
    if contract not in confirm:
        failures.append(f"ConfirmOrder transaction contract missing: {contract}")

repository_mutations = {
    "order-status": update_order_repo,
    "sale-create": create_sale_repo,
    "product-update": update_product_repo,
}
for name, source in repository_mutations.items():
    count = source.count("_context.SaveChangesAsync()")
    if count != 1:
        failures.append(
            f"{name} repository persistence authority changed: expected 1 save, got {count}"
        )

for contract in (
    "_unitOfWork.Orders.UpdateOrderStatusAsync",
    "_unitOfWork.Sales.CreateSaleAsync",
    "_unitOfWork.Products.UpdateProductAsync",
):
    if contract not in confirm:
        failures.append(f"ConfirmOrder repository mutation path missing: {contract}")

for dead_contract in ("GetSaleByIdAsync", "UpdateSaleAsync"):
    if dead_contract in sale_repo:
        failures.append(f"SaleRepository regained retired operation: {dead_contract}")

print(f"order-confirm-service-save-count={order_service_save_count}")
print("order-confirm-transaction-authority=preserved")
print("order-confirm-mutation-persistence=repository-owned-inside-transaction")
print("sale-create-persistence=SaleRepository")
print("sale-repository-maintained-operations=CreateSaleAsync")
print("residual-service-unitofwork-save-count=0")

if failures:
    print("Backend order/sale persistence authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-order-sale-persistence-authority=clean")
