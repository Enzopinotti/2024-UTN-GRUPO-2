from __future__ import annotations

import json
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
APPSETTINGS = ROOT / "Backend" / "antigal.server" / "appsettings.json"

failures: list[str] = []


def expect(condition: bool, message: str) -> None:
    if not condition:
        failures.append(message)


def read_path(config: dict[str, object], *parts: str) -> object | None:
    current: object = config
    for part in parts:
        if not isinstance(current, dict) or part not in current:
            return None
        current = current[part]
    return current


config = json.loads(APPSETTINGS.read_text(encoding="utf-8-sig"))

sensitive_paths = [
    ("ConnectionStrings", "DefaultConnection"),
    ("JWTSettings", "securityKey"),
    ("Cloudinary", "CloudName"),
    ("Cloudinary", "ApiKey"),
    ("Cloudinary", "ApiSecret"),
    ("EmailConfiguration", "From"),
    ("EmailConfiguration", "Username"),
    ("EmailConfiguration", "Password"),
    ("EmailConfiguration", "SmtpServer"),
    ("MercadoPago", "AccessToken"),
    ("BootstrapAdmin", "UserName"),
    ("BootstrapAdmin", "Email"),
    ("BootstrapAdmin", "Password"),
]

for path in sensitive_paths:
    value = read_path(config, *path)
    label = ":".join(path)
    expect(value is not None, f"expected appsettings key is missing: {label}")
    if value is not None:
        expect(value == "", f"tracked appsettings value must be empty: {label}")

bootstrap_enabled = read_path(config, "BootstrapAdmin", "Enabled")
expect(
    bootstrap_enabled is False,
    "tracked BootstrapAdmin:Enabled must remain false",
)

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
    expect(
        not path.endswith(".csproj.user"),
        f"tracked Visual Studio user project state: {path}",
    )
    expect(
        not path.endswith(".pubxml.user"),
        f"tracked Visual Studio publish user state: {path}",
    )

if failures:
    print("Current-tree security baseline failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("Current-tree security baseline passed.")
print(f"sensitive-appsettings-values={len(sensitive_paths)} empty")
print("bootstrap-admin-tracked-enabled=false")
print("tracked-dotnet-generated-output=0")
print("tracked-environment-files=0")
