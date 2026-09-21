# B32 — Like repository authority

## Goal

B31 bounded the remaining non-repository AppDbContext consumers to exactly:

- ContactoController
- ImageService
- LikeService

B32 audits LikeService first because it performs pure data-access work:

- query whether a user/product like exists;
- insert a like;
- delete a like;
- query the products liked by a user.

The goal is to separate persistence from the service without changing the
controller contract, HTTP behavior or database schema.

## Starting authority

Carrier before B32:

`161546956f7bebdecf7ad56f629ff4abd882ca87`

B31 closure:

- Quality `35596992166` — success
- Current-tree security `35596992195` — success
- frontend tests: 60/60
- backend tests: 44/44
- Release compiler warnings: 0
- npm audit: clean
- NuGet vulnerability audit: clean
- outside-repository direct-context consumers: 3
- outside-repository save sites: 6

## Characterization

Disposable characterization commit:

`6ae82d2245ebed7a5a355f36344cf05dfe7c1d9e`

Characterization run:

`35597700460` — success

Exact-tree findings:

- LikeService direct AppDbContext dependency: present
- LikeService save sites: 2
- LikeService references to `_context.Likes`: 5
- LikeService references to `_context.Productos`: 1
- IUnitOfWork Likes surface: absent
- UnitOfWork LikeRepository ownership: absent
- IUnitOfWork test implementations requiring adaptation: 2

The two test implementations were the existing ProductService and
Order/Sale authority stubs.

## Concurrency/integrity finding

The model snapshot does not contain a unique composite index on:

`(UserId, ProductoId)`

The historical add flow is therefore:

1. query for an existing like;
2. if none is observed, insert;
3. save.

That prevents ordinary sequential duplicates but does not provide a
database-level guarantee against two concurrent requests both passing the
existence check.

B32 does **not** add a migration or unique constraint. Doing so safely requires
first checking existing production data for duplicates and defining conflict
behavior. The risk is documented separately rather than hidden inside this
repository extraction.

## Maintained change

### Production extraction

Commit:

`5c01c9957f5e504d3b313ec90bd314e5d56b317d`

Added:

- `ILikeRepository`
- `LikeRepository`

LikeRepository now owns the exact historical data-access behavior:

- `AddLikeAsync`
- `RemoveLikeAsync`
- `GetUserLikesAsync`

The two mutation paths each retain one `AppDbContext.SaveChangesAsync()`.

No additional save or transaction was introduced.

### UnitOfWork ownership

Added to IUnitOfWork:

`ILikeRepository Likes { get; }`

UnitOfWork now lazily owns:

`public ILikeRepository Likes => _likeRepository ??= new LikeRepository(_context);`

LikeRepository is intentionally **not** registered directly in DI.

This matches the established UnitOfWork ownership model for Products, Orders,
Sales, Categories, ProductCategories and Carts.

### LikeService

LikeService now depends only on `IUnitOfWork`.

Its public contract remains unchanged:

- `AddLike(string userId, int productoId)`
- `RemoveLike(string userId, int productoId)`
- `GetUserLikes(string userId)`

Each method delegates to `IUnitOfWork.Likes`.

The controller therefore requires no changes.

## Permanent authority gates

Authority commit:

`4a079d5723a2817625c0c35e120e33052e93ae76`

Added:

`scripts/backend_like_repository_authority.py`

The gate requires:

- LikeService has no AppDbContext or `_context`;
- LikeService delegates all three paths through `IUnitOfWork.Likes`;
- ILikeRepository exposes the three maintained operations;
- LikeRepository has exactly two mutation save sites;
- LikeRepository retains Like and Product query paths;
- IUnitOfWork exposes Likes;
- UnitOfWork lazily owns LikeRepository;
- no direct DI registration for ILikeRepository appears.

It also reports the current schema fact:

`like-user-product-unique-index=False`

and explicitly labels that concurrency constraint as not addressed in B32.

## Existing repository ownership gate

`backend_repository_di_authority.py` was extended so LikeRepository joins the
UnitOfWork-owned set.

Expected ownership output now includes:

`unitofwork-owned-repositories=Products,Orders,Sales,Categories,ProductCategories,Carts,Likes`

and:

`likeservice-like-repository-authority=IUnitOfWork.Likes`

## Direct-context boundary reduction

B31's direct-context gate and runtime reflection test were updated.

