from __future__ import annotations

import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"

initializer = (BACKEND / "Data" / "DbInitializer.cs").read_text(encoding="utf-8-sig")
options = (BACKEND / "Data" / "AdminBootstrapOptions.cs").read_text(encoding="utf-8-sig")
policy = (BACKEND / "Data" / "AdminBootstrapPolicy.cs").read_text(encoding="utf-8-sig")
program = (BACKEND / "Program.cs").read_text(encoding="utf-8-sig")
appsettings = json.loads((BACKEND / "appsettings.json").read_text(encoding="utf-8-sig"))

failures: list[str] = []

for legacy_assignment in (
    r"string\s+adminEmail\s*=\s*\"",
    r"string\s+adminPassword\s*=\s*\"",
):
    if re.search(legacy_assignment, initializer):
        failures.append("DbInitializer regained embedded administrator credential authority")

for contract in (
    "IOptions<AdminBootstrapOptions>",
    "AdminBootstrapPolicy.Resolve(",
    "environment.IsDevelopment()",
    "FindByEmailAsync(bootstrap.Email)",
    "UserName = bootstrap.UserName",
    "Email = bootstrap.Email",
    "CreateAsync(adminUser, bootstrap.Password)",
    'AddToRoleAsync(adminUser, "Admin")',
):
    if contract not in initializer:
        failures.append(f"DbInitializer bootstrap contract missing: {contract}")

for contract in (
    'public const string SectionName = "BootstrapAdmin";',
    "public bool Enabled { get; set; }",
    "public string? UserName { get; set; }",
    "public string? Email { get; set; }",
    "public string? Password { get; set; }",
):
    if contract not in options:
        failures.append(f"AdminBootstrapOptions contract missing: {contract}")

for contract in (
    "if (!options.Enabled)",
    "return null;",
    "if (!isDevelopment)",
    "throw new InvalidOperationException(",
    "string.IsNullOrWhiteSpace(options.UserName)",
    "string.IsNullOrWhiteSpace(options.Email)",
    "string.IsNullOrWhiteSpace(options.Password)",
):
    if contract not in policy:
        failures.append(f"AdminBootstrapPolicy safety contract missing: {contract}")

for contract in (
    "builder.Services.Configure<AdminBootstrapOptions>(",
    "builder.Configuration.GetSection(AdminBootstrapOptions.SectionName)",
):
    if contract not in program:
        failures.append(f"Program bootstrap registration missing: {contract}")

bootstrap = appsettings.get("BootstrapAdmin")
if not isinstance(bootstrap, dict):
    failures.append("BootstrapAdmin appsettings section missing")
else:
    if bootstrap.get("Enabled") is not False:
        failures.append("BootstrapAdmin must be tracked disabled")
    for key in ("UserName", "Email", "Password"):
        if bootstrap.get(key) != "":
            failures.append(f"BootstrapAdmin tracked {key} must be empty")

print("bootstrap-admin-default-enabled=false")
print("bootstrap-admin-environment-authority=Development-only")
print("bootstrap-admin-credential-authority=external-configuration")
print("bootstrap-admin-embedded-credentials=absent")
print("bootstrap-admin-required-fields=UserName,Email,Password")
print("bootstrap-role-authority=Admin,User,Visitor")

if failures:
    print("Backend admin bootstrap authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-admin-bootstrap-authority=clean")
