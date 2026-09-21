from __future__ import annotations

import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
APPSETTINGS = ROOT / "Backend" / "antigal.server" / "appsettings.json"
CONFIGURATION = ROOT / "Backend" / "antigal.server" / "CONFIGURATION.md"
JWT_HANDLER = ROOT / "Backend" / "antigal.server" / "JwtFeatures" / "JwtHandler.cs"

failures: list[str] = []

settings = json.loads(APPSETTINGS.read_text(encoding="utf-8-sig"))
expiry = settings.get("JWTSettings", {}).get("expiryInMinutes")

print(f"jwt-expiry-default-minutes={expiry}")

if type(expiry) is not int or expiry <= 0:
    failures.append("tracked JWTSettings.expiryInMinutes must be a positive integer")
if expiry != 60:
    failures.append("maintained JWT expiration default must remain 60 minutes")

configuration = CONFIGURATION.read_text(encoding="utf-8-sig")
if "JWTSettings__expiryInMinutes" not in configuration:
    failures.append("CONFIGURATION.md must document JWTSettings__expiryInMinutes")

handler = JWT_HANDLER.read_text(encoding="utf-8-sig")
required_handler_contracts = [
    "int.TryParse(configuredExpiry",
    "expiryInMinutes <= 0",
    "DateTime.UtcNow.AddMinutes(expiryInMinutes)",
    "JWTSettings:expiryInMinutes",
]
for contract in required_handler_contracts:
    if contract not in handler:
        failures.append(f"JwtHandler expiry validation contract missing: {contract}")

if failures:
    print("JWT expiration configuration authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("jwt-expiry-validation=positive-integer")
print("jwt-expiry-configuration-authority=clean")
