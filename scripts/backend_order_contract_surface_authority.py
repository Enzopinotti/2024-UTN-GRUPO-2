from __future__ import annotations

import re
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

expected_service = {
    "GetAllOrdersAsync",
    "GetOrderByIdAsync",
    "ConfirmOrder",
}
expected_repository = {
    "GetAllOrdersAsync",
    "GetOrderByIdAsync",
    "GetPendingOrderByUserIdAsync",
    "UpdateOrderStatusAsync",
}

def interface_task_methods(source: str) -> set[str]:
    return set(
        re.findall(
            r"^\s*Task.*?\s+(\w+)\s*\(",
            source,
            flags=re.MULTILINE,
        )
    )

def public_async_task_methods(source: str) -> set[str]:
    return set(
        re.findall(
            r"^\s*public\s+async\s+Task.*?\s+(\w+)\s*\(",
            source,
            flags=re.MULTILINE,
        )
    )

service_contract_methods = interface_task_methods(service_contract)
service_methods = public_async_task_methods(service)
repository_contract_methods = interface_task_methods(repository_contract)
repository_methods = public_async_task_methods(repository)

if service_contract_methods != expected_service:
    failures.append(
        "IOrderService surface changed: "
        f"expected={sorted(expected_service)} actual={sorted(service_contract_methods)}"
    )

if service_methods != expected_service:
    failures.append(
        "OrderService public async surface changed: "
        f"expected={sorted(expected_service)} actual={sorted(service_methods)}"
    )

if repository_contract_methods != expected_repository:
    failures.append(
        "IOrderRepository surface changed: "
        f"expected={sorted(expected_repository)} actual={sorted(repository_contract_methods)}"
    )

if repository_methods != expected_repository:
    failures.append(
        "OrderRepository public async surface changed: "
        f"expected={sorted(expected_repository)} actual={sorted(repository_methods)}"
    )

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
