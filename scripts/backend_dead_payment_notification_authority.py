from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKEND = ROOT / "Backend" / "antigal.server"
TESTS = ROOT / "Backend" / "antigal.server.Tests"

sources = {
    "controller": (BACKEND / "Controllers" / "PaymentController.cs").read_text(encoding="utf-8-sig"),
    "service-interface": (BACKEND / "Services" / "IPaymentService.cs").read_text(encoding="utf-8-sig"),
    "service": (BACKEND / "Services" / "PaymentService.cs").read_text(encoding="utf-8-sig"),
    "repository-interface": (BACKEND / "Repositories" / "IPaymentRepository.cs").read_text(encoding="utf-8-sig"),
    "repository": (BACKEND / "Repositories" / "PaymentRepository.cs").read_text(encoding="utf-8-sig"),
}
tests = (TESTS / "PaymentNotificationSurfaceTests.cs").read_text(encoding="utf-8-sig")

failures: list[str] = []

for name, source in sources.items():
    for forbidden in (
        "ReceiveNotification",
        "HandlePaymentNotificationAsync",
        "UpdatePaymentStatusAsync",
    ):
        if forbidden in source:
            failures.append(f"legacy payment notification marker returned in {name}: {forbidden}")

controller = sources["controller"]
if '[HttpPost("notification")]' in controller:
    failures.append("legacy payment notification route returned")
if "[AllowAnonymous]" in controller:
    failures.append("PaymentController regained an anonymous action")
if '[HttpPost("create-payment")]' not in controller:
    failures.append("authenticated payment preference route missing")
if "_paymentService.CreatePaymentPreferenceAsync(amount, title, quantity)" not in controller:
    failures.append("payment preference delegation missing")

service_interface = sources["service-interface"]
repository_interface = sources["repository-interface"]
if service_interface.count("Task<") != 1:
    failures.append("IPaymentService should expose only maintained preference creation")
if repository_interface.count("Task<") != 1:
    failures.append("IPaymentRepository should expose only maintained payment creation persistence")

for contract in (
    "PaymentController_DoesNotExposeLegacyNotificationAction",
    "PaymentServiceContract_DoesNotExposeLegacyNotificationHandler",
    "PaymentRepositoryContract_DoesNotExposeUnverifiedStatusMutation",
):
    if contract not in tests:
        failures.append(f"payment notification runtime absence proof missing: {contract}")

print("legacy-payment-notification-route=absent")
print("legacy-payment-notification-service-contract=absent")
print("legacy-payment-status-query-mutation=absent")
print("payment-preference-creation=preserved")
print("real-mercadopago-webhook=not-implemented")

if failures:
    print("Backend dead payment notification authority failed:", file=sys.stderr)
    for failure in failures:
        print(f"- {failure}", file=sys.stderr)
    raise SystemExit(1)

print("backend-dead-payment-notification-authority=clean")
