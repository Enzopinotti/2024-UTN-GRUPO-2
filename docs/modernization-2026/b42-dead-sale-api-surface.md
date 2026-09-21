# B42 — Dead Sale API surface removal

## Goal

Remove a redundant public Sale API that trusted caller-supplied identity and business values, while preserving the real transactional sale-creation workflow owned by order confirmation.

## Starting checkpoint

Main before B42:

`5ca38e97e908775975101892ec6c043a914f0db6`

B41 checkpoint:

- frontend: 60/60
- backend: 79/79
- Release compiler warnings: 0
- npm / production npm audit: clean
- NuGet vulnerability audit: clean
- Cart routes authenticated and owner-bound

## Characterization

Repository search found no maintained frontend consumer of `api/Sale`.

The historical SaleController exposed:

- POST `realizar`
- GET `{idVenta}`
- PUT `{idVenta}/estado`

The create endpoint accepted caller-supplied:

- `idUsuario`
- `idOrden`
- `total`
- `metodoPago`

That surface duplicated a stronger maintained workflow.

`OrderService.ConfirmOrder` already owns the real business transition:

1. validate user;
2. load the pending order;
3. validate products/stock;
4. update order status;
5. create the Sale through `IUnitOfWork.Sales.CreateSaleAsync`;
6. decrement product stock;
7. commit one explicit transaction;
8. rollback on failure.

## Removed surface

B42 removes:

- `SaleController`
- `ISaleService`
- `SaleService`
- `SaleRequestDto`
- `SaleResponseDto`
- `SaleDto`
- SaleService DI registration
- the unused SaleService dependency from OrdersController
- `ISaleRepository.GetSaleByIdAsync`
- `ISaleRepository.UpdateSaleAsync`
- their SaleRepository implementations

An unused SaleResponseDto construction inside OrderService was also removed.

## Preserved authority

B42 does **not** remove sales from the product model.

Preserved:

- `Sale` entity and table;
- `VentaEstado`;
- `IUnitOfWork.Sales`;
- `SaleRepository`;
- `CreateSaleAsync`;
- order-confirmation transaction;
- order status persistence;
- sale creation persistence;
- product stock persistence;
- commit/rollback behavior.

`ISaleRepository` is deliberately create-only after B42.

## Tests and gates

`OrderSalePersistenceAuthorityTests` now focuses only on the maintained path:

- successful confirmation commits after repository-owned mutations;
- insufficient stock rolls back before mutations.

Added:

`DeadSaleApiSurfaceTests.cs`

It proves:

1. SaleController is absent from the production assembly;
2. SaleService and ISaleService are absent;
3. OrdersController depends only on IOrderService;
4. ISaleRepository exposes exactly one maintained operation: CreateSaleAsync;
5. legacy Sale API DTOs are absent.

Added:

`scripts/backend_dead_sale_api_authority.py`

The existing order/sale persistence gate was rewritten to protect only the maintained transaction path and create-only Sale repository authority.

Expected evidence includes:

- `sale-controller=absent`
- `sale-service-contract=absent`
- `sale-service=absent`
- `legacy-sale-dtos=absent`
- `orderscontroller-sale-service-dependency=absent`
- `sale-repository-authority=CreateSaleAsync-only`
- `order-confirm-sale-creation=preserved`
- `backend-dead-sale-api-authority=clean`

## Validation

Final B42 head:

`aa91450a063b73c01eb24ce07582e0a91562a565`

PR #34:

- Quality `35618063638` — success
- Current-tree security `35618063727` — success
- frontend: 60/60
- backend: 83/83
- Release compiler warnings: 0
- npm audit: 0
- production npm audit: 0
- NuGet vulnerability audit: clean

Published exact SHA:

- Quality `35618249659` — success
- Current-tree security `35618249686` — success

## Explicit non-goals

B42 does not:

- remove the Sale entity/table;
- modify database schema;
- change `VentaEstado`;
- change OrderService transaction boundaries;
- change stock validation;
- change the Cart -> Order transition;
- introduce a replacement public Sale API.

## Result

There is now one maintained authority for creating a Sale: the transactional order-confirmation workflow.

The redundant public API, service layer and read/update repository surface are gone.
