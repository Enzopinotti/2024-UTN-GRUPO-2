# B34 — Admin bootstrap authority

## Goal

B34 closes the production-safety debt around the historical database initializer without removing the role bootstrap required by the authorization model.

The historical initializer combined two concerns:

- ensuring the Admin/User/Visitor roles exist;
- creating an administrator from credentials embedded in source.

The second behavior is not an acceptable maintained production authority.

## Starting checkpoint

Main before B34:

`80c199e797411b26afd5bf18713d936a7241b482`

B33 validation:

- frontend: 60/60
- backend: 52/52
- Release compiler warnings: 0
- npm audit: clean
- NuGet vulnerability audit: clean
- controller-level AppDbContext consumers: 0

## Maintained design

### Role initialization

The authorization roles remain initialized when missing:

- Admin
- User
- Visitor

This preserves the existing Identity/authorization model.

### Administrator bootstrap

Added:

- `AdminBootstrapOptions`
- `AdminBootstrapPolicy`
- tracked `BootstrapAdmin` configuration section

Tracked configuration is intentionally:

- `Enabled = false`
- empty UserName
- empty Email
- empty Password

There are no maintained administrator credential defaults in source.

When bootstrap is disabled, no administrator user is created.

When bootstrap is enabled:

1. the host must be Development;
2. UserName, Email and Password must all come from external configuration;
3. incomplete configuration is rejected;
4. a non-Development attempt is rejected.

### Fail-closed startup

The previous initializer wrapper logged initialization exceptions and continued application startup.

That would have weakened the new safety policy because a rejected bootstrap configuration could be swallowed.

Program startup now awaits `DbInitializer.Initialize` directly. Initialization failure is fatal rather than allowing the API to continue with an uncertain authorization/bootstrap state.

## Security baseline hardening

The current-tree security baseline previously used first-match regular expressions over appsettings keys.

That is fragile when the same key name appears in multiple sections.

B34 changes the baseline to parse JSON structurally and validate exact configuration paths.

Tracked sensitive/configuration values checked as empty now include the BootstrapAdmin fields as well as the existing database/JWT/Cloudinary/email/Mercado Pago slots.

`BootstrapAdmin:Enabled` is separately required to remain false in tracked configuration.

## Permanent authority gate

Added:

`scripts/backend_admin_bootstrap_authority.py`

The gate requires:

- no embedded admin email/password assignment authority in DbInitializer;
- options-driven bootstrap;
- Development environment enforcement;
- explicit required-field validation;
- configuration registration in Program;
- configured values passed to Identity;
- tracked BootstrapAdmin defaults disabled/empty;
- fail-closed initializer startup.

Expected authority includes:

- `bootstrap-admin-default-enabled=false`
- `bootstrap-admin-environment-authority=Development-only`
- `bootstrap-admin-credential-authority=external-configuration`
- `bootstrap-admin-embedded-credentials=absent`
- `bootstrap-startup-failure-authority=fail-closed`
- `backend-admin-bootstrap-authority=clean`

## Tests

Added `AdminBootstrapPolicyTests`.

Five tests prove:

1. options default to disabled without credentials;
2. disabled bootstrap performs no bootstrap even outside Development;
3. enabled bootstrap is rejected outside Development;
4. incomplete enabled Development configuration is rejected;
5. complete enabled Development configuration is resolved exactly from supplied configuration.

Backend maintained test count increases from 52 to 57.

## Validation

PR #19 final head:

`7f975cefa93d64f01737100ddb5da751e655888d`

PR validation:

- Quality `35604776227` — success
- Current-tree security `35604776258` — success
- frontend: 60/60
- backend: 57/57
- Release compiler warnings: 0
- npm audit: 0 findings
- production npm audit: 0 findings
- NuGet vulnerability audit: clean
- `backend-admin-bootstrap-authority=clean`
- `bootstrap-admin-embedded-credentials=absent`

## Main promotion

B34 was promoted by non-forced fast-forward.

Published main SHA:

`7f975cefa93d64f01737100ddb5da751e655888d`

Exact published-SHA validation:

- Quality `35604941337` — success
- Current-tree security `35604941447` — success

## Explicit non-goals

B34 does not:

- remove role initialization;
- provision production administrators;
- introduce a secret store implementation;
- change Identity password policy;
- change JWT behavior;
- rotate or make claims about historical credentials;
- modify database schema;
- modify ImageService.

Operational production administrator provisioning remains an environment/deployment concern rather than tracked application defaults.

## Result

The repository no longer treats a source-embedded administrator as a maintained runtime assumption.

Admin bootstrap is now explicit, externally configured, Development-only, disabled in tracked configuration and fail-closed.
