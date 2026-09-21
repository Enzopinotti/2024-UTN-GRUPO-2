# B18 — Mercado Pago SDK 3.7.0

B18 starts from the completed B17 carrier:

`22a8ccfc25d81c9ccbfe1ea5aac36fa223a9fac7` — `B17: record carrier validation and closure`.

The B18 laboratory is developed on:

`modernize/b18-mercadopago3-lab`.

## Objective

Move the backend Mercado Pago integration from `mercadopago-sdk 2.4.1` to the current stable `3.7.0` release while preserving the existing Checkout Pro flow and avoiding unrelated payment-domain redesign.

The carrier already targets .NET 10.0 with SDK 10.0.401. Mercado Pago SDK 3.x targets .NET 8 or newer, so the carrier is inside the supported runtime boundary.

The 3.x line raised the target framework to .NET 8 and addressed dependency-vulnerability debt. B18 validates the major upgrade against the exact integration surface used by Antigal instead of treating the package update as a blind version bump.

## Maintained Mercado Pago surface

The application currently uses Mercado Pago in two places.

### Program configuration

`Backend/antigal.server/Program.cs` retains:

```csharp
using MercadoPago.Config;
MercadoPagoConfig.AccessToken = mercadoPagoAccessToken;
```

B18 does not change the existing configuration source or secret boundary.

### Checkout Pro preference creation

`Backend/antigal.server/Services/PaymentService.cs` retains:

- `PreferenceRequest`;
- `PreferenceItemRequest`;
- `PreferenceBackUrlsRequest`;
- `PreferenceClient`;
- `PreferenceClient.CreateAsync(request)`;
- `Preference.Id`;
- `Preference.InitPoint`.

No production code change was required for those SDK APIs after upgrading to 3.7.0.

## Compatibility contracts

B18 adds `Backend/antigal.server.Tests/MercadoPagoSdkCompatibilityTests.cs`.

The tests are intentionally offline and protect the SDK surface without making network calls.

They verify:

- Checkout Pro preference request construction;
- item title, quantity, currency and unit price;
- success/failure/pending back URLs;
- `AutoReturn`;
- `PreferenceClient` construction;
- `MercadoPagoConfig.AccessToken` read/write compatibility.

These contracts complement the existing application tests and provide an early compile/runtime signal if a future SDK upgrade changes the APIs Antigal consumes.

## First laboratory failure and correction

Initial B18 run:

`35550012705` — failed during compilation of the newly introduced compatibility test.

The application project itself compiled successfully with Mercado Pago SDK 3.7.0. The failure was caused only by the test harness initially using xUnit attributes in a repository whose backend test project uses MSTest:

- `CS0246: Xunit could not be found`;
- `CS0246: Fact / FactAttribute could not be found`.

The compatibility tests were converted to the repository's maintained MSTest framework.

No production Mercado Pago code was changed to resolve the failure.

## Validated laboratory evidence

Final B18 laboratory run:

`35550090358` — success on `077ff5816378a67ff32840f42fa2c7731ef83283`.

Validated signals:

- Node 24.20.0 authority retained;
- .NET SDK 10.0.401 authority retained;
- `mercadopago-sdk 3.7.0` package authority present;
- production Checkout Pro integration markers retained;
- frontend dependency, JSX, dead-source and Sass authorities green;
- ESLint green with zero warnings;
- frontend tests: 60 / 60 green across 13 files;
- Vite 8.2.2 production build green, 460 modules transformed;
- backend restore green through `antigal.server.slnx`;
- backend release build: 0 warnings, 0 errors;
- backend tests: 20 / 20 green;
- `mercadopago-sdk 3.7.0` resolved as the top-level package;
- targeted Mercado Pago outdated check reports no newer stable version;
- NuGet vulnerability audit clean for server, tests and EmailService;
- current-tree security baseline green;
- repository remains clean after validation.

## Permanent Quality boundary

After promotion, Quality must retain:

- `mercadopago-sdk 3.7.0`;
- the B18 Mercado Pago package-authority evidence;
- the compatibility tests as part of the backend test suite;
- .NET 10 / SDK 10.0.401;
- SLNX solution authority;
- warning-free backend compilation;
- clean frontend and NuGet vulnerability reports;
- all B17 and earlier source, runtime, frontend and security gates.

## Scope boundary

B18 does not:

- redesign `PaymentService`;
- change payment persistence;
- alter payment controller routes;
- change webhook/IPN behavior;
- introduce live Mercado Pago API calls into CI;
- change frontend Mercado Pago behavior;
- change credentials or secrets;
- upgrade Swashbuckle;
- change NPOI;
- alter database schema.

## Closure

B18 is ready for promotion after the temporary laboratory workflow is retired and the carrier passes permanent Quality and current-tree security.
