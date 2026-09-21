from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"
TESTS = ROOT / "Backend" / "antigal.server.Tests"

controller = (BACKEND / "Controllers" / "CartController.cs").read_text(encoding="utf-8-sig")
jwt = (BACKEND / "JwtFeatures" / "JwtHandler.cs").read_text(encoding="utf-8-sig")
tests = (TESTS / "CartControllerAuthorizationTests.cs").read_text(encoding="utf-8-sig")

failures: list[str] = []

for contract in (
    "[Authorize]",
    '[HttpGet("{userId}")]',
    '[HttpPost("{userId}")]',
    '[HttpPost("{userId}/items")]',
    '[HttpDelete("{userId}/items/{itemId}")]',
    '[HttpDelete("{userId}/clear")]',
    '[HttpPost("{userId}/confirmar")]',
):
    if contract not in controller:
        failures.append(f"Cart route/auth contract missing: {contract}")

for contract in (
    "ValidateOwnership(userId)",
    "User.FindFirstValue(JwtRegisteredClaimNames.Sub)",
    "User.FindFirstValue(ClaimTypes.NameIdentifier)",
    "string.Equals(authenticatedUserId, routeUserId, StringComparison.Ordinal)",
    "return Unauthorized(",
    "return Forbid();",
):
    if contract not in controller:
        failures.append(f"Cart ownership guard contract missing: {contract}")

if controller.count("ValidateOwnership(userId)") != 6:
    failures.append(
        "Every maintained Cart operation must validate route ownership before service delegation"
    )

ownership_position = controller.find("private ActionResult? ValidateOwnership")
if ownership_position < 0:
    failures.append("Cart ownership helper missing")
else:
    actions = controller[:ownership_position]
    for service_call in (
        "_cartService.GetCartByUserIdAsync(userId)",
        "_cartService.CreateCartAsync(userId)",
        "_cartService.AddItemToCartAsync(userId, addItemDto)",
        "_cartService.RemoveItemFromCartAsync(userId, itemId)",
        "_cartService.ClearCartAsync(userId)",
        "_cartService.ConfirmCartAsOrderAsync(userId)",
    ):
        service_position = actions.find(service_call)
        if service_position < 0:
            failures.append(f"Cart service delegation missing: {service_call}")
            continue

        guard_position = actions.rfind("ValidateOwnership(userId)", 0, service_position)
        if guard_position < 0:
            failures.append(f"Cart service delegation is not ownership-guarded: {service_call}")

if "new Claim(JwtRegisteredClaimNames.Sub, user.Id)" not in jwt:
    failures.append("JWT subject user-id authority changed")

for contract in (
    "Controller_RequiresAuthentication",
    "ForeignRouteUserId_BlocksEveryCartOperationBeforeService",
    "MatchingSubjectClaim_AllowsEveryCartOperation",
    "NameIdentifierClaim_IsAcceptedAsJwtMappingFallback",
    "MissingUserClaim_ReturnsUnauthorizedBeforeService",
    "Assert.AreEqual(0, service.TotalCalls)",
    "Assert.AreEqual(6, service.TotalCalls)",
):
    if contract not in tests:
        failures.append(f"Cart authorization test contract missing: {contract}")

print("cart-authentication-authority=required")
print("cart-route-contract=preserved")
print("cart-userid-authority=jwt-sub-or-nameidentifier")
print("cart-foreign-userid-result=forbid")
print("cart-missing-user-claim-result=unauthorized")
print("cart-ownership-guarded-operation-count=6")

if failures:
    print("Backend Cart ownership authorization authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-cart-ownership-authorization-authority=clean")
