# Backend configuration boundary

The 2024 repository historically committed runtime configuration directly in
`Backend/antigal.server/appsettings.json`.

The maintained 2026 tree keeps only non-secret structure/defaults in that file.
Sensitive or account-specific values must be supplied outside Git.

ASP.NET Core environment variables override nested configuration keys using
double underscores.

## Required external configuration

Set these only in a local shell, IDE secret store, CI secret store or deployment
platform:

```text
ConnectionStrings__DefaultConnection
JWTSettings__securityKey
JWTSettings__validIssuer
JWTSettings__validAudience
Cloudinary__CloudName
Cloudinary__ApiKey
Cloudinary__ApiSecret
EmailConfiguration__From
EmailConfiguration__Port
EmailConfiguration__Username
EmailConfiguration__Password
EmailConfiguration__SmtpServer
MercadoPago__AccessToken
```

Do **not** commit real values.

## Maintained non-secret JWT defaults

`JWTSettings__expiryInMinutes` defaults to `60` minutes in tracked
`appsettings.json`. It may be overridden through normal ASP.NET Core
configuration, but the effective value must be a positive integer.

This value is application behavior, not a secret. Keeping a tracked positive
default prevents the historical missing-value behavior from generating tokens
with effectively immediate expiration.

## Important historical boundary

Removing values from the current tree does not remove them from Git history and
does not prove that any previously committed credential has been rotated or
revoked.

If a historical value represented a live privileged credential, its owner must
rotate/revoke it at the corresponding provider. Historical values must not be
copied into issues, logs, documentation or CI artifacts.