After B32, the complete allowed non-repository AppDbContext consumer set is:

- ContactoController
- ImageService

LikeService is no longer allowed to regain direct context access.

Outside-repository save distribution becomes:

- ContactoController: 1
- ImageService: 3

Total:

`outside-repository-save-site-count=4`

This is a reduction from 6 before B32.

## Behavioral coverage

Coverage commit:

`64f0a1f0be689a26fd8df73ea6dcce63126072ba`

Added:

`LikeServiceRepositoryAuthorityTests.cs`

Four tests prove:

1. AddLike delegates user/product arguments to IUnitOfWork.Likes and preserves
   the boolean result.
2. RemoveLike delegates arguments and preserves the false/true contract.
3. GetUserLikes preserves the repository-provided product list.
4. LikeService's only constructor dependency is IUnitOfWork.

The two existing IUnitOfWork test doubles were updated only to expose the new
Likes repository property.

Backend maintained test count increases from 44 to 48.

## Lab validation

Quality:

`35598182138` — success

Current-tree security:

`35598182303` — success

Observed:

- `unitofwork-owned-repositories=Products,Orders,Sales,Categories,ProductCategories,Carts,Likes`
- `likeservice-like-repository-authority=IUnitOfWork.Likes`
- `likeservice-direct-context=absent`
- `likerepository-save-site-count=2`
- `like-user-product-unique-index=False`
- `outside-repository-context-consumer-count=2`
- `outside-repository-save-site-count=4`
- `outside-repository-save-distribution=ContactoController.cs:1,ImageService.cs:3`
- frontend tests: 60/60
- backend tests: 48/48
- Release compiler warnings: 0
- npm audit findings: 0
- NuGet vulnerability audit: clean

## Resulting architecture

The maintained path is now:

`LikesController -> ILikeService -> LikeService -> IUnitOfWork.Likes -> LikeRepository -> AppDbContext`

The controller remains responsible for authentication/HTTP semantics.

LikeService remains the application-facing abstraction.

LikeRepository owns persistence and queries.

UnitOfWork owns repository construction over the shared scoped AppDbContext.

## Explicit non-goals

B32 does not:

- change LikesController routes or response messages;
- add a unique database constraint;
- deduplicate existing Like rows;
- change Like entity shape;
- change User/Product relationships;
- introduce a direct ILikeRepository DI registration;
- modify ImageService;
- modify ContactoController;
- change authentication or authorization;
- change schema or migrations.

## Follow-up concurrency candidate

A later isolated block can characterize and, if safe, add a unique
`(UserId, ProductoId)` constraint.

Before that change, the migration path should prove:

1. whether duplicate rows exist;
2. how duplicates should be reconciled;
3. how AddLike maps a uniqueness violation back to its historical
   `false` contract;
4. whether the generated SQL Server index length/nullability is acceptable.

This remains deliberately outside B32.

## Rollback boundary

Rollback requires:

1. restoring AppDbContext injection and historical queries in LikeService;
2. removing Likes from IUnitOfWork/UnitOfWork;
3. deleting ILikeRepository/LikeRepository;
4. restoring LikeService to the direct-context allowlist.

No data/schema migration is involved.

Temporary lab triggers are removed before carrier promotion.


## Carrier promotion evidence

The cleaned B32 tree was promoted by non-forced fast-forward.

Promoted carrier SHA:

`28878ec087d80132329afe7158b3a837517a0d11`

Exact-SHA carrier validation:

- Quality `35598468670` — success
- Current-tree security `35598468653` — success
- frontend tests: 60/60
- backend tests: 48/48
- Release compiler warnings: 0
- npm audit findings: 0
- NuGet vulnerability audit: clean
- `unitofwork-owned-repositories=Products,Orders,Sales,Categories,ProductCategories,Carts,Likes`
- `likeservice-like-repository-authority=IUnitOfWork.Likes`
- `likeservice-direct-context=absent`
- `likerepository-save-site-count=2`
- `like-user-product-unique-index=False`
- `outside-repository-context-consumer-count=2`
- `outside-repository-save-site-count=4`
- `outside-repository-save-distribution=ContactoController.cs:1,ImageService.cs:3`
- `backend-like-repository-authority=clean`
- `backend-direct-context-persistence-authority=clean`

A documentation-only closure commit follows this evidence. B32 is closed only
after permanent Quality and Current-tree security pass again on that exact
closure SHA.
