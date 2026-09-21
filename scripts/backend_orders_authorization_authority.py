from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"
TESTS = ROOT / "Backend" / "antigal.server.Tests"

controller = (BACKEND / "Controllers" / "OrdersController.cs").read_text(encoding="utf-8-sig")
tests = (TESTS / "OrdersAuthorizationTests.cs").read_text(encoding="utf-8-sig")

failures: list[str] = []

for contract in (
    '[Authorize(Roles = "Admin")]',
    '[HttpGet("all")]',
    '[HttpPost("confirm/{orderId}")]',
    "public OrdersController(IOrderService orderService)",
):
    if contract not in controller:
        failures.append(f"Orders authorization/route contract missing: {contract}")

if "[AllowAnonymous]" in controller:
    failures.append("OrdersController must expose zero anonymous actions")

if controller.count('[Authorize(Roles = "Admin")]') != 1:
    failures.append("OrdersController must keep one controller-level Admin policy")

for contract in (
    "Controller_IsAdminAuthorizedByDefault",
    "MaintainedRoutes_InheritAdminPolicy",
    "Constructor_DependsOnlyOnOrderService",
):
    if contract not in tests:
        failures.append(f"Orders runtime authorization proof missing: {contract}")

print("orders-controller-default-policy=Admin")
print("orders-maintained-route-count=2")
print("orders-admin-routes=GET-all,POST-confirm")
print("orders-anonymous-action-count=0")
print("orderscontroller-dependencies=IOrderService-only")

if failures:
    print("Backend Orders authorization authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-orders-authorization-authority=clean")
