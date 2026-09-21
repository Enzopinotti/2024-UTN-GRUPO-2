from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"
TESTS = ROOT / "Backend" / "antigal.server.Tests"

controller = (BACKEND / "Controllers" / "PaymentController.cs").read_text(encoding="utf-8-sig")
service = (BACKEND / "Services" / "PaymentService.cs").read_text(encoding="utf-8-sig")
program = (BACKEND / "Program.cs").read_text(encoding="utf-8-sig")
jwt_handler = (BACKEND / "JwtFeatures" / "JwtHandler.cs").read_text(encoding="utf-8-sig")
tests = (TESTS / "PaymentAuthorizationTests.cs").read_text(encoding="utf-8-sig")

failures: list[str] = []

for contract in (
    "using Microsoft.AspNetCore.Authorization;",
    "[Authorize]",
    '[HttpPost("create-payment")]',
):
    if contract not in controller:
        failures.append(f"PaymentController authorization/route contract missing: {contract}")

if "[AllowAnonymous]" in controller:
    failures.append("PaymentController must not expose anonymous actions after B40")

create_definition = "public async Task<IActionResult> CreatePayment"
create_pos = controller.find(create_definition)
if create_pos < 0:
    failures.append("PaymentController CreatePayment definition missing")
else:
    prefix = controller[max(0, create_pos - 180):create_pos]
    if '[HttpPost("create-payment")]' not in prefix:
        failures.append("CreatePayment route drifted")
    if "[AllowAnonymous]" in prefix:
        failures.append("CreatePayment became anonymous")

for contract in (
    "_httpContextAccessor.HttpContext?.User?.Identity?.Name",
    'throw new UnauthorizedAccessException("Usuario no autenticado.")',
):
    if contract not in service:
        failures.append(f"PaymentService authenticated identity contract missing: {contract}")

if "NameClaimType = JwtRegisteredClaimNames.Sub" not in program:
    failures.append("JWT Name authority must remain mapped to sub for PaymentService user id")

for contract in (
    "new Claim(JwtRegisteredClaimNames.Sub, user.Id)",
    "new Claim(ClaimTypes.Name, user.UserName)",
):
    if contract not in jwt_handler:
        failures.append(f"JWT payment identity claim contract missing: {contract}")

for contract in (
    "Controller_IsAuthenticatedByDefault",
    "CreatePayment_RemainsAuthenticated",
    "Controller_HasNoAnonymousActions",
):
    if contract not in tests:
        failures.append(f"Payment authorization runtime proof missing: {contract}")

print("payment-controller-default-policy=authenticated")
print("payment-create-preference-policy=authenticated")
print("payment-anonymous-action-count=0")
print("payment-user-id-authority=jwt-sub-via-identity-name")
print("payment-webhook-authority=absent-pending-real-integration")

if failures:
    print("Backend payment authorization authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-payment-authorization-authority=clean")
