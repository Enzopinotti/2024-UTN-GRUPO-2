# B39 — Payment authorization boundary

## Goal

Require authenticated identity before creating a Mercado Pago preference while preserving the then-existing external notification callback for separate analysis.

## Characterization

`PaymentService.CreatePaymentPreferenceAsync` already depended on:

`HttpContext.User.Identity.Name`

and rejected a missing identity.

JWT configuration maps:

`NameClaimType = JwtRegisteredClaimNames.Sub`

while `JwtHandler` emits:

`sub = user.Id`

Therefore the maintained payment user-id authority is the authenticated user's id.

## Maintained policy

B39 made `PaymentController` authenticated by default.

At the B39 checkpoint:

- POST `create-payment`: authenticated
- POST `notification`: explicitly anonymous

The webhook was intentionally not declared safe in B39. Authenticity verification remained a separate security concern.

## Permanent proof

Added:

- `PaymentAuthorizationTests.cs`
- `scripts/backend_payment_authorization_authority.py`

The gate protects:

- controller authentication;
- payment preference route;
- JWT `sub` -> Identity.Name authority;
- absence of anonymous access on preference creation.

B40 subsequently tightened this same gate to require zero anonymous payment actions after retiring the legacy callback.

## Validation

Final B39 head:

`030b10a8cd95d86cc457bc19652dd0d0ffe800a8`

PR #29:

- Quality `35614876634` — success
- Security `35614876505` — success

Published SHA validation:

- Quality `35615075350` — success
- Security `35615075532` — success

## Result

Payment preference creation is no longer publicly reachable and its user identity is bound to the authenticated JWT subject.
