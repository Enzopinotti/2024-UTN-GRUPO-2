# B28 — Product mutation persistence authority

## Goal

B27 closed the ProductService dependency/state cleanup. B28 audits the next
product boundary: who owns persistence after a product mutation.

The historical product path had two persistence layers for add/update/delete:

1. ProductRepository performed `AppDbContext.SaveChangesAsync()`;
2. ProductService immediately called `IUnitOfWork.SaveChangesAsync()` again.

Import already behaved differently: ProductRepository persisted the workbook
batch and ProductService did not issue a second save.

B28 makes that existing repository-owned persistence boundary explicit and
removes only the redundant product-service commits.

## Starting authority

Carrier before B28:

`9579bfda31c32bd62bcfaa0786b4e56e17082f73`

B27 closure:

- Quality `35562039364` — success
- Current-tree security `35562039363` — success
- frontend tests: 60/60
- backend tests: 34/34
- Release build: 0 warnings
- npm audit: clean
- NuGet vulnerability audit: clean

## Characterization

Disposable characterization commit:

`8d7bddeb720328299fe60626711d017db6deb43c`

Characterization run:

`35562310612` — success

### Backend-wide persistence shape

Exact-tree counts:

- repository files calling `_context.SaveChangesAsync()`: 9
- repository-level save calls: 24
- service files calling `_unitOfWork.SaveChangesAsync()`: 3
- service-level UnitOfWork save calls: 5

Repository save distribution:

- CartRepository: 5
- CategoryRepository: 3
- EnvioRepository: 3
- OrderRepository: 2
- PaymentRepository: 2
- ProductCategoryRepository: 2
- ProductRepository: 4
- SaleRepository: 2
- UnitOfWork: 1

Service UnitOfWork-save distribution:

- OrderService: 1
- ProductService: 3
- SaleService: 1

This demonstrates that repository-owned mutation persistence is the dominant
maintained backend behavior. B28 therefore does not attempt a repository-wide
transaction redesign.

### Product-specific characterization

ProductRepository self-persisted all four mutation paths:

- AddProductAsync: yes
- UpdateProductAsync: yes
- DeleteProductAsync: yes
- ImportProductsFromExcelAsync: yes

ProductService then issued a second UnitOfWork save for:

- AddProductAsync
- DeleteProductAsync
- PutProductAsync

ImportProductsFromExcelAsync did not issue the second save.

The second service save therefore had no remaining tracked product mutation to
commit after the repository had already persisted it.

## Additional correctness finding: add duplicate guard

The same characterization exposed a functional defect in the add path.

`GetProductsByTitleAsync` returns an `IEnumerable<Producto>` materialized by
`ToListAsync()`. An empty query therefore returns an empty collection, not
`null`.

Historical ProductService used:

`if (productoExistente != null)`

That condition was true for both zero and non-zero matches, so the normal
"new product" path was blocked as if a duplicate existed.

B28 changes the guard to:

`if (productoExistente.Any())`

This preserves the duplicate rejection while allowing an empty result set to
reach ProductRepository.AddProductAsync.

The correction belongs in B28 because the add mutation cannot be meaningfully
validated without first making its no-duplicate branch reachable.

## Maintained change

Implementation commit:

`d36e8e784e68837296bea1d2fba763ba8a99800a`

### ProductService persistence

Removed exactly three calls:

`await _unitOfWork.SaveChangesAsync();`

from:

- AddProductAsync
- DeleteProductAsync
- PutProductAsync

No ProductRepository persistence call was removed.

Product mutation persistence authority is therefore:

`ProductService -> IUnitOfWork.Products -> ProductRepository -> AppDbContext.SaveChangesAsync()`

rather than a repository commit followed by a second service commit.

### Duplicate detection

Changed only the collection-presence condition from a null test to `Any()`.

No title-query semantics were changed.

## Permanent authority gate

Added:

`scripts/backend_product_persistence_authority.py`

Quality now requires:

- ProductRepository AddProductAsync to contain exactly one context save;
- UpdateProductAsync to contain exactly one context save;
- DeleteProductAsync to contain exactly one context save;
- ImportProductsFromExcelAsync to contain exactly one context save;
- ProductService to contain zero UnitOfWork SaveChangesAsync calls;
- the four maintained mutation delegation paths to remain present;
- AddProductAsync duplicate detection to use `productoExistente.Any()`;
- the historical `productoExistente != null` guard to remain absent.

Expected output:

- `product-repository-self-save-count=4`
- `product-service-second-save-count=0`
- `product-persistence-authority=ProductRepository`
- `product-add-duplicate-guard=Any`
- `backend-product-persistence-authority=clean`

## Behavioral coverage

The ProductService authority suite adds four mutation tests:

1. a title query with no matches reaches AddProductAsync exactly once and does
   not call UnitOfWork.SaveChangesAsync;
2. a real matching product blocks AddProductAsync and does not commit;
3. deleting an existing product delegates persistence to ProductRepository and
   performs no second UnitOfWork save;
4. updating an existing product preserves the response/data mutation contract,
   delegates repository persistence once and performs no second UnitOfWork
   save.

The StubUnitOfWork now counts SaveChanges calls explicitly, so future
reintroduction of a second service commit is behaviorally visible in addition
to being rejected by the static authority gate.

Backend maintained test count increases from 34 to 38.

## Lab validation

Quality:

`35562381860` — success

Current-tree security:

`35562381847` — success

Observed:

- `product-repository-self-save-count=4`
- `product-service-second-save-count=0`
- `product-persistence-authority=ProductRepository`
- `product-add-duplicate-guard=Any`
- frontend tests: 60/60
- backend tests: 38/38
- Release compiler warnings: 0
- npm audit findings: 0
- NuGet vulnerability audit: clean

## Explicit non-goals

B28 does not:

- move all repositories to UnitOfWork-owned commit semantics;
- modify OrderService or SaleService persistence;
- remove IUnitOfWork.SaveChangesAsync;
- change transaction APIs;
- alter ProductRepository database queries;
- change product schema or migrations;
- change controller routes or HTTP response shapes;
- change Excel import persistence semantics.

Those broader transaction-design questions need their own characterization
because the current backend overwhelmingly self-persists inside repositories.

## Rollback boundary

The B28 persistence portion is reversible by restoring three service-level
UnitOfWork saves.

The duplicate-guard correction is separately reversible by restoring the old
null condition, although doing so intentionally reintroduces the characterized
blocked-add behavior.

No schema, data, external-service or deployment migration is involved.

Temporary characterization workflow and lab triggers are removed before
carrier promotion.


## Carrier promotion evidence

The cleaned B28 tree was promoted by non-forced fast-forward.

Promoted carrier SHA:

`e690ebc76a0d6ea1aa87eec105ca44aa6b1587e8`

Exact-SHA carrier validation:

- Quality `35562491359` — success
- Current-tree security `35562491355` — success
- frontend tests: 60/60
- backend tests: 38/38
- Release compiler warnings: 0
- npm audit findings: 0
- NuGet vulnerability audit: clean
- `product-repository-self-save-count=4`
- `product-service-second-save-count=0`
- `product-persistence-authority=ProductRepository`
- `product-add-duplicate-guard=Any`
- `backend-product-persistence-authority=clean`

A documentation-only closure commit follows this promotion evidence. B28 is
closed only after permanent Quality and Current-tree security pass again on that
exact closure SHA.
