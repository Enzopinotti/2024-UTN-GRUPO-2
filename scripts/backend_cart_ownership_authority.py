from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"
TESTS = ROOT / "Backend" / "antigal.server.Tests"

controller = (BACKEND / "Controllers" / "CartController.cs").read_text(encoding="utf-8-sig")
program = (BACKEND / "Program.cs").read_text(encoding="utf-8-sig")
jwt_handler = (BACKEND / "JwtFeatures" / "JwtHandler.cs").read_text(encoding="utf-8-sig")
tests = (TESTS / "CartAuthorizationTests.cs").read_text(encoding="utf-8-sig")

failures: list[str] = []

for contract in (
    "using Microsoft.AspNetCore.Authorization;",
    "[Authorize]",
    '[HttpGet("{userId}")]',
    '[HttpPost("{userId}")]',
    '[HttpPost("{userId}/items")]',
    '[HttpDelete("{userId}/items/{itemId}")]',
    '[HttpDelete("{userId}/clear")]',
    '[HttpPost("{userId}/confirmar")]',
):
    if contract not in controller:
        failures.append(f"Cart authentication/route contract missing: {contract}")

if "[AllowAnonymous]" in controller:
    failures.append("CartController must not expose anonymous actions")

ownership_guard = "if (!IsCurrentUser(userId)) return Forbid();"
if controller.count(ownership_guard) != 6:
    failures.append("Every one of the six Cart actions must enforce the ownership guard")

for contract in (
    "var authenticatedUserId = User.Identity?.Name;",
    "string.Equals(authenticatedUserId, userId, StringComparison.Ordinal)",
):
    if contract not in controller:
        failures.append(f"Cart ownership helper contract missing: {contract}")

if "NameClaimType = JwtRegisteredClaimNames.Sub" not in program:
    failures.append("JWT Name authority must remain mapped to sub for Cart ownership")

if "new Claim(JwtRegisteredClaimNames.Sub, user.Id)" not in jwt_handler:
    failures.append("JwtHandler must keep emitting sub=user.Id for Cart ownership")

for contract in (
    "Controller_IsAuthenticatedByDefault",
    "Routes_PreserveHistoricalUserIdTemplates",
    "MismatchedUser_IsForbiddenAcrossEveryAction_WithoutServiceCalls",
    "MatchingJwtSubject_DelegatesRequestedUserId",
):
    if contract not in tests:
        failures.append(f"Cart ownership runtime proof missing: {contract}")

print("cart-controller-default-policy=authenticated")
print("cart-route-contract=preserved")
print("cart-ownership-key=jwt-sub")
print("cart-userid-route-authority=must-match-authenticated-sub")
print("cart-owned-action-count=6")
print("cart-anonymous-action-count=0")

if failures:
    print("Backend cart ownership authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-cart-ownership-authority=clean")
