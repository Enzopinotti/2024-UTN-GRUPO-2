# B46 — Admin product/image authenticated transport

## Goal

Align the maintained Admin product frontend with backend authorization and close the remaining public Image mutation surface without changing public catalog reads.

## Starting checkpoint

Main before B46:

`f5ef12edc80598a5911752971e8cfe2e8dc82146`

B45 published evidence:

- frontend: 61/61
- backend: 90/90
- Release compiler warnings: 0
- npm / production npm audit: clean
- NuGet vulnerability audit: clean

## Characterization

The frontend persisted JWT access tokens through `AuthContext`, but maintained frontend API calls did not send an Authorization header.

The Admin products flow called protected Product mutations and Image upload directly with `fetch`.

At the same time, `ImageController` exposed three mutating routes without an authorization policy.

The maintained Product catalog read remains public and should not require a token.

A second integration bug was also present: `ImageController/upload` returns:

`{ id, url }`

while the Admin products frontend expected:

`uploadData.isSuccess`

Therefore an otherwise successful upload could be interpreted as a frontend failure.

## Frontend authenticated transport

Added:

`Frontend/antigal.client/src/utils/authenticatedFetch.js`

Contract:

1. reads `localStorage.accessToken`;
2. fails before making any request if no token exists;
3. preserves caller-supplied headers;
4. adds `Authorization: Bearer <accessToken>`;
5. delegates to native `fetch`.

Added runtime tests in:

`Frontend/antigal.client/src/utils/authenticatedFetch.test.js`

The tests prove fail-closed behavior and header preservation.

## Admin product migration

`ProductListContainer.jsx` now uses authenticated transport for exactly five maintained mutations:

- POST Product/addProduct
- PUT Product/updateProduct
- DELETE Product/deleteProduct/{idProducto}
- POST Image/upload during product creation
- POST Image/upload during product editing

The public read:

`GET Product/getProducts`

continues to use plain `fetch`.

B46 does not attach JWTs indiscriminately to public catalog reads.

## Image authorization boundary

`ImageController` is now protected at controller level by:

`[Authorize(Roles = "Admin")]`

Maintained routes:

- POST `upload`
- DELETE `{imageId}`
- DELETE `eliminar-por-url`

Anonymous Image actions: 0.

Added runtime reflection tests:

`Backend/antigal.server.Tests/ImageAuthorizationTests.cs`

They prove:

- controller-level Admin policy;
- all three route templates remain unchanged;
- no action overrides the policy with AllowAnonymous;
- constructor dependency remains IImageService-only.

## Upload response contract

Both Admin Image upload paths now require the actual response field:

`uploadData.url`

The obsolete `uploadData.isSuccess` expectation is removed.

This aligns the frontend with the backend response without changing the backend response shape.

## Permanent gates

Added:

- `scripts/backend_image_authorization_authority.py`
- `scripts/frontend_authenticated_api_authority.mjs`

Quality now enforces:

- ImageController Admin policy;
- 3 maintained Image routes;
- 0 anonymous Image actions;
- persisted access-token source;
- Bearer header injection;
- fail-closed helper proof;
- exactly 5 authenticated Admin product/image mutations;
- public Product/getProducts read remains unauthenticated;
- Image upload frontend response authority is `url`.

Expected evidence includes:

- `image-controller-default-policy=Admin`
- `image-maintained-route-count=3`
- `backend-image-authorization-authority=clean`
- `frontend-auth-token-source=localStorage.accessToken`
- `admin-product-authenticated-mutation-count=5`
- `admin-image-upload-response-authority=url`
- `frontend-authenticated-api-authority=clean`

## Validation

Final B46 functional head:

`dffe0dd132280b3b1bab242b7df46052c5d516c5`

PR #41:

- Quality `35633825492` — success
- Current-tree security `35633825537` — success
- frontend: 63/63
- backend: 93/93
- Release compiler warnings: 0
- npm audit: 0
- production npm audit: 0
- NuGet vulnerability audit: clean

Published exact SHA:

- Quality `35633997525` — success
- Current-tree security `35633997355` — success

## Explicit non-goals

B46 does not:

- make every frontend request authenticated;
- attach tokens to public catalog endpoints;
- implement profile-picture ownership;
- activate Admin Users or Messages backend branches;
- migrate Category mutations from localStorage;
- add a client-side Admin route guard;
- repair password-reset/contact historical integrations.

Those are product-integration concerns and are tracked separately in issue #42.

## Result

The maintained Admin products/image mutation path now has a coherent end-to-end authorization contract:

- frontend sends the persisted JWT using Bearer transport;
- backend Image mutations require Admin;
- public catalog reads remain public;
- Image upload response handling matches the real backend payload.

This is the final functional block of the 2026 modernization program.
