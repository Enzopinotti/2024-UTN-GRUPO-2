from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROGRAM = ROOT / "Backend" / "antigal.server" / "Program.cs"
PRODUCTION_ROOTS = [
    ROOT / "Backend" / "antigal.server",
    ROOT / "Backend" / "EmailService",
]

failures: list[str] = []

program = PROGRAM.read_text(encoding="utf-8-sig")

registration_pattern = re.compile(
    r"builder\.Services\.Add(?P<lifetime>Scoped|Transient|Singleton)"
    r"<\s*IEmailSender\s*,\s*(?P<implementation>[^>]+)\s*>\s*\(\s*\)\s*;"
)
registrations = list(registration_pattern.finditer(program))

print(f"email-sender-registration-count={len(registrations)}")
for match in registrations:
    print(
        "email-sender-registration="
        f"{match.group('lifetime')}:{match.group('implementation').strip()}"
    )

if len(registrations) != 1:
    failures.append(
        "Program.cs must contain exactly one IEmailSender service registration"
    )
else:
    match = registrations[0]
    if match.group("lifetime") != "Transient":
        failures.append(
            "IEmailSender must preserve the historical effective Transient lifetime"
        )
    if match.group("implementation").strip() != "EmailSender":
        failures.append(
            "IEmailSender must resolve to the maintained EmailSender implementation"
        )

multi_resolution_patterns = [
    re.compile(r"IEnumerable\s*<\s*IEmailSender\s*>"),
    re.compile(r"GetServices\s*<\s*IEmailSender\s*>"),
]

multi_consumers: list[str] = []
for root in PRODUCTION_ROOTS:
    for path in root.rglob("*.cs"):
        if any(part in {"bin", "obj"} for part in path.parts):
            continue
        source = path.read_text(encoding="utf-8-sig")
        if any(pattern.search(source) for pattern in multi_resolution_patterns):
            multi_consumers.append(str(path.relative_to(ROOT)))

print(f"email-sender-multi-consumer-count={len(multi_consumers)}")
for path in multi_consumers:
    print(f"email-sender-multi-consumer={path}")

if multi_consumers:
    failures.append(
        "production code must not rely on multiple IEmailSender registrations "
        "without an explicit DI authority review"
    )

if failures:
    print("Backend email DI authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-email-di-authority=clean")
