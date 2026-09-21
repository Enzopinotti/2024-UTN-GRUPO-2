from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"

files = [
    path for path in sorted(BACKEND.rglob("*.cs"))
    if not any(part in {"bin", "obj"} for part in path.parts)
]

failures: list[str] = []
outside_context_consumers: set[str] = set()
outside_save_distribution: dict[str, int] = {}

for path in files:
    source = path.read_text(encoding="utf-8-sig")
    relative = path.relative_to(ROOT).as_posix()
    is_repository = "/Repositories/" in relative

    if is_repository:
        continue

    constructor_with_context = re.search(
        r"public\s+\w+\([^)]*AppDbContext\s+context[^)]*\)",
        source,
        flags=re.S,
    )
    if constructor_with_context:
        outside_context_consumers.add(relative)

    save_count = source.count("_context.SaveChangesAsync()")
    if save_count:
        outside_save_distribution[relative] = save_count

if outside_context_consumers:
    failures.append(
        "non-repository AppDbContext consumers returned: "
        + str(sorted(outside_context_consumers))
    )

if outside_save_distribution:
    failures.append(
        "non-repository SaveChanges sites returned: "
        + str(outside_save_distribution)
    )

print(f"outside-repository-context-consumer-count={len(outside_context_consumers)}")
print("outside-repository-context-consumers=" + ",".join(sorted(outside_context_consumers)))
print(f"outside-repository-save-site-count={sum(outside_save_distribution.values())}")
print(
    "outside-repository-save-distribution="
    + ",".join(
        f"{path}:{count}" for path, count in sorted(outside_save_distribution.items())
    )
)
print("non-repository-appdbcontext-authority=zero")
print("non-repository-savechanges-authority=zero")

if failures:
    print("Backend direct-context persistence authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-direct-context-persistence-authority=clean")
