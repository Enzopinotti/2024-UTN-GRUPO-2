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
- `NPOI 2.7.6` remains intentionally pinned.

EmailService project:

- `MimeKit 4.18.0 → 4.18.1`.

MailKit remains on `4.18.0` because it is already on its current direct release line and accepts MimeKit 4.18.x.

## Why these packages are grouped

The promoted changes are deliberately conservative:

- CloudinaryDotNet remains within the 1.x line;
- MimeKit is a patch update within 4.18.x.

The packages compile against the existing net10.0 carrier without requiring application source rewrites.

MimeKit 4.18.1 also contains a parser hardening fix for integer-overflow handling in corrupt TNEF content.

### NPOI 2.8.0 rejection

The first B15 candidate also tested `NPOI 2.8.0`. The laboratory rejected it.

Observed evidence:

- NPOI 2.8.0 emits a build-time warning requiring explicit acceptance of the OSMF EULA;
- the 2.8.0 release introduced an Open Source Maintenance Fee EULA for binary users that generate revenue;
- the candidate dependency graph resolved `Microsoft.Build.Tasks.Git 8.0.0`;
- NuGet reported that transitive package with moderate vulnerability `GHSA-23fw-v26w-5fgq`;
- the B15 vulnerability gate failed before promotion.

B15 therefore keeps `NPOI 2.7.6` as the validated pre-EULA authority. No EULA acceptance flag, vulnerability suppression, or forced transitive override is introduced.

Any future NPOI migration requires a separate licensing and dependency-security decision.

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
- NPOI 2.7.6 (intentionally held);
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
- `dotnet package list --outdated` no longer reports CloudinaryDotNet or MimeKit;
- NPOI remains exactly 2.7.6 and the dependency graph remains vulnerability-clean;
- current-tree security remains green;
- validation leaves the repository clean.

## Scope boundary

B15 does not:

- cross a major-version boundary for any promoted package;
- alter application source code;
- change target frameworks;
- change API or persistence behavior;
- remove the crypto security override without explicit dependency-graph evidence;
- upgrade the deferred major packages listed above.

## Validated laboratory evidence

Final B15 laboratory run:

`35547803582` — success on `20dcaa9fbd2cd83d8cd123e0fd514aa3b702a18a`.

Validated signals:

- CloudinaryDotNet 1.29.3 restored and current;
- MimeKit 4.18.1 restored and current;
- NPOI 2.7.6 retained intentionally;
- frontend authorities, lint, tests and Vite build green;
- backend restore green through SLNX;
- backend release build green;
- backend tests: 16 / 16 green;
- noun-first `dotnet package list` vulnerability audit clean;
- targeted outdated packages: 0 for CloudinaryDotNet and MimeKit;
- current-tree security baseline green;
- repository clean after validation.

Rejected-candidate evidence is preserved from run `35547683790`, where NPOI 2.8.0 introduced the OSMF EULA warning and a vulnerable `Microsoft.Build.Tasks.Git 8.0.0` transitive dependency.

## Closure condition

B15 is ready for carrier promotion only after the validated permanent files are reconciled onto the current carrier, the temporary B15 workflow is retired, and carrier Quality plus current-tree security pass after promotion.
