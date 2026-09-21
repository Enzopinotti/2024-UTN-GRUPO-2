from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"
PROGRAM = BACKEND / "Program.cs"
JWT_HANDLER = BACKEND / "JwtFeatures" / "JwtHandler.cs"
LEGACY_SERVICE = BACKEND / "Services" / "ServiceToken.cs"

failures: list[str] = []

if LEGACY_SERVICE.exists():
    failures.append("legacy ServiceToken.cs must remain absent")

program = PROGRAM.read_text(encoding="utf-8-sig")
if "AddScoped<ServiceToken>" in program:
    failures.append("legacy ServiceToken DI registration must remain absent")
if program.count("AddSingleton<JwtHandler>()") != 1:
    failures.append("Program.cs must retain exactly one canonical JwtHandler registration")

legacy_config = re.compile(r'["\']Jwt:(?:Key|Issuer|Audience)["\']')
legacy_methods = re.compile(
    r"\b(?:GenerateRefreshToken|GetPrincipalFromExpiredToken|GenerateEmailConfirmationToken)\b"
)

legacy_config_files: list[str] = []
legacy_method_files: list[str] = []

for path in sorted(BACKEND.rglob("*.cs")):
    if any(part in {"bin", "obj"} for part in path.parts):
        continue
    source = path.read_text(encoding="utf-8-sig")
    if legacy_config.search(source):
        legacy_config_files.append(str(path.relative_to(ROOT)))
    if legacy_methods.search(source):
        legacy_method_files.append(str(path.relative_to(ROOT)))

if legacy_config_files:
    failures.append(
        "legacy Jwt:* configuration authority returned in: "
        + ", ".join(legacy_config_files)
    )
if legacy_method_files:
    failures.append(
        "legacy ServiceToken method authority returned in: "
        + ", ".join(legacy_method_files)
    )

handler = JWT_HANDLER.read_text(encoding="utf-8-sig")
required = [
    'GetSection("JWTSettings")',
    'JWTSettings:expiryInMinutes',
]
for contract in required:
    if contract not in handler:
        failures.append(f"canonical JwtHandler contract missing: {contract}")

print(f"legacy-servicetoken-file-count={int(LEGACY_SERVICE.exists())}")
print(f"legacy-jwt-config-file-count={len(legacy_config_files)}")
print(f"legacy-servicetoken-method-file-count={len(legacy_method_files)}")

if failures:
    print("Backend token authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("servicetoken-authority=absent")
print("legacy-jwt-config-authority=absent")
print("jwt-handler-authority=canonical")
