# B45 — Profile Orders dead backend branch removal

## Goal

Remove an unreachable frontend backend-placeholder branch from the profile `Mis Pedidos` page while preserving the exact visible behavior.

## Starting checkpoint

Main before B45:

`18b6f80337d295159fcaa9a0db82145da2b03b18`

B44 checkpoint:

- frontend: 60/60
- backend: 90/90
- Release compiler warnings: 0
- npm / production npm audit: clean
- NuGet vulnerability audit: clean
- Orders management API: Admin-only
- Order service/repository contracts reduced to maintained operations

## Characterization

Before B45, `Orders.jsx` declared:

`const [usingBackend] = useState(false);`

There was no setter.

Therefore the backend branch was permanently unreachable.

That branch contained:

- `fetch("http://localhost:5279/api/")`;
- the comment `//falta endpoint`;
- backend error handling;
- SweetAlert usage reachable only through the dead branch.

The maintained visible behavior was always:

`fakeOrders.filter(order => order.userId === currentUser.id)`

## Change

`Orders.jsx` now derives its list directly from the maintained source:

`const orders = fakeOrders.filter((order) => order.userId === currentUser.id);`

Removed from the component:

- `useState`;
- `useEffect`;
- SweetAlert;
- `usingBackend`;
- the localhost placeholder URL;
- `fetch`;
- the unreachable error path.

The component became synchronous and declarative.

## Runtime proof

Added:

`Frontend/antigal.client/src/pages/profile/Orders.state.test.jsx`

The test:

1. renders the page as user 1;
2. proves only user 1 demo orders are supplied;
3. proves the user 2 order is absent;
4. rerenders with user 2 through the mocked outlet context;
5. proves the list follows the changed current user;
6. proves `fetch` is never called.

This raised the maintained frontend suite from 60/60 to 61/61.

## Permanent source authority

Added:

`scripts/frontend_profile_orders_authority.mjs`

The gate requires:

- local `fakeOrders` import;
- filter by `currentUser.id`;
- maintained `UserOrderListContainer` handoff;
- runtime proof presence.

It rejects the return of:

- `useState`;
- `useEffect`;
- SweetAlert;
- `usingBackend`;
- `fetch(`;
- `localhost:5279`;
- `//falta endpoint`.

Expected evidence:

- `profile-orders-data-source=fakeOrders`
- `profile-orders-user-filter=currentUser.id`
- `profile-orders-dead-backend-branch=absent`
- `profile-orders-network-call=absent`
- `frontend-profile-orders-authority=clean`

## Validation

Final B45 head:

`62d81cee1d5b21068f93675c4e2c6c07b7e6ed19`

PR #39:

- Quality `35623872565` — success
- Current-tree security `35623872758` — success
- frontend: 61/61
- backend: 90/90
- Release compiler warnings: 0
- npm audit: 0
- production npm audit: 0
- NuGet vulnerability audit: clean

Published exact SHA:

- Quality `35624048526` — success
- Current-tree security `35624048479` — success

## Explicit non-goals

B45 does not:

- add a real user-order endpoint;
- remove demo order data;
- change order-card rendering;
- change the Admin Orders API;
- connect the profile to backend order persistence.

If a real `Mis Pedidos` API is added later, it should be designed as a separate authenticated owner-bound surface, not by reusing the Admin Orders routes.

## Result

The profile Orders page now states its real behavior in code: it renders local demo orders filtered by the current outlet user.

There is no fake backend switch, placeholder URL or unreachable network/error path left in the component.
