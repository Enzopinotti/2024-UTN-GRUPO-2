# B30 — UnitOfWork contract authority

## Goal

B29 removed the last maintained service-level calls to
`IUnitOfWork.SaveChangesAsync()`.

B30 determines whether the method still represents a real application contract
or only dead surface area left behind by the earlier persistence design.

The block explicitly distinguishes:

- UnitOfWork SaveChanges authority;
- repository/direct AppDbContext persistence;
- transaction coordination through BeginTransactionAsync.

Those concerns are not interchangeable.

## Starting authority

Carrier before B30:

`1fc55a4df7807d97202e37ff840631ba12079dab`

B29 closure:

- Quality `35594120911` — success
- Current-tree security `35594120910` — success
- frontend tests: 60/60
- backend tests: 41/41
- Release compiler warnings: 0
- npm audit: clean
- NuGet vulnerability audit: clean
- maintained service-level UnitOfWork SaveChanges calls: 0

## Characterization

Initial characterization commit:

`ba1a56015d83ee1f906531d57535ce36ccc2c469`

The first classifier intentionally scanned every production SaveChangesAsync
reference. It failed because it treated direct `_context.SaveChangesAsync()`
calls inside services as if they were UnitOfWork calls.

That failure was useful: it exposed that the backend still has legitimate
direct AppDbContext persistence outside repository classes, including
ImageService, LikeService and ContactoController. Those writes are unrelated to
the IUnitOfWork contract and must not be removed by B30.

Classifier correction commit:

`7a43b384f4f118962aca1c91831a882fb25e9421`

Characterization run:

`35595091155` — success

Exact carrier findings:

- production `_unitOfWork.SaveChangesAsync()` calls: 0
- direct production `_context.SaveChangesAsync()` calls: 33
- repository-context saves: 24
- `IUnitOfWork.SaveChangesAsync` contract: present
- `UnitOfWork.SaveChangesAsync` implementation: present
- test IUnitOfWork implementations forced to expose the method: 2
- dead-surface candidate: true

The two test implementations were:

- ProductServiceRepositoryAuthorityTests.StubUnitOfWork
- OrderSalePersistenceAuthorityTests.StubUnitOfWork

No production consumer required the method.

## Maintained change

Implementation commit:

`04a4accc81f3a492fee72f53b3a4adbe920749b7`

### IUnitOfWork

Removed:

`Task<int> SaveChangesAsync();`

Preserved:

`Task<IDbContextTransaction> BeginTransactionAsync();`

Repository properties and IDisposable remain unchanged.

### UnitOfWork

Removed the dead method:

`public async Task<int> SaveChangesAsync()`

and its direct context flush.

No repository persistence code was modified.

### Tests

Removed SaveChanges counters and fake implementations from the two
`IUnitOfWork` stubs because the interface no longer exposes that operation.

Existing mutation tests remain focused on repository behavior and transaction
behavior.

Added:

`UnitOfWorkContractAuthorityTests.cs`

The maintained tests assert:

1. neither `IUnitOfWork` nor `UnitOfWork` exposes `SaveChangesAsync`;
2. both still expose `BeginTransactionAsync` returning
   `Task<IDbContextTransaction>`.

Backend maintained test count increases from 41 to 43.

## Permanent authority gate

Added:

`scripts/backend_unitofwork_contract_authority.py`

Quality now requires:

- SaveChangesAsync absent from IUnitOfWork;
- SaveChangesAsync implementation absent from UnitOfWork;
- zero production `_unitOfWork.SaveChangesAsync()` calls;
- zero test IUnitOfWork implementations carrying a SaveChanges surface;
- BeginTransactionAsync preserved in interface and implementation;
- direct AppDbContext persistence still present.

Expected output:

- `iunitofwork-savechanges-contract=absent`
- `unitofwork-savechanges-implementation=absent`
- `production-unitofwork-save-call-count=0`
- `test-unitofwork-save-surface-count=0`
- `production-direct-context-save-call-count=32`
- `unitofwork-transaction-authority=preserved`
- `unitofwork-authority=repositories-plus-transactions`
- `backend-unitofwork-contract-authority=clean`

The direct-context count drops from 33 to 32 only because the dead
UnitOfWork.SaveChanges implementation itself contained one direct context save.
No business persistence path was removed.

## Lab validation

Quality:

`35595260811` — success

Current-tree security:

`35595260751` — success

Observed:

- frontend tests: 60/60
- backend tests: 43/43
- Release compiler warnings: 0
- npm audit findings: 0
- NuGet vulnerability audit: clean
- UnitOfWork SaveChanges contract: absent
- production UnitOfWork SaveChanges callers: 0
- transaction authority: preserved
- direct AppDbContext persistence calls: 32

## Resulting architecture

After B30, UnitOfWork has two explicit responsibilities:

1. provide coordinated access to repository instances that share the scoped
   AppDbContext;
2. expose the explicit transaction boundary used by workflows such as
   OrderService.ConfirmOrder.

Persistence remains owned by the repository/direct-context mutation paths that
already existed and were separately characterized in B28/B29.

The misleading third responsibility — a generic application-level
SaveChangesAsync surface with no consumers — is removed.

## Explicit non-goals

B30 does not:

- remove any repository SaveChangesAsync call;
- remove ImageService or LikeService direct context persistence;
- remove ContactoController direct context persistence;
- redesign those direct-context services;
- alter BeginTransactionAsync;
- alter ConfirmOrder commit/rollback behavior;
- change AppDbContext lifetime;
- change database schema, migrations, routes or response contracts.

Those remaining direct-context persistence paths can be audited separately,
rather than being conflated with the now-retired UnitOfWork API.

## Rollback boundary

Rollback requires restoring:

1. `Task<int> SaveChangesAsync()` to IUnitOfWork;
2. the UnitOfWork implementation that forwards to AppDbContext;
3. corresponding test-stub implementations if required.

No schema, data, external-service or deployment migration is involved.

Temporary characterization workflow and lab triggers are removed before carrier
promotion.
