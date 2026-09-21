from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PRODUCT_REPOSITORY = ROOT / "Backend" / "antigal.server" / "Repositories" / "ProductRepository.cs"
PRODUCT_SERVICE = ROOT / "Backend" / "antigal.server" / "Services" / "ProductService.cs"

repository = PRODUCT_REPOSITORY.read_text(encoding="utf-8-sig")
service = PRODUCT_SERVICE.read_text(encoding="utf-8-sig")
failures: list[str] = []

repository_methods = {
    "add": r"public async Task<Producto> AddProductAsync\(.*?\n        \}",
    "update": r"public async Task<bool> UpdateProductAsync\(.*?\n        \}",
    "delete": r"public async Task<bool> DeleteProductAsync\(.*?\n        \}",
    "import": r"public async Task<IEnumerable<Producto>> ImportProductsFromExcelAsync\(.*?\n        \}",
}

repository_self_save_count = 0
for name, pattern in repository_methods.items():
    match = re.search(pattern, repository, flags=re.S)
    if not match:
        failures.append(f"ProductRepository method missing: {name}")
        continue
    method = match.group(0)
    count = method.count("_context.SaveChangesAsync()")
    repository_self_save_count += count
    if count != 1:
        failures.append(
            f"ProductRepository {name} persistence authority changed: expected 1 SaveChangesAsync, got {count}"
        )

service_second_save_count = service.count("_unitOfWork.SaveChangesAsync()")
if service_second_save_count:
    failures.append(
        f"ProductService regained redundant UnitOfWork persistence: {service_second_save_count} calls"
    )

add_match = re.search(
    r"public async Task<ResponseDto> AddProductAsync\(.*?\n        \}",
    service,
    flags=re.S,
)
if not add_match:
    failures.append("ProductService AddProductAsync missing")
else:
    add_method = add_match.group(0)
    if "productoExistente.Any()" not in add_method:
        failures.append("ProductService duplicate guard must use Any() over title-query results")
    if "productoExistente != null" in add_method:
        failures.append("ProductService null duplicate guard returned")

for method_name, repository_call in (
    ("AddProductAsync", "_unitOfWork.Products.AddProductAsync(producto)"),
    ("DeleteProductAsync", "_unitOfWork.Products.DeleteProductAsync(id)"),
    ("PutProductAsync", "_unitOfWork.Products.UpdateProductAsync(productoExistente)"),
    ("ImportProductsFromExcelAsync", "_unitOfWork.Products.ImportProductsFromExcelAsync(stream)"),
):
    match = re.search(
        rf"public async Task<ResponseDto> {method_name}\(.*?\n        \}}",
        service,
        flags=re.S,
    )
    if not match or repository_call not in match.group(0):
        failures.append(f"ProductService repository mutation path missing: {method_name}")

print(f"product-repository-self-save-count={repository_self_save_count}")
print(f"product-service-second-save-count={service_second_save_count}")
print("product-persistence-authority=ProductRepository")
print("product-add-duplicate-guard=Any")

if failures:
    print("Backend product persistence authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-product-persistence-authority=clean")
