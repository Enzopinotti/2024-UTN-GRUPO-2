# B19 — Swashbuckle.AspNetCore 10.2.3

B19 starts from the completed B18 carrier:

`5380be00433eef95696b0e98715a23bc001ac78c` — `B18: upgrade Mercado Pago SDK to 3.7.0`.

The B19 laboratory is developed on:

`modernize/b19-swashbuckle10-lab`.

## Objective

Move the backend OpenAPI/Swagger stack from `Swashbuckle.AspNetCore 6.9.0` to
`10.2.3` while preserving the existing development-only Swagger UI and JWT
security contract.

The carrier already targets `net10.0` with .NET SDK 10.0.401. Swashbuckle
10.2.3 supports net10.0 and depends on the Microsoft.OpenApi 2.x surface used
by the .NET 10 OpenAPI ecosystem.

## Breaking-change boundary

Swashbuckle 10 moved to Microsoft.OpenApi 2.x.

The maintained Antigal configuration used the v1 namespace and reference
surface:

- `Microsoft.OpenApi.Models`;
- `OpenApiReference`;
- `OpenApiSecurityScheme.Reference`.

B19 migrates that configuration to:

- `using Microsoft.OpenApi;`;
- `OpenApiSecurityScheme`;
- `OpenApiSecuritySchemeReference`;
- the document-aware `AddSecurityRequirement(document => ...)` overload.

The existing JWT scheme remains an `ApiKey` named `Authorization` so B19
does not intentionally change Swagger UI token-entry semantics.

B19 does not switch the emitted document to OpenAPI 3.1. Swashbuckle 10 keeps
OpenAPI 3.0 as its default unless configured otherwise, so specification
versioning remains a separate decision.

## Compatibility contracts

B19 adds `SwaggerSdkCompatibilityTests.cs` to protect the Microsoft.OpenApi
2.x surface used by production:

- JWT security scheme construction;
- header location;
- ApiKey scheme type;
- Bearer scheme name;
- `OpenApiSecuritySchemeReference` construction;
- `OpenApiSecurityRequirement` dictionary compatibility.

The tests are offline and do not launch the application or require database,
JWT, email, Cloudinary or Mercado Pago credentials.

## First laboratory run

Initial run:

`35553755806`.

Functional validation passed:

- frontend source-authority gates;
- ESLint;
- frontend tests;
- Vite production build;
- frontend npm audit;
- backend restore through SLNX;
- backend warning-free release build;
- backend tests including the new B19 contracts;
- NuGet vulnerability audit;
- targeted Swashbuckle outdated check;
- current-tree security baseline.

The only failure was the final repository-cleanliness assertion. The B19
workflow had written its own audit evidence files into the repository checkout,
so `git status --porcelain` correctly reported those generated untracked
files.

This was a laboratory-harness defect, not a source or dependency failure.

The workflow now writes all B19 evidence under `RUNNER_TEMP`, matching the
permanent Quality pattern, so evidence generation no longer dirties the
checkout.

## Permanent Quality boundary after promotion

B19 will require:

- `Swashbuckle.AspNetCore 10.2.3`;
- no maintained `Microsoft.OpenApi.Models` usage;
- no legacy `OpenApiReference` usage;
- the document-aware `OpenApiSecuritySchemeReference` JWT requirement;
- B19 compatibility tests in the backend suite;
- clean NuGet vulnerability reporting;
- Swashbuckle not reported as outdated;
- all B18 and earlier frontend, backend, runtime, source-authority and security
  gates to remain green.

## Scope boundary

B19 does not:

- enable OpenAPI 3.1 output;
- redesign the JWT authentication scheme;
- expose Swagger outside the existing development environment;
- replace Swashbuckle with Microsoft.AspNetCore.OpenApi;
- change controllers or API routes;
- alter persistence, database schema, payment behavior or frontend behavior;
- change NPOI 2.7.6.

## Validated laboratory evidence

Final B19 laboratory run:

`35553882388` — success on
`022420691fcbedaf977f32886cf0ed6b9ada6adb`.

Validated signals:

- Swashbuckle.AspNetCore 10.2.3 authority present;
- Microsoft.OpenApi 2.x JWT security surface present;
- legacy `Microsoft.OpenApi.Models` and `OpenApiReference` usage absent;
- frontend source-authority gates green;
- ESLint green with zero warnings;
- frontend tests: 60 / 60 green across 13 files;
- Vite production build green;
- frontend npm audit: 0 vulnerabilities;
- backend restore green through the SLNX solution;
- backend release build: 0 warnings, 0 errors;
- backend tests: 22 / 22 green;
- NuGet vulnerability audit clean;
- targeted Swashbuckle outdated check clean;
- current-tree security baseline green;
- repository remains clean after validation.

The first run `35553755806` is retained as rejected harness evidence: all
functional/security checks passed, but the workflow wrote its own evidence
files into the checkout. That defect was corrected by moving evidence to
`RUNNER_TEMP`.

## Promotion readiness

Permanent Quality now protects:

- Swashbuckle.AspNetCore 10.2.3;
- Microsoft.OpenApi 2.x source authority;
- absence of legacy OpenAPI reference APIs;
- the B19 compatibility test file;
- every B18 and earlier authority gate.

The temporary B19 workflow can therefore be retired before promotion. Carrier
closure evidence will be appended after the promotion commit passes permanent
Quality and current-tree security.
