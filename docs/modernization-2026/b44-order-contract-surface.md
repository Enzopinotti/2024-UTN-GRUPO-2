# B44 — Order contract surface reduction

## Goal

Remove dead Order service wrappers and repository operations after B43 made the actual externally maintained Orders surface explicit.

## Starting checkpoint

Main before B44:

`5893d7e4f21d45e0bd8775c674f3e9f2240331a3`

B43 established:

- two maintained Orders routes;
- both Admin-only;
- zero anonymous Orders actions;
- OrdersController depends only on IOrderService.

## Measured service surface

Before B44, `IOrderService` exposed seven operations.

Maintained consumers require only:

- `GetAllOrdersAsync`
- `GetOrderByIdAsync`
- `ConfirmOrder`

The following wrappers had no maintained caller:

- `GetOrdersByUserIdAsync`
- `GetOrdersByStatusAsync`
- `GetPendingOrderByUserIdAsync`
- `UpdateOrderStatusAsync`

The last two names are still legitimately used by `ConfirmOrder`, but directly on `IUnitOfWork.Orders`; the redundant service wrappers were not part of that transaction.

## Measured repository surface

Before B44, `IOrderRepository` exposed seven operations.

Maintained workflows require:

- `GetAllOrdersAsync`
- `GetOrderByIdAsync`
- `GetPendingOrderByUserIdAsync`
- `UpdateOrderStatusAsync`

Removed as unconsumed:

- `GetOrdersByUserIdAsync`
- `GetOrdersByStatusAsync`
- `AddOrderAsync`

## Resulting contracts

After B44:

### IOrderService

Exactly 3 operations:

- GetAllOrdersAsync
- GetOrderByIdAsync
- ConfirmOrder

### IOrderRepository

Exactly 4 operations:

- GetAllOrdersAsync
- GetOrderByIdAsync
- GetPendingOrderByUserIdAsync
- UpdateOrderStatusAsync

## Preserved transaction authority

B44 does not modify the business logic inside `ConfirmOrder`.

The maintained path still includes:

- pending-order lookup through `IUnitOfWork.Orders.GetPendingOrderByUserIdAsync`;
- order-status mutation through `IUnitOfWork.Orders.UpdateOrderStatusAsync`;
- Sale creation through `IUnitOfWork.Sales.CreateSaleAsync`;
- product stock mutation;
- explicit commit;
- rollback on failure.

The existing Order/Sale persistence authority tests remain green.

## Runtime proof

Added:

`OrderContractSurfaceTests.cs`

It proves the exact public method sets for:

- IOrderService;
- OrderService;
- IOrderRepository;
- OrderRepository.

The transaction-focused test stub was reduced to the same maintained repository contract.

## Permanent gate

Added:

`scripts/backend_order_contract_surface_authority.py`

The first B44 Quality run exposed a false positive in the new gate: the initial implementation treated repository method names occurring inside `ConfirmOrder` as if the retired service wrappers had returned.

That was a gate bug, not a product-code failure.

The gate was corrected to inspect actual method declarations using regular expressions. No product logic changed in that correction.

Final expected evidence:

- `order-service-maintained-operations=3`
- `order-service-contract=GetAllOrdersAsync,GetOrderByIdAsync,ConfirmOrder`
- `order-repository-maintained-operations=4`
- `order-repository-contract=GetAllOrdersAsync,GetOrderByIdAsync,GetPendingOrderByUserIdAsync,UpdateOrderStatusAsync`
- `retired-order-service-wrapper-count=4`
- `retired-order-repository-operation-count=3`
- `backend-order-contract-surface-authority=clean`

## Validation

Final B44 head:

`7cef9745f289b21c6ddb98cd8770b67c27500694`

Initial diagnostic run:

- Quality `35622765832` — failed only in the new source gate false positive;
- Current-tree security `35622765819` — success.

Final PR #37 validation after the gate correction:

- Quality `35622900838` — success
- Current-tree security `35622900909` — success
- frontend: 60/60
- backend: 90/90
- Release compiler warnings: 0
- npm audit: 0
- production npm audit: 0
- NuGet vulnerability audit: clean

Published exact SHA:

- Quality `35623068108` — success
- Current-tree security `35623068306` — success

## Explicit non-goals

B44 does not:

- change Orders authorization;
- change route templates;
- change OrderService confirmation semantics;
- remove the Order entity/table;
- add a user-facing Orders endpoint;
- modify Sale creation or stock mutation.

## Result

The Order service/repository contracts now describe only maintained behavior.

The external Admin Orders surface and the internal transaction path remain intact, while seven dead contract operations/wrappers were removed.
