# B41 — Cart ownership boundary

## Goal

Close the cart IDOR-style boundary without changing the historical route templates.

Before B41, every Cart action accepted a caller-supplied `userId` and delegated it directly to the service/repository without controller authentication or ownership validation.

That allowed the API shape to select a different user's cart by changing the route id.

## Starting checkpoint

Main before B41:

`469f9ba058001582d32e03ef458a4e97e96982b8`

B40 checkpoint:

- frontend: 60/60
- backend: 75/75
- Release compiler warnings: 0
- npm audit: clean
- NuGet vulnerability audit: clean
- PaymentController anonymous actions: 0
- non-repository AppDbContext consumers: 0

## Consumer characterization

Repository search found no maintained frontend consumer of the backend Cart API.

The current frontend cart remains local-state/localStorage based.

Even so, B41 deliberately preserves the backend route contract rather than deleting or renaming it.

Maintained templates remain:

- GET `{userId}`
- POST `{userId}`
- POST `{userId}/items`
- DELETE `{userId}/items/{itemId}`
- DELETE `{userId}/clear`
- POST `{userId}/confirmar`

## Authentication authority

`CartController` is now decorated with:

`[Authorize]`

No action uses `[AllowAnonymous]`.

The JWT bearer configuration already maps:

`NameClaimType = JwtRegisteredClaimNames.Sub`

and `JwtHandler` emits:

`sub = user.Id`

Therefore:

`User.Identity.Name == authenticated user id`

for maintained JWT-authenticated requests.

## Ownership authority

Every one of the six Cart actions performs the same check before calling the service:

`if (!IsCurrentUser(userId)) return Forbid();`

The helper requires:

- an authenticated identity name;
- ordinal equality between authenticated JWT subject and route userId.

A mismatched route id therefore never reaches `ICartService`.

This preserves all service/repository contracts while moving caller ownership validation to the HTTP boundary.

## Runtime proof

Added:

`CartAuthorizationTests.cs`

Four tests prove:

1. CartController is authenticated by default and not anonymous;
2. all six historical route templates are unchanged;
3. a mismatched user is forbidden across all six actions and produces zero service calls;
4. a matching JWT `sub` delegates the exact requested user id.

The test identity uses `JwtRegisteredClaimNames.Sub` as its name claim type to mirror production JWT configuration.

## Permanent gate

Added:

`scripts/backend_cart_ownership_authority.py`

The gate requires:

- controller-level `[Authorize]`;
- zero `AllowAnonymous` markers;
- all six historical route templates;
- exactly six ownership guards;
- `User.Identity.Name` ownership source;
- ordinal id comparison;
- Program `NameClaimType = sub`;
- JwtHandler `sub = user.Id`;
- runtime proof methods.

Expected output:

- `cart-controller-default-policy=authenticated`
- `cart-route-contract=preserved`
- `cart-ownership-key=jwt-sub`
- `cart-userid-route-authority=must-match-authenticated-sub`
- `cart-owned-action-count=6`
- `cart-anonymous-action-count=0`
- `backend-cart-ownership-authority=clean`

## Validation

Final B41 head:

`64cc87f85ef80e2dc6f447fc9815501eec993501`

PR #32 validation:

- Quality `35616801430` — success
- Current-tree security `35616801514` — success
- frontend: 60/60
- backend: 79/79
- Release compiler warnings: 0
- npm audit: 0 findings
- production npm audit: 0 findings
- NuGet vulnerability audit: clean

Published exact-SHA validation:

- Quality `35616965352` — success
- Current-tree security `35616965435` — success

## Explicit non-goals

B41 does not:

- rename Cart routes;
- remove the caller-visible userId segment;
- move Cart identity logic into repositories;
- change cart persistence;
- migrate the frontend cart from localStorage to backend persistence;
- add Admin cross-user Cart access;
- change JWT claim mapping.

## Result

Cart routes still look the same to legitimate callers, but route identity is no longer authority by itself.

A caller can operate only on the cart whose route `userId` matches the authenticated JWT subject.
