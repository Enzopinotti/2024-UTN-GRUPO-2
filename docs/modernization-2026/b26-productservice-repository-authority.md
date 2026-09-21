# B26 — ProductService repository authority

## Goal

B25 established explicit repository ownership and showed that
`IProductRepository` remained the only repository with two ownership paths:

- a direct scoped DI registration consumed by `ProductService`;
- a lazy `ProductRepository` owned by `UnitOfWork.Products`.

B26 removes that split authority while preserving all product behavior.

## Starting authority

Carrier before B26:

`5598f05a37d5886b2f0b68d0c48ca49d49f2897a`

B25 closure:

- Quality `35560826500` — success
- Current-tree security `35560826477` — success
- frontend: 60/60
- backend: 28/28
- Release build: 0 warnings
- npm audit: clean
- NuGet vulnerability audit: clean

## Characterization

Disposable characterization commit:

`c8bbb61c4de1a71e54ea6059a4fd9995faa952f4`

Characterization run:

`35561377355` — success

The exact carrier proved all of the following at the same time:

- `Program.cs` directly registered
  `IProductRepository -> ProductRepository`;
- `ProductService` stored a direct `IProductRepository` field;
- `ProductService` required that repository in its constructor;
- `GetProducts` used the direct repository;
- `GetProductsHomeAsync` used the direct repository;
- the rest of the product service already used `IUnitOfWork.Products`;
- `UnitOfWork.Products` already lazily constructed
  `ProductRepository(_context)`.

Observed characterization:

- `direct-product-repository-registration=True`
- `productservice-direct-field=True`
- `productservice-direct-constructor-parameter=True`
- `productservice-direct-get-products=True`
- `productservice-direct-get-featured=True`
- `productservice-unitofwork-get-products=False`
- `productservice-unitofwork-get-featured=False`
- `unitofwork-product-owner=True`

`ProductRepository` itself is stateless apart from its `AppDbContext`.
Both historical instances therefore represented the same repository behavior
over the same request-scoped database context, but through two ownership paths.

## Maintained change

Implementation commit:

`f2e04bc649641fc5d7669a1a03c641fc0e7c4dce`

The change is deliberately narrow.

### ProductService

Removed:

- direct `IProductRepository` field;
- direct `IProductRepository` constructor parameter.

The constructor is now:

`ProductService(IUnitOfWork unitOfWork, ResponseDto response)`

The two exceptional methods now use the same authority as the rest of the
service:

- `GetProducts(...) -> _unitOfWork.Products.GetProductsAsync(...)`
- `GetProductsHomeAsync() -> _unitOfWork.Products.GetFeaturedProductsAsync()`

No product query implementation changed.

### Program.cs

Removed:

`AddScoped<IProductRepository, ProductRepository>()`

`ProductRepository` remains active through `UnitOfWork.Products`.

### Repository ownership gate

`scripts/backend_repository_di_authority.py` now classifies Products together
with the other UnitOfWork-owned repositories.

It requires:

- no direct `IProductRepository` DI registration;
- `IUnitOfWork.Products` to remain present;
- `UnitOfWork.Products` to remain the lazy ProductRepository owner;
- no direct `IProductRepository` constructor dependency in ProductService;
- no `_productRepository` state in ProductService;
- `GetProducts` and `GetProductsHomeAsync` to use
  `IUnitOfWork.Products`.

The direct repository DI set is now intentionally limited to:

- `IPaymentRepository`
- `IEnvioRepository`

because those still have maintained direct consumers.

## Behavioral tests

Added:

`ProductServiceRepositoryAuthorityTests.cs`

Four tests cover the B26 boundary:

1. `GetProducts` delegates filters through `IUnitOfWork.Products` and
   preserves the historical successful response contract.
2. `GetProductsHomeAsync` gets featured products through
   `IUnitOfWork.Products` and preserves the successful response contract.
3. the empty featured-products response remains unchanged.
4. ProductService exposes no direct `IProductRepository` constructor
   dependency.

The backend maintained suite therefore increases from 28 to 32 tests.

## Lab validation

Quality:

`35561468923` — success

Current-tree security:

`35561469048` — success

Observed authority:

- `unitofwork-owned-direct-registration-count=0`
- `unitofwork-owned-repositories=Products,Orders,Sales,Categories,ProductCategories,Carts`
- `productservice-product-repository-authority=IUnitOfWork.Products`
- `direct-repository-di=IPaymentRepository,IEnvioRepository`

Regression baseline:

- frontend tests: 60/60
- backend tests: 32/32
- Release compiler warnings: 0
- npm audit findings: 0
- NuGet vulnerability audit: clean

## Explicit non-goals

B26 does not:

- alter ProductRepository queries;
- alter ProductRepository lifetime inside UnitOfWork;
- alter AppDbContext lifetime;
- change transaction behavior;
- modify product controllers/routes;
- change database schema or data;
- modify product DTOs/models;
- change the mutable injected `ResponseDto` pattern.

The `ResponseDto` lifetime/state question remains intentionally isolated for
a later block.

## Rollback boundary

B26 can be rolled back by:

1. restoring the direct ProductRepository DI registration;
2. restoring the ProductService direct repository constructor parameter/field;
3. switching the two affected methods back to that field.

There are no schema, data or external-service migrations.

Temporary characterization workflow and lab branch triggers are removed before
carrier promotion.
