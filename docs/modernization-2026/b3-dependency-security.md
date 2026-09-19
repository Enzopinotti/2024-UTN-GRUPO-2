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

## Frontend boundary

The frontend still reports the historical CRA dependency debt measured in B0/B2.

B3a does not migrate CRA, React Router or the build system. Frontend dependency
maintenance remains a separate block after backend dependency provenance is
understood.
