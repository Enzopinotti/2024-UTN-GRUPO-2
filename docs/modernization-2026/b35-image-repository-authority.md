# B35 — Image repository authority

## Goal

B35 closes the final maintained non-repository direct `AppDbContext` dependency.

After B34 the only allowed production consumer of `AppDbContext` outside repositories was `ImageService`.

B35 preserves Cloudinary as the service-owned external integration while moving all EF query/mutation work behind a UnitOfWork-owned image repository.

## Starting checkpoint

Main before B35:

`435ab617c5f81071755f244a2794a5a3d74700ba`

B34 closure:

- frontend: 60/60
- backend: 57/57
- Release compiler warnings: 0
- npm audit: clean
- NuGet vulnerability audit: clean
- non-repository AppDbContext consumers: 1
- outside-repository SaveChanges sites: 3

The remaining consumer was exactly:

`Backend/antigal.server/Services/ImageService.cs`

## Characterized behavior

B31 had already reduced ImageService to one local database flush per mutating public operation.

The maintained operation ordering before B35 was:

### Upload

1. Cloudinary upload.
2. Create local Imagen.
3. Update one optional association using the existing precedence:
   - Producto, else
   - User, else
   - Categoria.
4. Save local database changes.

### Delete by id

1. Read Imagen from local database.
2. Return false if absent.
3. Delete from Cloudinary.
4. If Cloudinary succeeds, remove Imagen and clear the same optional association.
5. Save local database changes.

### Delete by URL

1. Extract public id from URL.
2. Delete from Cloudinary.
3. If Cloudinary succeeds, find optional local Imagen by URL.
4. If present, remove Imagen and clear association.
5. Save local database changes.
6. Return true even if no local Imagen exists, matching historical behavior.

B35 preserves those orderings. It does not claim cross-system transactionality between Cloudinary and SQL Server.

## Maintained architecture

The image path is now:

`ImageController -> IImageService -> ImageService -> IUnitOfWork.Images -> ImageRepository -> AppDbContext`

### ImageService

`ImageService` now depends on:

- `Cloudinary`
- `IUnitOfWork`

It no longer imports, injects or stores `AppDbContext`.

Cloudinary remains service-owned because it is an external integration rather than repository persistence.

### ImageRepository

Added:

- `IImageRepository`
- `ImageRepository`

Repository operations:

- `AddAsync`
- `GetByIdAsync`
- `GetByUrlAsync`
- `DeleteAsync`

The repository owns:

- Imagen persistence;
- Producto image-list association updates;
- User image URL updates;
- Categoria image URL updates.

The static mutation save surface is exactly two sites:

- one in AddAsync;
- one in DeleteAsync.

Each public image mutation still performs at most one local database flush.

### UnitOfWork

`IUnitOfWork` now exposes:

`IImageRepository Images { get; }`

`UnitOfWork` lazily owns the implementation:

`public IImageRepository Images => _imageRepository ??= new ImageRepository(_context);`

No direct `IImageRepository` DI registration is maintained.

The UnitOfWork-owned repository set now includes:

`Products, Orders, Sales, Categories, ProductCategories, Carts, Likes, Contactos, Images`

## Zero direct-context authority

The source gate and runtime reflection test were tightened from an allowlist of one to an empty set.

After B35:

- non-repository AppDbContext consumers: **0**
- non-repository SaveChangesAsync sites: **0**

Expected permanent output:

- `outside-repository-context-consumer-count=0`
- `outside-repository-save-site-count=0`
- `non-repository-appdbcontext-authority=zero`
- `non-repository-savechanges-authority=zero`
- `backend-direct-context-persistence-authority=clean`

## Image authority gate

Added:

`scripts/backend_image_repository_authority.py`

It protects:

- ImageService has no direct context;
- constructor authority is Cloudinary + IUnitOfWork;
- repository delegation for add/read/delete;
- Cloudinary remains service-owned;
- upload ordering remains Cloudinary before database persistence;
- delete-by-id ordering remains DB read -> Cloudinary -> DB delete;
- delete-by-url ordering remains Cloudinary -> optional DB read/delete;
- association mutation authority remains Producto/User/Categoria;
- ImageRepository has exactly two mutation save sites;
- UnitOfWork owns ImageRepository;
- direct image repository DI remains absent.

## Tests

Added:

`ImageServiceRepositoryAuthorityTests.cs`

The maintained runtime boundary test was also tightened to require an empty non-repository AppDbContext consumer set.

Backend maintained test count increases from 57 to 58.

## PR validation

PR #21 head:

`973465b4c1ef62ddbcfd85182504858d699408ec`

Quality:

`35605583110` — success

Current-tree security:

`35605583001` — success

Observed:

- frontend: 60/60
- backend: 58/58
- Release compiler warnings: 0
- npm audit: 0 findings
- production npm audit: 0 findings
- NuGet vulnerability audit: clean
- `imageservice-direct-context=absent`
- `backend-image-repository-authority=clean`
- `outside-repository-context-consumer-count=0`
- `outside-repository-save-site-count=0`

## Main promotion

B35 was promoted by non-forced fast-forward.

Published main SHA:

`973465b4c1ef62ddbcfd85182504858d699408ec`

Exact published-SHA validation:

- Quality `35605758328` — success
- Current-tree security `35605758293` — success

## Explicit non-goals

B35 does not:

- change ImageController routes or payloads;
- change Cloudinary credentials/configuration;
- add a distributed transaction;
- compensate Cloudinary if SQL persistence fails;
- alter image schema or migrations;
- change Producto/User/Categoria association precedence;
- change Likes persistence or schema.

## Result

The backend persistence boundary is now materially simpler than the historical design:

**all maintained AppDbContext access is repository-owned.**

No service or controller may regain direct context state unnoticed because both source and runtime gates require the outside-repository set to remain empty.
