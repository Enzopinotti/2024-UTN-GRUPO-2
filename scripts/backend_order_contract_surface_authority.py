from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"
TESTS = ROOT / "Backend" / "antigal.server.Tests"

service_contract = (BACKEND / "Services" / "IOrderService.cs").read_text(encoding="utf-8-sig")
service = (BACKEND / "Services" / "OrderService.cs").read_text(encoding="utf-8-sig")
repository_contract = (BACKEND / "Repositories" / "IOrderRepository.cs").read_text(encoding="utf-8-sig")
repository = (BACKEND / "Repositories" / "OrderRepository.cs").read_text(encoding="utf-8-sig")
tests = (TESTS / "OrderContractSurfaceTests.cs").read_text(encoding="utf-8-sig")

failures: list[str] = []

maintained_service = (
    "GetAllOrdersAsync",
    "GetOrderByIdAsync",
    "ConfirmOrder",
)
retired_service = (
    "GetOrdersByUserIdAsync",
    "GetOrdersByStatusAsync",
    "GetPendingOrderByUserIdAsync",
    "UpdateOrderStatusAsync",
)

maintained_repository = (
    "GetAllOrdersAsync",
    "GetOrderByIdAsync",
    "GetPendingOrderByUserIdAsync",
    "UpdateOrderStatusAsync",
)
retired_repository = (
    "GetOrdersByUserIdAsync",
    "GetOrdersByStatusAsync",
    "AddOrderAsync",
)

for name in maintained_service:
    if name not in service_contract:
        failures.append(f"IOrderService maintained operation missing: {name}")
    if name not in service:
        failures.append(f"OrderService maintained implementation missing: {name}")

for name in retired_service:
    if name in service_contract:
        failures.append(f"IOrderService retired operation returned: {name}")
    if f"public async" in service and name in service:
        failures.append(f"OrderService retired wrapper returned: {name}")

for name in maintained_repository:
    if name not in repository_contract:
        failures.append(f"IOrderRepository maintained operation missing: {name}")
    if name not in repository:
        failures.append(f"OrderRepository maintained implementation missing: {name}")

for name in retired_repository:
    if name in repository_contract:
        failures.append(f"IOrderRepository retired operation returned: {name}")
    if name in repository:
        failures.append(f"OrderRepository retired implementation returned: {name}")

if "_unitOfWork.Orders.GetPendingOrderByUserIdAsync(orderDto.idUsuario)" not in service:
    failures.append("ConfirmOrder pending-order repository authority missing")
if "_unitOfWork.Orders.UpdateOrderStatusAsync(orden.idOrden, orden.estado)" not in service:
    failures.append("ConfirmOrder order-status repository authority missing")

for contract in (
    "OrderServiceContract_ContainsOnlyMaintainedOperations",
    "OrderRepositoryContract_ContainsOnlyMaintainedOperations",
    "OrderService_ImplementationMatchesMaintainedContract",
    "OrderRepository_ImplementationMatchesMaintainedContract",
):
    if contract not in tests:
        failures.append(f"Order contract runtime proof missing: {contract}")

print("order-service-maintained-operations=3")
print("order-service-contract=GetAllOrdersAsync,GetOrderByIdAsync,ConfirmOrder")
print("order-repository-maintained-operations=4")
print("order-repository-contract=GetAllOrdersAsync,GetOrderByIdAsync,GetPendingOrderByUserIdAsync,UpdateOrderStatusAsync")
print("retired-order-service-wrapper-count=4")
print("retired-order-repository-operation-count=3")

if failures:
    print("Backend Order contract surface authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-order-contract-surface-authority=clean")
