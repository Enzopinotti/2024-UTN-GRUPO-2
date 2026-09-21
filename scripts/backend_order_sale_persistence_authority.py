from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"

order_service = (BACKEND / "Services" / "OrderService.cs").read_text(encoding="utf-8-sig")
sale_service = (BACKEND / "Services" / "SaleService.cs").read_text(encoding="utf-8-sig")
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
update_sale = method(sale_service, r"public async Task<bool> UpdateSaleStatusAsync\(")
update_order_repo = method(order_repo, r"public async Task<bool> UpdateOrderStatusAsync\(")
create_sale_repo = method(sale_repo, r"public async Task<Sale\?> CreateSaleAsync\(")
update_sale_repo = method(sale_repo, r"public async Task<bool> UpdateSaleAsync\(")
update_product_repo = method(product_repo, r"public async Task<bool> UpdateProductAsync\(")

order_service_save_count = confirm.count("_unitOfWork.SaveChangesAsync()")
sale_service_save_count = update_sale.count("_unitOfWork.SaveChangesAsync()")

if order_service_save_count:
    failures.append(f"OrderService ConfirmOrder regained final UnitOfWork save: {order_service_save_count}")
if sale_service_save_count:
    failures.append(f"SaleService UpdateSaleStatusAsync regained UnitOfWork save: {sale_service_save_count}")

transaction_contracts = (
    "_unitOfWork.BeginTransactionAsync()",
    "transaction.CommitAsync()",
    "transaction.RollbackAsync()",
)
for contract in transaction_contracts:
    if contract not in confirm:
        failures.append(f"ConfirmOrder transaction contract missing: {contract}")

repository_mutations = {
    "order-status": (update_order_repo, "_context.SaveChangesAsync()"),
    "sale-create": (create_sale_repo, "_context.SaveChangesAsync()"),
    "sale-update": (update_sale_repo, "_context.SaveChangesAsync()"),
    "product-update": (update_product_repo, "_context.SaveChangesAsync()"),
}
for name, (source, save_call) in repository_mutations.items():
    count = source.count(save_call)
    if count != 1:
        failures.append(
            f"{name} repository persistence authority changed: expected 1 save, got {count}"
        )

confirm_mutation_calls = (
    "_unitOfWork.Orders.UpdateOrderStatusAsync",
    "_unitOfWork.Sales.CreateSaleAsync",
    "_unitOfWork.Products.UpdateProductAsync",
)
for contract in confirm_mutation_calls:
    if contract not in confirm:
        failures.append(f"ConfirmOrder repository mutation path missing: {contract}")

if "_unitOfWork.Sales.UpdateSaleAsync(sale)" not in update_sale:
    failures.append("SaleService repository mutation path missing")

print(f"order-confirm-service-save-count={order_service_save_count}")
print(f"sale-update-service-save-count={sale_service_save_count}")
print("order-confirm-transaction-authority=preserved")
print("order-confirm-mutation-persistence=repository-owned-inside-transaction")
print("sale-update-persistence=SaleRepository")
print("residual-service-unitofwork-save-count=0")

if failures:
    print("Backend order/sale persistence authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-order-sale-persistence-authority=clean")
