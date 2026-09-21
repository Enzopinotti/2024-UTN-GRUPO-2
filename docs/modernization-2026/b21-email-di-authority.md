# B21 — Email dependency-injection authority

## Why this block exists

The post-B20 audit found that `Program.cs` registered the same service twice:

- `AddScoped<IEmailSender, EmailSender>()`
- later, `AddTransient<IEmailSender, EmailSender>()`

This was not treated as harmless formatting debt. In the built-in Microsoft
dependency-injection container, a normal single-service resolution uses the
last registration, while enumerable resolution exposes all registrations.

Therefore deleting the later registration and keeping the earlier Scoped
registration would have changed the effective lifetime.

## Starting authority

Carrier before B21:

`76ae94082bd4337159722d18f53d6bca158e5b9d`

B20 closure gates were green before branching.

Production consumers use direct `IEmailSender` injection in the maintained
controllers. B21 also adds a permanent source authority gate that rejects a
future production dependency on `IEnumerable<IEmailSender>` or
`GetServices<IEmailSender>()` unless the DI boundary is deliberately reviewed.

## Preserved behavior

B21 canonicalizes the registration to exactly one:

`AddTransient<IEmailSender, EmailSender>()`

It keeps the registration alongside the other application service interfaces
and removes the later duplicate.

This preserves the historical direct-resolution lifetime because the duplicate
configuration previously resolved `IEmailSender` through its last,
Transient registration.

The implementation type remains `EmailSender`. Email transport, SMTP
configuration, controller behavior and message content are unchanged.

## Behavior contracts

`EmailServiceRegistrationTests` adds two deterministic contracts:

1. the historical duplicate Scoped + Transient registration resolves a direct
   `IEmailSender` through the last Transient registration and exposes two
   implementations through enumerable resolution;
2. the canonical single Transient registration preserves the same direct
   resolution lifetime while reducing enumerable resolution to one maintained
   implementation.

No SMTP connection or live email is used by these tests.

## Permanent authority

`scripts/backend_di_authority.py` is now part of Quality.

It requires:

- exactly one `IEmailSender` registration in `Program.cs`;
- lifetime `Transient`;
- implementation `EmailSender`;
- zero production consumers of multiple `IEmailSender` registrations.

This keeps a future duplicate registration from silently becoming part of
application behavior.

## Lab evidence

Implementation head:

`247a85f33dedf8e71d1c721ff63cad09975b6eb8`

Validation:

- Quality `35557854358` — success
- Current-tree security `35557854346` — success
- `email-sender-registration-count=1`
- `email-sender-registration=Transient:EmailSender`
- `email-sender-multi-consumer-count=0`
- `backend-email-di-authority=clean`
- frontend maintained suite: 60/60
- backend maintained suite: 24/24
- backend Release build: 0 warnings
- npm audit: 0 findings
- NuGet vulnerability audit: clean
- repository clean-tree gate: success

## Explicit non-goals

B21 does not:

- change SMTP credentials/configuration;
- send live email in CI;
- redesign email delivery;
- change controller routes or response contracts;
- change `EmailSender` implementation behavior;
- change service lifetime from the historical effective lifetime;
- introduce an email queue/background worker;
- change any frontend code or dependency version.

## Rollback boundary

B21 is source-only and reversible. Restoring the two historical registrations
would restore the previous duplicate DI collection without schema, data or
external-system migration.

Temporary lab workflow triggers are removed before carrier promotion. Final
closure requires Quality and Current-tree security to pass again on the exact
promoted carrier SHA.


## Carrier promotion evidence

The cleaned B21 tree was promoted by non-forced fast-forward to the maintained
carrier.

Promoted carrier SHA:

`4cc63beab22ad90c21f03eeed69f3e042c1733ab`

Exact-SHA carrier validation:

- Quality `35557964888` — success
- Current-tree security `35557964871` — success
- email DI authority: clean
- frontend tests: 60/60
- backend tests: 24/24
- frontend build: success
- backend Release build: 0 warnings
- npm audit: 0 findings
- NuGet vulnerability audit: clean

A documentation-only closure commit follows this evidence. B21 is considered
closed only after permanent Quality and Current-tree security pass again on the
exact closure SHA.


## Carrier promotion evidence

The cleaned B21 tree was promoted by non-forced fast-forward to the maintained
carrier.

Promoted carrier SHA:

`4cc63beab22ad90c21f03eeed69f3e042c1733ab`

Exact-SHA carrier validation:

- Quality `35557964888` — success
- Current-tree security `35557964871` — success
- `email-sender-registration-count=1`
- `email-sender-registration=Transient:EmailSender`
- `email-sender-multi-consumer-count=0`
- `backend-email-di-authority=clean`
- frontend tests: 60/60
- backend tests: 24/24
- backend Release build: 0 warnings
- npm audit: 0 findings
- NuGet vulnerability audit: clean
- GitHub Actions runtime authority: Node 24

A documentation-only closure commit follows this evidence. B21 is considered
closed only after permanent Quality and Current-tree security pass again on that
exact closure SHA.
