# B2 — Behavior contracts

B2 adds the first meaningful automated tests to the reconstructed collaborative
application.

## Frontend

The original CRA starter test is removed because it still asserted the
placeholder text `learn react` and no longer represented this application.

Thirteen deterministic Jest contracts now cover existing pure behavior:

- category local-storage read/write/add/update/delete;
- product local-storage read/write/add/update/delete;
- shared text formatting and connector-word casing.

These tests require no network, payment provider or backend.

## Backend

A dedicated `antigal.server.Tests` MSTest project is added to the existing
solution.

Eight initial contracts cover behavior that does not require SQL Server or
external providers:

- valid and invalid category validation;
- category name/description boundaries;
- valid product validation;
- required brand;
- positive price;
- non-negative stock boundary;
- `verificarDisponible()` stock semantics.

The test project references the historical application project directly and
does not duplicate product models.

## Deliberate exclusions

B2 does not yet execute:

- real SQL Server;
- Mercado Pago;
- Cloudinary;
- SMTP;
- destructive order/payment integration flows.

Those require controlled seams or disposable infrastructure before they can be
safe CI contracts.

## Quality gate

Permanent Quality now requires both frontend and backend tests before builds,
security checks and evidence capture.
