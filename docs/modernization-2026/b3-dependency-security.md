# B3 — Dependency-security reduction

B3 starts only after B0–B2 established collaborative provenance, reproducible
builds and 21 deterministic tests.

The goal is to reduce known dependency risk without turning the 2024 academic
project into a framework rewrite.

## B3a — Mail dependency cleanup

### Baseline

B2 Quality still reported known NuGet advisories in the server dependency graph,
including direct MailKit/MimeKit references.

A dedicated B3 lab showed that merely moving direct MailKit/MimeKit to 4.18.0
was not sufficient because the historical `NETCore.MailKit 2.1.0` dependency
still pulled an older MailKit line and legacy framework packages into
`EmailService`.

Repository search proved:

- application code uses `MailKit.Net.Smtp`, `MailKit.Security` and
  `MimeKit` directly;
- no application code uses `NETCore.MailKit`, `AddMailKit` or an
  `IMailKit` abstraction;
- `EmailService` source does not use ASP.NET Identity or Identity EF packages;
- the main server project does not directly use MailKit/MimeKit APIs; it consumes
  the `EmailService` project.

### Maintained composition

B3a therefore:

- removes unused `NETCore.MailKit 2.1.0`;
- removes unused Identity/Identity EF references from `EmailService`;
- removes redundant direct MailKit/MimeKit references from the server project;
- keeps mail implementation in `EmailService`;
- pins `MailKit 4.18.0` and `MimeKit 4.18.0` directly in that project.

No email behavior code changes.

### Lab evidence

Isolated lab branch:

`modernize/b3-dependency-security-lab`

Validated candidate:

`1ceab252084695ac5b4a66093f1d81fd33139871`

Workflow run:

`35417126195` — success.

Evidence:

- frontend: 13/13 tests green;
- frontend production build green;
- backend: 8/8 tests green;
- backend release build green;
- current-tree security baseline green;
- `EmailService` vulnerability report: no vulnerable packages from current
  NuGet sources;
- historical MailKit advisory absent;
- historical MimeKit advisory absent;
- `NETCore.MailKit` absent.

Artifact:

- id: `10575604067`;
- digest:
  `sha256:ef2df7b41fb09f840477dc9ac81244e0fa870c596e84358c9bd9688825ccd1ea`.

### Remaining backend debt

B3a deliberately does not claim the server dependency graph is clean.

Known remaining report items include:

- AutoMapper 13.0.1;
- Azure.Identity;
- Microsoft.Build;
- Microsoft.Identity.Client;
- NuGet.Packaging / NuGet.Protocol;
- SixLabors.ImageSharp;
- System.Text.Json.

Those are handled as separate evidence-driven sub-blocks.

## AutoMapper boundary

The current application uses AutoMapper in one registration path only:
`UserForRegistrationDto -> User`, including `UserName = Email`.

Patched AutoMapper lines introduce a newer licensing model. B3 will therefore
test removing this small dependency through explicit mapping rather than
blindly upgrading the package.

That change must preserve registration mapping semantics and pass all existing
tests plus a new mapping contract before promotion.

## B3b — Remove AutoMapper from the registration path

### Why removal instead of upgrade

The application used AutoMapper only for one registration mapping:

`UserForRegistrationDto -> User`.

That mapping copied:

- `FirstName`;
- `LastName`;
- `Email`;
- `UserName = Email`.

AutoMapper 13.0.1 is affected by `GHSA-rvv3-g6hj-g44x`. Patched modern
AutoMapper lines also use a newer licensing model, so the smallest maintenance
change for this academic repository is to remove the dependency rather than
introduce a new licensing/configuration obligation.

### Maintained replacement

B3b:

- removes the AutoMapper package;
- removes `MappingProfile`;
- removes AutoMapper DI registration and constructor injection;
- adds a small explicit `UserRegistrationMapper`;
- preserves the four historical registration mapping semantics;
- adds two backend tests for field preservation and null input;
- removes tracked Visual Studio `*.pubxml.user` state and permanently ignores
  that local publish-user file class.

No authentication, password, email-confirmation or role behavior is changed.

### Lab evidence

Validated candidate:

`57b5d705137e32798f0a8f38822e80dd8e5dd5b5`

Workflow:

`35418469188` — success.

Evidence:

- frontend: 13/13 tests green;
- frontend production build green;
- backend: 10/10 tests green;
- backend release build green;
- current-tree security baseline green;
- AutoMapper project reference count: 0;
- AutoMapper advisory `GHSA-rvv3-g6hj-g44x`: absent.

Artifact:

- id: `10576423934`;
- digest:
  `sha256:eb7cab4110ca4252f3721c9e5485804bb50d1365988edcb0a661c45f516db594`.

Permanent Quality now prevents both the dependency and advisory from returning.

## Remaining backend dependency debt

After B3a/B3b, the next investigation is the remaining transitive/runtime tool
surface, including NPOI/ImageSharp and old Microsoft design-time dependencies.

A separate .NET 10 lab has proven one possible clean graph, but B3 does not
promote that runtime migration merely because it is green. The next lab must
first determine whether the same advisory cleanup is achievable while
preserving the current `net8.0` application target.

## Frontend boundary

The frontend still reports the historical CRA dependency debt measured in B0/B2.

B3a does not migrate CRA, React Router or the build system. Frontend dependency
maintenance remains a separate block after backend dependency provenance is
understood.
