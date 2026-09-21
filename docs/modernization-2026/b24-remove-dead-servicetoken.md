# B24 — Remove dead ServiceToken authority

## Why this block exists

After B23, the backend still contained a second token-generation service:
`Services/ServiceToken.cs`.

Unlike the maintained `JwtHandler`, this class used a separate configuration
shape:

- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`

The maintained runtime configuration and authentication path use
`JWTSettings:*`.

B24 did not assume the older class was dead. It characterized the exact carrier
before changing product source.

## Starting authority

Carrier before B24:

`9d42d32bbf94dca485aaec10b840929fcf4e4aea`

Its exact closure gates were:

- Quality `35559584432` — success
- Current-tree security `35559584425` — success
- frontend: 60/60
- backend: 28/28
- npm audit: clean
- NuGet vulnerability audit: clean

## Exact characterization

Disposable characterization commit:

`4de3efe9c9fcf18db27ef8532ff1760f3af2f620`

Run:

`35559746453` — success

The exact checked-out carrier source reported:

- `servicetoken-registration=true`
- one ServiceToken type-reference file outside the class: `Program.cs`
- `servicetoken-method-consumer-files=0`
- `servicetoken-non-registration-consumer-files=0`
- `legacy-jwt-config-files-outside-class=0`

The only external reference to `ServiceToken` was its own DI registration.
`GenerateRefreshToken`, `GetPrincipalFromExpiredToken`, and the class's
`GenerateEmailConfirmationToken` had no production consumers.

All legacy `Jwt:Key/Issuer/Audience` references were contained in the dead
class itself.

## Maintained change

B24 removes:

- `Backend/antigal.server/Services/ServiceToken.cs`;
- `builder.Services.AddScoped<ServiceToken>();`.

No active controller, service interface, route or authentication middleware is
changed.

The maintained JWT authority remains:

- `JwtHandler`;
- `JWTSettings:securityKey`;
- `JWTSettings:validIssuer`;
- `JWTSettings:validAudience`;
- `JWTSettings:expiryInMinutes`.

## Permanent authority

`scripts/backend_token_authority.py` is now part of Quality.

It requires:

- legacy `ServiceToken.cs` to remain absent;
- its DI registration to remain absent;
- no maintained backend C# source to restore legacy `Jwt:Key`,
  `Jwt:Issuer` or `Jwt:Audience` authority;
- no maintained backend source to restore the legacy ServiceToken-only method
  names;
- exactly one canonical `JwtHandler` DI registration;
- the canonical handler to remain on `JWTSettings` and B22 expiration
  configuration.

This does not prohibit future refresh-token functionality. It requires a future
implementation to establish an explicit maintained authority instead of silently
reviving the unused 2024 service.

## Implementation evidence

Implementation commit:

`06e5ba1f55dcaecf066a7966a2c9122436ac246d`

Runs:

- Quality `35559803504` — success
- Current-tree security `35559803514` — success
- `legacy-servicetoken-file-count=0`
- `legacy-jwt-config-file-count=0`
- `legacy-servicetoken-method-file-count=0`
- `servicetoken-authority=absent`
- `legacy-jwt-config-authority=absent`
- `jwt-handler-authority=canonical`
- frontend tests: 60/60
- backend tests: 28/28
- backend Release build: 0 warnings
- npm audit: 0 findings
- NuGet vulnerability audit: clean

## Explicit non-goals

B24 does not:

- remove or redesign `JwtHandler`;
- add a refresh-token flow;
- change token claims, issuer, audience, signing algorithm or expiration;
- change ASP.NET authentication middleware;
- change account email-confirmation tokens, which use ASP.NET Identity directly;
- change frontend code;
- change database schema or data.

## Rollback boundary

B24 is reversible by restoring `ServiceToken.cs` and its DI registration.
No database, external identity provider, secret rotation or data migration is
involved.

Temporary lab triggers are removed before carrier promotion. Final closure
requires Quality and Current-tree security to pass again on the exact promoted
carrier SHA.


## Carrier promotion evidence

The cleaned B24 tree was promoted by non-forced fast-forward to the maintained
carrier.

Promoted carrier SHA:

`84e69fb458899290413fa0c0f461fec4a0fe8714`

Exact-SHA carrier validation:

- Quality `35559922827` — success
- Current-tree security `35559922779` — success
- `servicetoken-authority=absent`
- `legacy-jwt-config-authority=absent`
- `jwt-handler-authority=canonical`
- frontend tests: 60/60
- backend tests: 28/28
- backend Release build: 0 warnings
- npm audit: 0 findings
- NuGet vulnerability audit: clean

A documentation-only closure commit follows this evidence. B24 is considered
closed only after permanent Quality and Current-tree security pass again on that
exact closure SHA.
