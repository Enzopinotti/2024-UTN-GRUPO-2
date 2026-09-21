# B22 — JWT expiration configuration authority

## Why this block exists

The post-B21 audit initially considered the conservative direct dependency
refresh `System.IdentityModel.Tokens.Jwt 8.19.2 → 8.23.0`.

Before changing the package, the maintained JWT creation path was inspected.
`JwtHandler` reads `JWTSettings:expiryInMinutes`, but neither the historical
`main` appsettings nor the maintained 2026 configuration declared that key.

The package refresh was therefore deferred. Configuration correctness took
priority over version freshness.

## Historical behavior characterization

B22 first validated the existing behavior without changing product code.

Characterization commit:

`3693d7f624fcc883cd221ac2f6ce07d5bccb58e7`

Runs:

- Quality `35558201271` — success
- Current-tree security `35558201217` — success
- backend tests: 25/25

The focused contract constructed the real `JwtHandler` without
`expiryInMinutes` and proved that the generated token had at most five seconds
of remaining lifetime. This characterizes the historical missing-value path as
effectively immediate expiration.

The defect also exists on historical `main`; it was not introduced by the 2026
security/configuration cleanup.

## Maintained behavior

B22 establishes a non-secret default:

`JWTSettings:expiryInMinutes = 60`

The equivalent environment-variable override is:

`JWTSettings__expiryInMinutes`

`JwtHandler` now requires the configured value to parse as a positive integer.
Missing, zero, negative or otherwise invalid values fail with an explicit
configuration error rather than silently producing an effectively expired JWT.

Token expiration is generated from UTC time with the configured positive
duration.

## Behavior contracts

`JwtExpirationConfigurationTests` validates:

1. `60` produces approximately one hour of future token lifetime;
2. missing expiration configuration is rejected explicitly;
3. a non-positive expiration is rejected.

No real credentials, HTTP requests or external auth provider are used.

The maintained backend suite increases from 24 to 27 tests.

## Permanent authority

`scripts/jwt_configuration_authority.py` is part of Quality.

It requires:

- tracked `JWTSettings.expiryInMinutes` to be the positive integer `60`;
- `CONFIGURATION.md` to document `JWTSettings__expiryInMinutes`;
- `JwtHandler` to keep positive-integer validation;
- UTC-based expiration from the validated configured value.

## Implementation evidence

Implementation commit:

`3d5ee463555e31357c80fd1879eb57055d56a7c4`

Runs:

- Quality `35558348287` — success
- Current-tree security `35558348264` — success
- `jwt-expiry-default-minutes=60`
- `jwt-expiry-validation=positive-integer`
- `jwt-expiry-configuration-authority=clean`
- frontend maintained suite: 60/60
- backend maintained suite: 27/27
- backend Release build: 0 warnings
- npm audit: 0 findings
- NuGet vulnerability audit: clean
- repository clean-tree gate: success

## Package boundary

B22 deliberately keeps:

`System.IdentityModel.Tokens.Jwt 8.19.2`

The measured `8.23.0` refresh remains a separate candidate after the JWT
configuration contract is promoted and closed. This keeps a behavior fix and a
dependency change independently attributable and reversible.

## Explicit non-goals

B22 does not:

- change signing algorithms;
- change issuer or audience;
- rotate or expose JWT signing keys;
- redesign authentication or refresh-token flows;
- change authorization roles/claims;
- change `ServiceToken`;
- upgrade the JWT package;
- change frontend code.

## Rollback boundary

B22 changes only JWT expiration configuration/validation, tests, documentation
and its permanent Quality authority. There is no database or external-system
migration.

Temporary lab workflow triggers are removed before carrier promotion. Final
closure requires Quality and Current-tree security to pass on the exact promoted
carrier SHA.


## Carrier promotion evidence

The cleaned B22 tree was promoted by non-forced fast-forward to the maintained
carrier.

Promoted carrier SHA:

`2d56ce172a37eab95d9b67eb010a7028738b2b8e`

Exact-SHA carrier validation:

- Quality `35558478411` — success
- Current-tree security `35558478422` — success
- JWT expiration authority: clean
- frontend tests: 60/60
- backend tests: 27/27
- frontend build: success
- backend Release build: 0 warnings
- npm audit: 0 findings
- NuGet vulnerability audit: clean

A documentation-only closure commit follows this evidence. B22 is considered
closed only after permanent Quality and Current-tree security pass again on the
exact closure SHA.
