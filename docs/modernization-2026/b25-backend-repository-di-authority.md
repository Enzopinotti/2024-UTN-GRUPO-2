# B25 — Backend DI / repository ownership authority

## Why this block exists

B24 proved that a registered backend service could be completely unused.
B25 therefore audited every explicit generic service registration in
`Program.cs` instead of assuming that all remaining DI registrations were
runtime authority.

The goal was not to delete repository code. It was to distinguish:

1. services/repositories actually resolved from dependency injection;
2. repositories owned and lazily constructed by `UnitOfWork`;
3. registrations that duplicated that UnitOfWork ownership without any direct
   consumer.

## Starting authority

Carrier before B25:

`ebba6390cfde203c5fc73d48b775d200d215521d`

Exact closure gates:

- Quality `35560000525` — success
- Current-tree security `35560000523` — success
- frontend: 60/60
- backend: 28/28
- npm audit: clean
- NuGet vulnerability audit: clean

## Exact inventory

Disposable inventory commit:

`83cd92005c3709fc66eb81998b412b702bda2494`

Inventory run:

`35560476324` — success

The inventory parsed the exact checked-out `Program.cs` and correlated its
generic `AddScoped/AddTransient/AddSingleton` registrations with maintained
backend C# consumers.

It found **22 explicit generic registrations**.

Four repository registrations had zero direct DI consumers:

- `ICategoriaRepository -> CategoriaRepository`
- `IProductCategoryRepository -> ProductCategoryRepository`
- `ICartRepository -> CartRepository`
- `IOrderRepository -> OrderRepository`

These were not dead repositories.

`UnitOfWork` owns them and lazily constructs them:

- `Orders -> new OrderRepository(_context)`
- `Categories -> new CategoriaRepository(_context)`
- `ProductCategories -> new ProductCategoryRepository(_context)`
- `Carts -> new CartRepository(_context, _carritoMapper)`

This matches the existing `SaleRepository` pattern, which was already
UnitOfWork-owned without a direct DI registration.

## Maintained change

B25 removes only these four redundant lines from `Program.cs`:

- `AddScoped<ICategoriaRepository, CategoriaRepository>()`
- `AddScoped<IProductCategoryRepository, ProductCategoryRepository>()`
- `AddScoped<ICartRepository, CartRepository>()`
- `AddScoped<IOrderRepository, OrderRepository>()`

No repository class, repository interface or UnitOfWork property is removed.

Runtime ownership therefore remains unchanged: the same UnitOfWork continues
to construct the same repository implementation against its scoped
`AppDbContext`.

## Registrations deliberately retained

The inventory also prevented over-cleanup.

### Direct repository DI retained

`IProductRepository -> ProductRepository` remains directly registered because
`ProductService` consumes `IProductRepository` directly for maintained
product query paths, in addition to using `IUnitOfWork` elsewhere.

`IPaymentRepository -> PaymentRepository` remains directly registered because
`PaymentService` consumes it.

`IEnvioRepository -> EnvioRepository` remains directly registered because
both `UnitOfWork` and `EnvioService` consume it through DI.

### Other concrete registrations retained

`JwtHandler` has an active controller consumer.

`CarritoMapper` is consumed by `UnitOfWork` and `CartRepository`.

`ResponseDto` is consumed by `ProductService`. Its stateful injection is a
separate design question; B25 does not classify it as dead simply because a DTO
is unusual as a DI service.

## Permanent authority

`scripts/backend_repository_di_authority.py` is now part of Quality.

It requires:

- no direct DI registrations for the four UnitOfWork-owned repositories;
- the corresponding UnitOfWork lazy-construction contracts to remain present;
- the corresponding `IUnitOfWork` properties to remain present;
- direct DI registrations for `IProductRepository`,
  `IPaymentRepository` and `IEnvioRepository` to remain while their current
  direct consumers exist;
- `ISaleRepository` to remain UnitOfWork-owned rather than gaining a redundant
  direct registration.

This makes repository ownership explicit instead of relying on comments or
accidental registration order.

## Implementation evidence

Implementation commit:

`a1a33fbfb331ef81b5f802008807fa12bdaddaf3`

Runs:

- Quality `35560583762` — success
- Current-tree security `35560583750` — success
- `unitofwork-owned-direct-registration-count=0`
- `unitofwork-owned-repositories=Orders,Sales,Categories,ProductCategories,Carts`
- `direct-repository-di=IProductRepository,IPaymentRepository,IEnvioRepository`
- `backend-repository-di-authority=clean`
- frontend tests: 60/60
- backend tests: 28/28
- backend Release build: 0 warnings
- npm audit: 0 findings
- NuGet vulnerability audit: clean

## Explicit non-goals

B25 does not:

- remove repository implementations or interfaces;
- refactor UnitOfWork;
- consolidate the two ProductRepository access paths in ProductService;
- change transaction boundaries;
- change DbContext lifetime;
- change repository behavior or queries;
- redesign ResponseDto lifetime/state;
- change controllers, routes, database schema or frontend code.

## Follow-up candidates

The inventory exposed two design questions that are **not** dead-authority
removals and should remain separate blocks:

1. `ProductService` uses both a directly injected `IProductRepository` and
   `IUnitOfWork.Products`. A future block can characterize whether those
   access paths can be canonicalized without behavior change.
2. `ProductService` injects a mutable transient `ResponseDto` while most
   other methods allocate responses locally. A future block can characterize
   request/state behavior before changing that lifetime.

Keeping those questions separate preserves attribution and rollback boundaries.

## Rollback boundary

B25 is reversible by restoring the four direct registrations. No repository,
schema, data, external API or configuration migration is involved.

Temporary lab triggers are removed before carrier promotion. Final closure
requires Quality and Current-tree security to pass again on the exact promoted
carrier SHA.


## Carrier promotion evidence

The cleaned B25 tree was promoted by non-forced fast-forward to the maintained
carrier.

Promoted carrier SHA:

`9210536e4946c283955e9d6b823a9d18f086856d`

Exact-SHA carrier validation:

- Quality `35560719529` — success
- Current-tree security `35560719572` — success
- `unitofwork-owned-direct-registration-count=0`
- `unitofwork-owned-repositories=Orders,Sales,Categories,ProductCategories,Carts`
- `direct-repository-di=IProductRepository,IPaymentRepository,IEnvioRepository`
- `backend-repository-di-authority=clean`
- frontend tests: 60/60
- backend tests: 28/28
- backend Release build: 0 warnings
- npm audit: 0 findings
- NuGet vulnerability audit: clean

A documentation-only closure commit follows this evidence. B25 is considered
closed only after permanent Quality and Current-tree security pass again on that
exact closure SHA.
