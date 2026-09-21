# B29 — Order/Sale persistence and transaction authority

## Goal

B28 eliminated the three redundant ProductService UnitOfWork saves and left
exactly two maintained service-level `IUnitOfWork.SaveChangesAsync()` calls:

- one in `OrderService.ConfirmOrder`;
- one in `SaleService.UpdateSaleStatusAsync`.

B29 characterizes those two paths separately before changing them, because
`ConfirmOrder` also owns an explicit database transaction whose atomicity must
not be confused with the redundant final flush.

## Starting authority

Carrier before B29:

`a8b5534219c35c8faff1837d28a706735d8a430f`

B28 closure:

- Quality `35562571949` — success
- Current-tree security `35562571979` — success
- frontend tests: 60/60
- backend tests: 38/38
- Release compiler warnings: 0
- npm audit: clean
- NuGet vulnerability audit: clean

## Characterization

Disposable characterization commit:

`0a07f55525ba889822ae1f0e64d9d6a6fd84b1de`

Characterization run:

`35593368031` — success

Exact-tree findings:

### ConfirmOrder

- service-level UnitOfWork save count: 1
- BeginTransactionAsync: present
- CommitAsync: present
- RollbackAsync: present
- OrderRepository.UpdateOrderStatusAsync path: present
- SaleRepository.CreateSaleAsync path: present
- ProductRepository.UpdateProductAsync path: present

Every mutation delegated inside that transaction already self-persisted:

- OrderRepository.UpdateOrderStatusAsync: 1 context save
- SaleRepository.CreateSaleAsync: 1 context save
- ProductRepository.UpdateProductAsync: 1 context save

The final `_unitOfWork.SaveChangesAsync()` therefore occurred after the
repository mutation paths had already flushed the tracked changes into the
still-open transaction.

The **transaction commit remains semantically required**. The final empty
UnitOfWork flush does not.

### UpdateSaleStatusAsync

The service called:

1. `SaleRepository.UpdateSaleAsync(sale)`;
2. then `IUnitOfWork.SaveChangesAsync()`.

`SaleRepository.UpdateSaleAsync` already performs exactly one
`AppDbContext.SaveChangesAsync()`, so the service call was a second flush.

## Maintained change

Primary implementation commit:

`90535f96847edc8da9bb84b12c1171df67c688f4`

### OrderService

Removed exactly:

`await _unitOfWork.SaveChangesAsync();`

from the end of the successful mutation sequence in `ConfirmOrder`.

Preserved unchanged:

- `BeginTransactionAsync()`;
- the try/catch transaction boundary;
- `CommitAsync()` on success;
- `RollbackAsync()` on failure;
- order-status persistence;
- sale creation;
- per-product stock update persistence.

The resulting persistence model is:

`Begin transaction -> repository-owned saves -> Commit transaction`

instead of:

`Begin transaction -> repository-owned saves -> empty UnitOfWork save -> Commit transaction`.

### SaleService

Removed the conditional second UnitOfWork save after successful
`SaleRepository.UpdateSaleAsync`.

The repository remains the mutation/persistence owner.

## Permanent authority gate

Added:

`scripts/backend_order_sale_persistence_authority.py`

Quality now requires:

- ConfirmOrder service-level save count = 0;
- UpdateSaleStatusAsync service-level save count = 0;
- ConfirmOrder transaction begin/commit/rollback contracts to remain present;
- OrderRepository.UpdateOrderStatusAsync to retain exactly one context save;
- SaleRepository.CreateSaleAsync to retain exactly one context save;
- SaleRepository.UpdateSaleAsync to retain exactly one context save;
- ProductRepository.UpdateProductAsync to retain exactly one context save;
- ConfirmOrder repository mutation paths to remain present;
- SaleService to continue delegating its update through SaleRepository.

Expected authority output:

- `order-confirm-service-save-count=0`
- `sale-update-service-save-count=0`
- `order-confirm-transaction-authority=preserved`
- `order-confirm-mutation-persistence=repository-owned-inside-transaction`
- `sale-update-persistence=SaleRepository`
- `residual-service-unitofwork-save-count=0`
- `backend-order-sale-persistence-authority=clean`

## Behavioral coverage

Added:

`OrderSalePersistenceAuthorityTests.cs`

The suite covers three boundaries.

### Sale status update

Proves that:

