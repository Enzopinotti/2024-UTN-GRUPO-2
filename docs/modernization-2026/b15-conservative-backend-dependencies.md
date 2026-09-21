# B15 — Conservative backend dependency refresh

B15 starts from the completed B14 carrier:

`4b435d1d534a8758c476eee87c02c3c23ee58356` — `B14: record carrier validation and closure`.

The B15 laboratory is developed on:

`modernize/b15-backend-dependencies-lab`.

## Objective

Refresh a deliberately small set of non-major backend dependencies while preserving the validated .NET 10, SLNX, frontend, backend and security authorities.

B15 targets only packages for which the existing direct dependency can move to the current stable release without crossing a major-version boundary.

## Package changes

Server project:

- `CloudinaryDotNet 1.26.2 → 1.29.3`;
- `NPOI 2.7.6 → 2.8.0`.

EmailService project:

- `MimeKit 4.18.0 → 4.18.1`.

MailKit remains on `4.18.0` because it is already on its current direct release line and accepts MimeKit 4.18.x.

## Why these packages are grouped

These three changes are deliberately conservative:

- CloudinaryDotNet remains within the 1.x line;
- NPOI remains within the 2.x line;
- MimeKit is a patch update within 4.18.x.

The packages compile against the existing net10.0 carrier without requiring application source rewrites.

MimeKit 4.18.1 also contains a parser hardening fix for integer-overflow handling in corrupt TNEF content.

## Deferred major upgrades

B15 explicitly does not upgrade:

- `FluentValidation.DependencyInjectionExtensions 11.10.0 → 12.x`;
- `mercadopago-sdk 2.4.1 → 3.x`;
- `SharpGrip.FluentValidation.AutoValidation.Mvc 1.4.0 → 2.x`;
- `SixLabors.ImageSharp 2.1.13 → 4.x`;
- `Swashbuckle.AspNetCore 6.9.0 → 10.x`.

Those are breaking-major migrations and require separate compatibility blocks with focused source/API validation.

## NuGet CLI modernization

Because the carrier is already pinned to .NET SDK 10.0.401, B15 moves the permanent vulnerability audit from the historical verb-first form:

`dotnet list <solution> package`

to the .NET 10 noun-first form:

`dotnet package list --project <solution>`.

Permanent Quality uses `--no-restore` because its restore step is already explicit.

## Permanent Quality boundary

Quality must retain:

- CloudinaryDotNet 1.29.3;
- NPOI 2.8.0;
- MimeKit 4.18.1;
- MailKit 4.18.0;
- the explicit System.Security.Cryptography.Xml 10.0.12 security override;
- noun-first `dotnet package list` vulnerability auditing;
- net10.0 / SDK 10.0.401;
- SLNX solution authority;
- warning-free backend compilation;
- 16 / 16 backend tests;
- clean NuGet vulnerability reports.

All B14 and earlier frontend/runtime/security gates remain active.

## Laboratory validation

B15 must prove:

- the three new direct versions are restored exactly;
- frontend authorities, lint, tests and Vite build remain green;
- backend restore succeeds through the SLNX solution;
- release build remains warning-free;
- backend tests remain 16 / 16 green;
- `dotnet package list --vulnerable --include-transitive` is clean;
- `dotnet package list --outdated` no longer reports CloudinaryDotNet, NPOI or MimeKit;
- current-tree security remains green;
- validation leaves the repository clean.

## Scope boundary

B15 does not:

- cross a major-version boundary for any targeted package;
- alter application source code;
- change target frameworks;
- change API or persistence behavior;
- remove the crypto security override without explicit dependency-graph evidence;
- upgrade the deferred major packages listed above.

## Closure condition

B15 is ready for carrier promotion only after the laboratory is fully green, permanent Quality owns the refreshed pins and noun-first audit command, the temporary B15 workflow is retired, and carrier Quality plus current-tree security pass after promotion.
