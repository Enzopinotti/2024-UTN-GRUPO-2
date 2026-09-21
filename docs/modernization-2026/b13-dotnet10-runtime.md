# B13 — .NET 10 runtime authority

B13 starts from the completed B12 carrier:

`bcb9421763874b36b7fbf9a9b1699e5e3b0882f6` — `B12: record carrier validation and closure`.

The B13 laboratory is developed on:

`modernize/b13-dotnet10-lab`.

## Objective

Move the maintained backend solution from .NET 8 to .NET 10 while preserving all existing frontend, backend, security and behavior contracts.

B13 is a platform/runtime migration only. It does not redesign API behavior, alter persistence semantics, change business rules, or opportunistically upgrade unrelated third-party packages.

## Runtime authority

B13 pins the repository SDK through `global.json`:

- SDK: `10.0.401`;
- `rollForward: disable`;
- prerelease SDKs disabled.

All maintained .NET projects move from `net8.0` to `net10.0`:

- `Backend/antigal.server/antigal.server.csproj`;
- `Backend/EmailService/EmailService.csproj`;
- `Backend/antigal.server.Tests/antigal.server.Tests.csproj`.

The GitHub Actions laboratory and permanent Quality workflow both install the exact SDK `10.0.401`.

## Microsoft package alignment

The direct Microsoft runtime/data package family is aligned with the .NET 10 servicing line:

- `Microsoft.AspNetCore.Authentication.JwtBearer 10.0.12`;
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.12`;
- `Microsoft.EntityFrameworkCore 10.0.12`;
- `Microsoft.EntityFrameworkCore.SqlServer 10.0.12`;
- `Microsoft.EntityFrameworkCore.Tools 10.0.12`.

The migration keeps third-party package authority unchanged unless validation proves a compatibility or security requirement.

## Transitive crypto security finding

The initial .NET 10 probes exposed a vulnerable transitive floor for:

`System.Security.Cryptography.Xml`.

`dotnet nuget why` traced this dependency through the maintained graph. NPOI 2.7.6 permits an older `System.Security.Cryptography.Xml` floor, so B13 adds an intentional direct security override:

`System.Security.Cryptography.Xml 10.0.12`.

The reference carries `NoWarn="NU1510"` only for this explicit transitive security pin. The laboratory does not suppress vulnerability auditing.

## Validated laboratory evidence

Validated B13 run:

`35546703312` — success on `0a61171fa4906cf6601d09e9dc058c1b2d0066ba`.

Validated signals:

- Node 24.20.0 frontend authority preserved;
- .NET SDK 10.0.401;
- .NET runtime / ASP.NET Core runtime 10.0.12;
- all three maintained projects target `net10.0`;
- frontend production dependency authority clean;
- JSX extension authority clean;
- dead-source authority clean;
- native Sass authority clean;
- frontend lint green;
- frontend tests green;
- frontend Vite build green;
- .NET 10 restore green;
- .NET 10 build: 0 warnings / 0 errors;
- backend tests: 16 / 16 green;
- current-tree security baseline green;
- NuGet vulnerable-package audit: clean for server, EmailService and tests.

The `--outdated --include-transitive` report still lists older transitive packages from third-party dependencies. Those are evidence for future dependency-specific work, not justification for forcing indirect upgrades in B13.

## Permanent Quality boundary

B13 updates the permanent Quality workflow so the carrier must retain:

- exact SDK 10.0.401;
- `global.json` with roll-forward disabled;
- `net10.0` on server, EmailService and tests;
- Microsoft ASP.NET Core / EF Core direct package family on 10.0.12;
- `System.Security.Cryptography.Xml 10.0.12` explicit security override;
- warning-free backend compilation;
- 16 / 16 backend tests;
- clean NuGet vulnerability audit.

All B12 and earlier frontend/runtime/security gates remain active.

## Scope boundary

B13 does not:

- replace the solution format;
- redesign controllers or endpoints;
- change database migrations;
- rewrite Identity/authentication flows;
- update NPOI, Cloudinary, Mercado Pago, Swashbuckle, FluentValidation or other third-party packages unless required for .NET 10 compatibility;
- force transitive packages merely because `dotnet list package --outdated --include-transitive` reports newer versions.

Those are separate future blocks with their own compatibility evidence.

## Closure

B13 was promoted to the maintenance carrier through PR #10 as:

`30ff876869665a65352f47864dd894d9314824d7` — `B13: adopt .NET 10 runtime authority`.

The temporary B13 workflow was retired before promotion.

Final carrier validation:

- Quality run `35547104368`: success;
- Current-tree security baseline run `35547104557`: success;
- .NET SDK authority: 10.0.401;
- .NET target framework: net10.0;
- backend compiler warnings: 0;
- backend tests: 16 / 16 green;
- NuGet vulnerable-package audit: clean;
- all B12 and earlier frontend/runtime/security gates remained green.

B13 is therefore closed on the carrier.
