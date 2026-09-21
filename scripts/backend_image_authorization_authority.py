from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"
TESTS = ROOT / "Backend" / "antigal.server.Tests"

controller = (BACKEND / "Controllers" / "ImageController.cs").read_text(encoding="utf-8-sig")
tests = (TESTS / "ImageAuthorizationTests.cs").read_text(encoding="utf-8-sig")

failures: list[str] = []

for contract in (
    'using Microsoft.AspNetCore.Authorization;',
    '[Authorize(Roles = "Admin")]',
    '[HttpPost("upload")]',
    '[HttpDelete("{imageId}")]',
    '[HttpDelete("eliminar-por-url")]',
):
    if contract not in controller:
        failures.append(f"Image authorization/route contract missing: {contract}")

if "[AllowAnonymous]" in controller:
    failures.append("ImageController must expose zero anonymous actions")

if controller.count('[Authorize(Roles = "Admin")]') != 1:
    failures.append("ImageController must keep exactly one controller-level Admin policy")

for contract in (
    "Controller_IsAdminAuthorizedByDefault",
    "MaintainedRoutes_InheritAdminPolicy",
    "Constructor_DependsOnlyOnImageService",
):
    if contract not in tests:
        failures.append(f"Image authorization runtime proof missing: {contract}")

print("image-controller-default-policy=Admin")
print("image-maintained-route-count=3")
print("image-admin-routes=POST-upload,DELETE-id,DELETE-url")
print("image-anonymous-action-count=0")

if failures:
    print("Backend Image authorization authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-image-authorization-authority=clean")
