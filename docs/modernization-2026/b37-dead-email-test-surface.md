# B37 — Dead email test surface removal

## Goal

B37 removes a legacy test-only HTTP surface that could trigger a real SMTP send from a public GET endpoint.

The maintained product still needs email for:

- account confirmation;
- password reset.

B37 therefore removes only the dead test surface and preserves the production email infrastructure.

## Starting checkpoint

Main before B37:

`cc38da1dfa6d993212f71a84d17d8195d6ef70ed`

B36 closure:

- frontend: 60/60
- backend: 64/64
- Release compiler warnings: 0
- npm audit: clean
- NuGet vulnerability audit: clean
- favorite concurrency integrity: enforced
- direct AppDbContext outside repositories: 0

## Characterization

The repository contained:

`Backend/antigal.server/Controllers/EmailController .cs`

The controller exposed a parameterless GET action named `SendTestEmail`.

That action:

- constructed a fixed test message;
- targeted a hardcoded address;
- called the real `IEmailSender`;
- had no authentication or authorization attribute;
- had no maintained frontend or backend consumer found by repository search.

The repository also contained:

`Backend/antigal.server/Models/Dto/TestEmailDto.cs`

Repository search found no consumer of that DTO other than its own definition.

## Production email authority preserved

`AccountController` remains the maintained consumer of `IEmailSender`.

Its two product email flows remain:

1. account email confirmation;
2. password reset.

Both continue to use:

`await _emailSender.SendEmailAsync(message)`

The following infrastructure remains unchanged:

- `IEmailSender`;
- `EmailSender`;
- email configuration;
- canonical `AddTransient<IEmailSender, EmailSender>()` registration;
- SMTP implementation;
- account confirmation/reset tokens.

B37 performs no live email call in CI.

## Removed

B37 deletes:

- `Backend/antigal.server/Controllers/EmailController .cs`
- `Backend/antigal.server/Models/Dto/TestEmailDto.cs`

The unusual historical filename with a space before `.cs` disappears with the removed controller.

## Permanent gate

Added:

`scripts/backend_dead_email_test_surface_authority.py`

It requires:

- the legacy EmailController file to remain absent;
- TestEmailDto to remain absent;
- AccountController to retain IEmailSender;
- account confirmation email markers to remain present;
- password-reset email markers to remain present;
- canonical email DI to remain present;
- EmailSender/IEmailSender contracts to remain present;
- legacy test-email markers to remain absent from production server source.

Expected output includes:

- `email-test-controller=absent`
- `test-email-dto=absent`
- `account-email-confirmation-authority=preserved`
- `account-password-reset-email-authority=preserved`
- `email-sender-di-authority=preserved`
- `live-test-email-endpoint-authority=absent`
- `backend-dead-email-test-surface-authority=clean`

## Runtime tests

Added:

`DeadEmailTestSurfaceTests.cs`

The tests prove:

1. the production assembly no longer exposes the legacy EmailController type;
2. the production assembly no longer exposes TestEmailDto;
3. AccountController still has IEmailSender in its maintained constructor dependency surface.

Backend maintained test count increases from 64 to 66.

## Validation

PR #26 head:

`3c809216691329ee72a496aef8a9a4f693ce022d`

PR validation:

- Quality `35611950082` — success
- Current-tree security `35611949954` — success
- frontend: 60/60
- backend: 66/66
- Release compiler warnings: 0
- npm audit: 0 findings
- production npm audit: 0 findings
- NuGet vulnerability audit: clean

B37 was promoted by non-forced fast-forward.

Published main SHA:

`3c809216691329ee72a496aef8a9a4f693ce022d`

Exact published-SHA validation:

- Quality `35612120570` — success
- Current-tree security `35612120466` — success

## Explicit non-goals

B37 does not:

- remove email confirmation;
- remove password reset;
- remove IEmailSender or EmailSender;
- change SMTP configuration;
- change email templates/content in account flows;
- add a replacement test-email HTTP endpoint;
- perform live SMTP validation.

## Result

A public, side-effecting test endpoint is no longer part of the maintained API surface.

The product email boundary is narrower and explicit: SMTP is reachable through maintained account workflows, not through a generic test controller.