- the requested state change is applied;
- SaleRepository.UpdateSaleAsync is invoked exactly once;
- UnitOfWork.SaveChangesAsync is not invoked afterwards.

### ConfirmOrder success

Uses explicit test doubles for UnitOfWork, repositories, UserManager,
ProductService and IDbContextTransaction.

Proves that:

- order status changes to `Confirmada`;
- product stock is decremented;
- order status repository mutation occurs once;
- sale creation occurs once;
- product update occurs once;
- UnitOfWork.SaveChangesAsync remains zero;
- transaction CommitAsync occurs exactly once;
- RollbackAsync does not run.

### ConfirmOrder failure

The insufficient-stock path proves that:

- the order remains pending;
- stock remains unchanged;
- no mutation repositories are called;
- UnitOfWork.SaveChangesAsync remains zero;
- transaction CommitAsync does not run;
- RollbackAsync runs exactly once.

Backend maintained test count therefore increases from 38 to 41.

## CI hardening during the lab

The first test-fixture version intentionally went through normal Quality rather
than bypassing it.

Implementation run:

`35593530200` — failed at backend build because the test subclass inherited a
member named `Options`, causing unqualified `Options.Create(...)` to resolve
incorrectly.

Fixture correction commit:

`f29d1b7651e08d5b81adb853be3c3a7a451634fa`

The next run compiled, and the existing zero-warning gate then rejected one
CS8625 warning caused by passing a null IServiceProvider to the UserManager test
base constructor.

Warning-free fixture correction commit:

`1f22093c69548d135cb55f9f68677f6329aef82d`

The fixture now uses an explicit empty ServiceProvider and remains warning-free.

These iterations changed only test infrastructure after the primary product
change; the production OrderService/SaleService edit did not expand.

## Final lab validation

Quality:

`35593754059` — success

Current-tree security:

`35593754052` — success

Observed:

- `order-confirm-service-save-count=0`
- `sale-update-service-save-count=0`
- `order-confirm-transaction-authority=preserved`
- `residual-service-unitofwork-save-count=0`
- frontend tests: 60/60
- backend tests: 41/41
- Release compiler warnings: 0
- npm audit findings: 0
- NuGet vulnerability audit: clean

## Resulting backend persistence shape

After B29 there are **zero maintained calls to
`IUnitOfWork.SaveChangesAsync()` from service code**.

That does not mean IUnitOfWork.SaveChangesAsync is deleted in B29.
It remains part of the interface/implementation until a separate authority
audit proves whether any non-service consumers or future transaction patterns
require it.

Repositories remain the dominant persistence owner, while UnitOfWork still
coordinates repository access and the explicit ConfirmOrder transaction.

## Explicit non-goals

B29 does not:

- remove IUnitOfWork.SaveChangesAsync;
- move repository saves into UnitOfWork;
- alter BeginTransactionAsync;
- alter database isolation;
- combine the multiple repository saves inside ConfirmOrder into one EF flush;
- change order, sale or product schemas;
- change controller routes or response contracts;
- change stock validation or sale calculation logic.

A future block may characterize the now-unused UnitOfWork SaveChanges surface,
but deleting it is intentionally outside B29.

## Rollback boundary

Rollback requires restoring one UnitOfWork save in ConfirmOrder and one after a
successful SaleService status update.

Transaction begin/commit/rollback code is not part of the rollback because B29
never changes it.

No schema, data, external-service or deployment migration is involved.

Temporary characterization workflow and lab triggers are removed before carrier
promotion.


## Carrier promotion evidence

The cleaned B29 tree was promoted by non-forced fast-forward.

Promoted carrier SHA:

`41bbff780612c7bd24e8bef97a3ba80bc2f22297`

Exact-SHA carrier validation:

- Quality `35593992926` — success
- Current-tree security `35593992925` — success
- frontend tests: 60/60
- backend tests: 41/41
- Release compiler warnings: 0
- npm audit findings: 0
- NuGet vulnerability audit: clean
- `order-confirm-service-save-count=0`
- `sale-update-service-save-count=0`
- `order-confirm-transaction-authority=preserved`
- `residual-service-unitofwork-save-count=0`
- `backend-order-sale-persistence-authority=clean`

A documentation-only closure commit follows this evidence. B29 is considered
closed only after permanent Quality and Current-tree security pass again on that
exact closure SHA.
