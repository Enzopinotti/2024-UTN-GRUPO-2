from __future__ import annotations

import re
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
APPSETTINGS = ROOT / "Backend" / "antigal.server" / "appsettings.json"

failures: list[str] = []


def expect(condition: bool, message: str) -> None:
    if not condition:
        failures.append(message)


text = APPSETTINGS.read_text(encoding="utf-8-sig")

sensitive_keys = [
    "DefaultConnection",
    "securityKey",
    "CloudName",
    "ApiKey",
    "ApiSecret",
    "From",
    "Username",
    "Password",
    "SmtpServer",
    "AccessToken",
]

for key in sensitive_keys:
    match = re.search(rf'"{re.escape(key)}"\s*:\s*"([^"]*)"', text)
    expect(match is not None, f"expected appsettings key is missing: {key}")
    if match:
        expect(match.group(1) == "", f"tracked appsettings value must be empty: {key}")

tracked = subprocess.check_output(
    ["git", "ls-files"],
    cwd=ROOT,
    text=True,
).splitlines()

for path in tracked:
    normalized = f"/{path}/"
    expect("/bin/" not in normalized, f"tracked .NET bin output: {path}")
    expect("/obj/" not in normalized, f"tracked .NET obj output: {path}")
    expect(
        not Path(path).name.startswith(".env"),
        f"tracked environment file: {path}",
    )

if failures:
    print("Current-tree security baseline failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("Current-tree security baseline passed.")
print(f"sensitive-appsettings-values={len(sensitive_keys)} empty")
print("tracked-dotnet-generated-output=0")
print("tracked-environment-files=0")
