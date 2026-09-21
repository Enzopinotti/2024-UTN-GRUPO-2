# B40 — Dead payment notification surface removal

## Goal

Retire a public payment-status mutation surface that could not establish notification authenticity.

## Characterization

The legacy callback accepted:

- `paymentId`
- `status`

directly from query parameters and delegated them into local payment-state mutation.

That is not a trustworthy Mercado Pago webhook contract. Caller-supplied status is not canonical provider state and the path performed no maintained authenticity verification.

No maintained frontend consumer required the callback.

## Decision

B40 fails closed by removing the callback instead of pretending to secure it with a superficial attribute.

Removed end-to-end:

- POST `api/Payment/notification`
- `IPaymentService.HandlePaymentNotificationAsync`
- `PaymentService.HandlePaymentNotificationAsync`
- `IPaymentRepository.UpdatePaymentStatusAsync`
- `PaymentRepository.UpdatePaymentStatusAsync`

Preserved:

- authenticated POST `api/Payment/create-payment`
- Mercado Pago preference creation
- JWT user identity authority
- local pending Payment creation

## Authorization result

After B40, `PaymentController` exposes zero anonymous actions.

A public payment callback may return only as a separate integration with:

- authenticity verification;
- provider-owned resource lookup;
- canonical state resolution;
- idempotent update behavior.

## Permanent proof

Added:

- `PaymentNotificationSurfaceTests.cs`
- `scripts/backend_dead_payment_notification_authority.py`

The runtime tests prove absence of:

- `PaymentController.ReceiveNotification`
- `IPaymentService.HandlePaymentNotificationAsync`
- `IPaymentRepository.UpdatePaymentStatusAsync`

The source gate additionally requires the legacy route and implementation markers to remain absent while preserving preference creation.

## Validation

Final B40 implementation head:

`c58aab494519d4428a8d44fce97b09af936d0562`

PR #30:

- Quality `35615586713` — success
- Security `35615586569` — success

Published exact SHA:

- Quality `35615746568` — success
- Security `35615746479` — success
- frontend: 60/60
- backend: 75/75
- Release compiler warnings: 0
- npm audit: 0
- production npm audit: 0
- NuGet vulnerability audit: clean

Observed permanent gates:

- `product-category-default-policy=Admin`
- `payment-controller-default-policy=authenticated`
- `payment-anonymous-action-count=0`
- `legacy-payment-notification-route=absent`
- `legacy-payment-status-query-mutation=absent`
- `backend-dead-payment-notification-authority=clean`

## Result

The maintained payment API has one externally reachable operation: authenticated preference creation.

There is no public status-mutation callback until a real Mercado Pago webhook authority is implemented.
