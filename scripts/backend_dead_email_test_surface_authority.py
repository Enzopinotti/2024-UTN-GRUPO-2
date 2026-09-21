from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"
EMAIL_PROJECT = ROOT / "Backend" / "EmailService"

failures: list[str] = []

removed_paths = (
    BACKEND / "Controllers" / "EmailController .cs",
    BACKEND / "Models" / "Dto" / "TestEmailDto.cs",
)
for path in removed_paths:
    if path.exists():
        failures.append(f"dead email test surface returned: {path.relative_to(ROOT).as_posix()}")

account = (BACKEND / "Controllers" / "AccountController.cs").read_text(encoding="utf-8-sig")
program = (BACKEND / "Program.cs").read_text(encoding="utf-8-sig")
sender = (EMAIL_PROJECT / "EmailSender.cs").read_text(encoding="utf-8-sig")
sender_interface = (EMAIL_PROJECT / "IEmailSender.cs").read_text(encoding="utf-8-sig")

for contract in (
    "private readonly IEmailSender _emailSender;",
    "IEmailSender emailSender",
    "Email Confirmation",
    "Password Reset",
    "await _emailSender.SendEmailAsync(message);",
):
    if contract not in account:
        failures.append(f"Account email production contract missing: {contract}")

if "builder.Services.AddTransient<IEmailSender, EmailSender>();" not in program:
    failures.append("canonical IEmailSender DI registration missing")

for contract in (
    "public class EmailSender : IEmailSender",
    "public async Task SendEmailAsync(Message message)",
):
    if contract not in sender:
        failures.append(f"EmailSender production contract missing: {contract}")

for contract in (
    "void SendEmail(Message message);",
    "Task SendEmailAsync(Message message);",
):
    if contract not in sender_interface:
        failures.append(f"IEmailSender production contract missing: {contract}")

production_sources = [
    path
    for path in BACKEND.rglob("*.cs")
    if not any(part in {"bin", "obj"} for part in path.parts)
]
for path in production_sources:
    source = path.read_text(encoding="utf-8-sig")
    relative = path.relative_to(ROOT).as_posix()
    for marker in (
        "SendTestEmail",
        "Email sent successfully.",
        "This is content from our email.",
    ):
        if marker in source:
            failures.append(f"dead test email marker returned in {relative}: {marker}")

print("email-test-controller=absent")
print("test-email-dto=absent")
print("account-email-confirmation-authority=preserved")
print("account-password-reset-email-authority=preserved")
print("email-sender-di-authority=preserved")
print("live-test-email-endpoint-authority=absent")

if failures:
    print("Backend dead email test surface authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-dead-email-test-surface-authority=clean")
