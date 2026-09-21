# B23 — System.IdentityModel.Tokens.Jwt 8.23.0

## Why this block exists

B22 deliberately deferred the direct JWT package refresh until JWT expiration
configuration had an explicit contract and permanent Quality authority.

After B22 closed, the direct package remained:

`System.IdentityModel.Tokens.Jwt 8.19.2`

The current NuGet release is `8.23.0`, published 2026-09-18. It remains inside
the supported IdentityModel 8.x line used with .NET 10.

B23 changes only this direct package authority and adds a signed-token
round-trip contract. It does not redesign authentication.

## Starting authority

B23 was branched from the reconciled carrier:

`2d6174c7d0430f1468d8467ea307f823f7d0dbbf`

That SHA had:

- Quality `35559225307` — success
- Current-tree security `35559225336` — success
- frontend: 60/60
- backend: 27/27
- npm audit: clean
- NuGet vulnerability audit: clean

## Package change

B23 changes exactly one maintained direct NuGet version:

`System.IdentityModel.Tokens.Jwt 8.19.2 → 8.23.0`

The permanent backend package authority in Quality is updated to require
`8.23.0`.

No direct package reference is added for
`Microsoft.IdentityModel.Protocols` or
`Microsoft.IdentityModel.Protocols.OpenIdConnect`.

## Resolved graph characterization

The lab explicitly printed the restored IdentityModel graph.

Resolved on the server/test graph:

- `System.IdentityModel.Tokens.Jwt 8.23.0`
- `Microsoft.IdentityModel.Tokens 8.23.0`
- `Microsoft.IdentityModel.JsonWebTokens 8.23.0`
- `Microsoft.IdentityModel.Logging 8.23.0`
- `Microsoft.IdentityModel.Abstractions 8.23.0`
- `Microsoft.IdentityModel.Protocols 8.19.2`
- `Microsoft.IdentityModel.Protocols.OpenIdConnect 8.19.2`

The 8.19.2 protocol packages remain transitively owned by
`Microsoft.AspNetCore.Authentication.JwtBearer 10.0.12`, whose dependency
floor is `Microsoft.IdentityModel.Protocols.OpenIdConnect >= 8.19.2`.
The OpenIdConnect package in turn accepts
`System.IdentityModel.Tokens.Jwt >= 8.19.2`.

Therefore the observed graph is within the declared NuGet compatibility ranges.
B23 intentionally does not add new direct references merely to force every
IdentityModel package to the same patch version.

## Compatibility contract

`JwtPackageCompatibilityTests` creates a token with the real maintained
`JwtHandler`, then validates the signed token through
`JwtSecurityTokenHandler.ValidateToken` using:

- issuer validation;
- audience validation;
- lifetime validation;
- signing-key validation;
- zero test clock skew.

It also verifies the validated token surface, HMAC-SHA256 algorithm, issuer,
audience and approximately one-hour future expiration.

No external identity provider, real credential or HTTP call is used.

The backend maintained suite increases from 27 to 28 tests.

## Lab evidence

Implementation head:

`c7bf57fa3f0161a7e208e0decaf59655b70eae3d`

Runs:

- Quality `35559323585` — success
- Current-tree security `35559323577` — success
- frontend tests: 60/60
- backend tests: 28/28
- backend Release build: 0 warnings
- npm audit: 0 findings
- NuGet vulnerability audit: clean
- repository clean-tree gate: success

## Explicit non-goals

B23 does not:

- change JWT issuer or audience;
- change signing key handling;
- change signing algorithm;
- change B22's 60-minute expiration authority;
- change claims or role mapping;
- change refresh-token behavior;
- change `ServiceToken`;
- add direct protocol-package references;
- change ASP.NET Core authentication packages;
- change frontend code.

## Rollback boundary

B23 is reversible by restoring the direct package pin to 8.19.2 and removing
the B23 compatibility test / package pin update. No database, configuration
secret or external-system migration is involved.

Temporary lab triggers and the graph-reporting diagnostic are removed before
carrier promotion. Final closure requires Quality and Current-tree security to
pass again on the exact promoted carrier SHA.


## Carrier promotion evidence

The cleaned B23 tree was promoted by non-forced fast-forward to the maintained
carrier.

Promoted carrier SHA:

`319dc05eac4ac43891722a08e9afc66005a7f95f`

Exact-SHA carrier validation:

- Quality `35559494397` — success
- Current-tree security `35559494484` — success
- frontend tests: 60/60
- backend tests: 28/28
- backend Release build: 0 warnings
- npm audit: 0 findings
- NuGet vulnerability audit: clean
- direct JWT package authority: `System.IdentityModel.Tokens.Jwt 8.23.0`
- JWT expiration authority: clean
- email DI authority: clean
- GitHub Actions runtime authority: Node 24

A documentation-only closure commit follows this evidence. B23 is considered
closed only after permanent Quality and Current-tree security pass again on that
exact closure SHA.
