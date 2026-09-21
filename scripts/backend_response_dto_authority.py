from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"
PROGRAM = BACKEND / "Program.cs"
PRODUCT_SERVICE = BACKEND / "Services" / "ProductService.cs"

program = PROGRAM.read_text(encoding="utf-8-sig")
product_service = PRODUCT_SERVICE.read_text(encoding="utf-8-sig")
failures: list[str] = []

registration_patterns = (
    "AddTransient<ResponseDto>",
    "AddScoped<ResponseDto>",
    "AddSingleton<ResponseDto>",
)
for pattern in registration_patterns:
    if pattern in program:
        failures.append(f"ResponseDto DI registration returned: {pattern}")

constructor_pattern = re.compile(
    r"[\(,]\s*ResponseDto\s+[A-Za-z_][A-Za-z0-9_]*\s*(?:[,\)])"
)
field_pattern = re.compile(
    r"\bResponseDto\s+_[A-Za-z_][A-Za-z0-9_]*\s*;"
)

constructor_consumers: list[str] = []
field_consumers: list[str] = []
local_allocation_count = 0
local_allocation_files = 0
new_pattern = re.compile(r"\bnew\s+ResponseDto\s*(?:\(|\{)")

for path in sorted(BACKEND.rglob("*.cs")):
    if any(part in {"bin", "obj"} for part in path.parts):
        continue
    source = path.read_text(encoding="utf-8-sig")
    relative = path.relative_to(ROOT).as_posix()
    if constructor_pattern.search(source):
        constructor_consumers.append(relative)
    if field_pattern.search(source):
        field_consumers.append(relative)
    count = len(new_pattern.findall(source))
    if count:
        local_allocation_count += count
        local_allocation_files += 1

if constructor_consumers:
    failures.append(
        "ResponseDto constructor injection returned: " + ", ".join(constructor_consumers)
    )
if field_consumers:
    failures.append(
        "ResponseDto injected/shared field state returned: " + ", ".join(field_consumers)
    )

required_product_contracts = (
    "public ProductService(IUnitOfWork unitOfWork)",
    "var response = new ResponseDto();",
    "return response;",
)
for contract in required_product_contracts:
    if contract not in product_service:
        failures.append(f"ProductService local response contract missing: {contract}")

if "_response" in product_service:
    failures.append("ProductService regained shared _response state")

print("responsedto-di-registration=absent")
print(f"responsedto-constructor-consumer-count={len(constructor_consumers)}")
print(f"responsedto-field-consumer-count={len(field_consumers)}")
print(f"responsedto-local-allocation-count={local_allocation_count}")
print(f"responsedto-local-allocation-file-count={local_allocation_files}")
print("productservice-response-authority=method-local")

if failures:
    print("Backend ResponseDto authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-responsedto-authority=clean")
