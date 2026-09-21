from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
CONTROLLER = ROOT / "Backend" / "antigal.server" / "Controllers" / "ProductCategoryController.cs"
TEST = ROOT / "Backend" / "antigal.server.Tests" / "ProductCategoryAuthorizationTests.cs"

controller = CONTROLLER.read_text(encoding="utf-8-sig")
tests = TEST.read_text(encoding="utf-8-sig")

failures: list[str] = []

for contract in (
    "using Microsoft.AspNetCore.Authorization;",
    '[Authorize(Roles = "Admin")]',
    '[HttpPost("asignar")]',
    '[HttpDelete("desasignar")]',
    '[HttpGet("categorias/{idProducto}")]',
    '[HttpGet("productos/{idCategoria}")]',
):
    if contract not in controller:
        failures.append(f"ProductCategory authorization/route contract missing: {contract}")

if controller.count("[AllowAnonymous]") != 2:
    failures.append("ProductCategoryController must expose exactly two anonymous read endpoints")

for method_marker in (
    "AsignarCategoriaAProductoAsync",
    "DesasignarCategoriaDeProductoAsync",
):
    method_pos = controller.find(method_marker)
    if method_pos < 0:
        failures.append(f"ProductCategory mutation missing: {method_marker}")
        continue
    prefix = controller[max(0, method_pos - 180):method_pos]
    if "[AllowAnonymous]" in prefix:
        failures.append(f"ProductCategory mutation became anonymous: {method_marker}")

for method_marker in (
    "ObtenerCategoriasDeProductoAsync",
    "ObtenerProductosDeCategoriaAsync",
):
    method_pos = controller.find(method_marker)
    if method_pos < 0:
        failures.append(f"ProductCategory read missing: {method_marker}")
        continue
    prefix = controller[max(0, method_pos - 180):method_pos]
    if "[AllowAnonymous]" not in prefix:
        failures.append(f"ProductCategory public read lost AllowAnonymous: {method_marker}")

for contract in (
    "Controller_IsAdminAuthorizedByDefault",
    "MutationEndpoints_RemainAdminProtected",
    "ReadEndpoints_RemainExplicitlyAnonymous",
):
    if contract not in tests:
        failures.append(f"ProductCategory runtime authorization proof missing: {contract}")

print("product-category-default-policy=Admin")
print("product-category-anonymous-read-count=2")
print("product-category-admin-mutations=POST-asignar,DELETE-desasignar")
print("product-category-public-reads=GET-categorias,GET-productos")

if failures:
    print("Backend ProductCategory authorization authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-product-category-authorization-authority=clean")
