from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"
TESTS = ROOT / "Backend" / "antigal.server.Tests"

absent_paths = (
    BACKEND / "Controllers" / "SaleController.cs",
    BACKEND / "Services" / "ISaleService.cs",
    BACKEND / "Services" / "SaleService.cs",
    BACKEND / "Models" / "Dto" / "VentaDtos" / "SaleRequestDto.cs",
    BACKEND / "Models" / "Dto" / "VentaDtos" / "SaleResponseDto.cs",
    BACKEND / "Models" / "Dto" / "VentaDtos" / "SaleDto.cs",
)

program = (BACKEND / "Program.cs").read_text(encoding="utf-8-sig")
orders_controller = (BACKEND / "Controllers" / "OrdersController.cs").read_text(encoding="utf-8-sig")
order_service = (BACKEND / "Services" / "OrderService.cs").read_text(encoding="utf-8-sig")
sale_interface = (BACKEND / "Repositories" / "ISaleRepository.cs").read_text(encoding="utf-8-sig")
sale_repository = (BACKEND / "Repositories" / "SaleRepository.cs").read_text(encoding="utf-8-sig")
tests = (TESTS / "DeadSaleApiSurfaceTests.cs").read_text(encoding="utf-8-sig")

failures: list[str] = []

for path in absent_paths:
    if path.exists():
        failures.append(f"retired Sale API file returned: {path.relative_to(ROOT).as_posix()}")

for forbidden in ("ISaleService", "SaleService"):
    if forbidden in program:
        failures.append(f"dead Sale service DI returned: {forbidden}")
    if forbidden in orders_controller:
        failures.append(f"OrdersController dead Sale dependency returned: {forbidden}")

if "public OrdersController(IOrderService orderService)" not in orders_controller:
    failures.append("OrdersController constructor is not IOrderService-only")

for forbidden in ("SaleResponseDto", "SaleDto"):
    if forbidden in order_service:
        failures.append(f"OrderService regained dead Sale DTO construction: {forbidden}")

if "_unitOfWork.Sales.CreateSaleAsync(sale)" not in order_service:
    failures.append("OrderService maintained Sale creation path missing")

if "Task<Sale?> CreateSaleAsync(Sale sale);" not in sale_interface:
    failures.append("ISaleRepository CreateSaleAsync contract missing")
for forbidden in ("GetSaleByIdAsync", "UpdateSaleAsync"):
    if forbidden in sale_interface:
        failures.append(f"ISaleRepository regained retired operation: {forbidden}")
    if forbidden in sale_repository:
        failures.append(f"SaleRepository regained retired operation: {forbidden}")

if sale_repository.count("_context.SaveChangesAsync()") != 1:
    failures.append("SaleRepository must own exactly one maintained create save site")

for contract in (
    "ProductionAssembly_DoesNotExposeSaleController",
    "SaleServiceTypes_AreAbsent",
    "OrdersController_Constructor_DependsOnlyOnOrderService",
    "SaleRepositoryContract_IsCreateOnly",
    "LegacySaleDtos_AreAbsent",
):
    if contract not in tests:
        failures.append(f"dead Sale API runtime proof missing: {contract}")

print("sale-controller=absent")
print("sale-service-contract=absent")
print("sale-service=absent")
print("legacy-sale-dtos=absent")
print("orderscontroller-sale-service-dependency=absent")
print("sale-repository-authority=CreateSaleAsync-only")
print("order-confirm-sale-creation=preserved")

if failures:
    print("Backend dead Sale API authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-dead-sale-api-authority=clean")
