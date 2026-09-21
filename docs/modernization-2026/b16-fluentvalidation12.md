# B16 — FluentValidation 12 and SharpGrip 2

B16 starts from the completed B15 carrier:

`5adc33ff63319873b3c8117d846fa28e5783fbb6` — `B15: record carrier validation and closure`.

The B16 laboratory is developed on:

`modernize/b16-fluentvalidation12-lab`.

## Objective

Move the backend validation stack across its deferred major-version boundary while preserving the already validated .NET 10, SLNX, frontend, backend and security authorities.

B16 upgrades only the validation integration pair:

- `FluentValidation.DependencyInjectionExtensions 11.10.0 → 12.1.1`;
- `SharpGrip.FluentValidation.AutoValidation.Mvc 1.4.0 → 2.0.0`.

No application endpoint, persistence or domain behavior is intentionally changed.

## Platform compatibility

The carrier is pinned to .NET SDK 10.0.401 and `net10.0`.

FluentValidation 12 requires .NET 8 or newer, so the carrier is within its supported platform boundary.

SharpGrip FluentValidation AutoValidation MVC 2.0.0 also targets .NET 8 or newer.

## FluentValidation 12 breaking-change boundary

The official FluentValidation 12 upgrade guide describes the release as primarily removing deprecated APIs and obsolete platform support.

B16 explicitly rejects these removed APIs from maintained backend source and tests:

- `Transform(...)`;
- `TransformForEach(...)`;
- `InjectValidator(...)`;
- `CascadeMode.StopOnFirstFailure`;
- `AbstractValidator.EnsureInstanceNotNull`;
- legacy test helpers `ShouldHaveAnyValidationError` and `ShouldNotHaveAnyValidationErrors`.

The maintained validators do not depend on those APIs.

## Validation registration contracts

The production registration remains:

```csharp
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();
```

B16 adds focused tests that protect both integration boundaries:

- assembly scanning resolves the maintained `Categoria` and `Producto` validators;
- SharpGrip MVC auto-validation contributes its services to the MVC service collection.

The assembly-registration test intentionally uses `ValidacionCategoria` as the assembly marker rather than coupling the test project to the application entry-point type. Both markers live in the same backend assembly, so the test protects FluentValidation assembly discovery without making the test depend on startup-type namespace resolution.

## First laboratory failure and correction

Initial B16 run:

`35548226031` — failed during test-project compilation.

The upgraded packages and main backend project compiled successfully. The failure came from the new test using `AddValidatorsFromAssemblyContaining<Program>()` without importing the `antigal.server` namespace:

`CS0246: The type or namespace name 'Program' could not be found`.

This was a test-harness issue, not a FluentValidation 12 or SharpGrip 2 compatibility failure.

The test was corrected to use `ValidacionCategoria` as the assembly marker.

## Validated laboratory evidence

Final B16 laboratory run:

`35549358575` — success on `1d51db1f83f89f2272e54e00bd0ef9ce9e95e61e`.

Validated signals:

- Node 24.20.0 authority retained;
- .NET SDK 10.0.401 authority retained;
- frontend dependency, JSX, dead-source and Sass authorities green;
- ESLint green with zero warnings;
- frontend tests: 60 / 60 green across 13 files;
- Vite 8.2.2 production build green, 460 modules transformed;
- backend restore green through `antigal.server.slnx`;
- backend release build warning-free;
- backend tests: 18 / 18 green;
- `FluentValidation.DependencyInjectionExtensions 12.1.1` resolved;
- `FluentValidation 12.1.1` resolved transitively;
- `SharpGrip.FluentValidation.AutoValidation.Mvc 2.0.0` resolved;
- NuGet vulnerability audit clean for server, tests and EmailService;
- targeted outdated check reports no newer versions for the two B16 packages;
- current-tree security baseline green;
- repository remains clean after validation.

## Permanent Quality boundary

After promotion, Quality must retain:

- FluentValidation DI 12.1.1;
- SharpGrip MVC auto-validation 2.0.0;
- rejection of removed FluentValidation 12 APIs;
- .NET 10 / SDK 10.0.401;
- SLNX solution authority;
- warning-free backend compilation;
- at least the maintained validator registration tests;
- clean frontend and NuGet vulnerability reports;
- all B15 and earlier frontend/runtime/security gates.

## Scope boundary

B16 does not:

- migrate controller behavior;
- introduce new validators;
- redesign validation responses;
- change model-state semantics intentionally;
- upgrade Mercado Pago, ImageSharp or Swashbuckle;
- change NPOI 2.7.6;
- alter database schema or persistence behavior.

Those remaining major dependencies require separate blocks.

## Closure

B16 is ready for promotion after the validated laboratory workflow is retired and the carrier passes its permanent Quality and current-tree security workflows.
