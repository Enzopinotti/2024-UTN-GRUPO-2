# B43 — Orders authorization boundary

## Goal

Close the remaining public Orders management surface without changing routes or the transactional order-confirmation workflow.

## Starting checkpoint

Main before B43:

`0a1a8804f0342b96feb2bb531e86fd118523ce41`

B42 checkpoint:

- frontend: 60/60
- backend: 83/83
- Release compiler warnings: 0
- npm / production npm audit: clean
- NuGet vulnerability audit: clean
- public Sale API removed
- Sale creation preserved inside OrderService.ConfirmOrder

## Consumer characterization

The maintained frontend does not consume the backend Orders API.

The profile Orders page has:

- `usingBackend = false`;
- a permanently disabled branch;
- a placeholder `http://localhost:5279/api/` URL;
- the comment `//falta endpoint`.

The visible profile flow uses local `fakeOrders` data instead.

The backend Orders controller exposes two management routes:

- GET `api/Orders/all`
- POST `api/Orders/confirm/{orderId}`

The confirmation action is not a read-only user endpoint. It:

1. resolves the order;
2. invokes the transactional confirmation workflow;
3. changes order state;
4. creates a Sale;
5. decrements stock;
6. commits or rolls back the explicit transaction.

## Authorization decision

`OrdersController` is now:

`[Authorize(Roles = "Admin")]`

at controller level.

Both maintained routes inherit that policy.

There are zero `AllowAnonymous` actions.

The route templates remain unchanged.

## Runtime proof

Added:

`OrdersAuthorizationTests.cs`

The tests prove:

1. controller default role is Admin;
2. GET `all` remains present;
3. POST `confirm/{orderId}` remains present;
4. neither action overrides the Admin policy;
5. OrdersController depends only on IOrderService.

## Permanent gate

Added:

`scripts/backend_orders_authorization_authority.py`

Expected evidence:

- `orders-controller-default-policy=Admin`
- `orders-maintained-route-count=2`
- `orders-admin-routes=GET-all,POST-confirm`
- `orders-anonymous-action-count=0`
- `orderscontroller-dependencies=IOrderService-only`
- `backend-orders-authorization-authority=clean`

## Validation

Final B43 head:

`5893d7e4f21d45e0bd8775c674f3e9f2240331a3`

PR #36:

- Quality `35618810137` — success
- Current-tree security `35618810159` — success
- frontend: 60/60
- backend: 86/86
- Release compiler warnings: 0
- npm audit: 0
- production npm audit: 0
- NuGet vulnerability audit: clean

Published exact SHA:

- Quality `35622430426` — success
- Current-tree security `35622429764` — success

## Explicit non-goals

B43 does not:

- rename either Orders route;
- alter OrderService;
- alter repositories;
- change transaction semantics;
- add a user-facing `Mis Pedidos` endpoint;
- make the Admin Orders API owner-bound.

A future user-order API is a separate product surface and should derive ownership from authenticated identity.

## Result

The existing Orders API is now explicitly a management surface.

Both maintained routes are Admin-only, while the transactional behavior of order confirmation remains unchanged.
