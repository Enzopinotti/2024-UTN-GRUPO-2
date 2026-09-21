# B31 — Direct AppDbContext persistence authority

## Goal

B30 retired the unused UnitOfWork SaveChanges surface and made repository/direct
AppDbContext persistence the explicit backend write model.

B31 audits the remaining production writes that happen outside repository
classes instead of assuming they should all be migrated at once.

The goals are:

1. inventory every direct AppDbContext consumer outside repositories;
2. remove measured redundant persistence inside ImageService;
3. make the remaining direct-context boundary explicit and gated;
4. avoid inventing new repository abstractions without behavior-driven need.

## Starting authority

Carrier before B31:

`0140098a754127bdb94f00931c5c0ae1af154301`

B30 closure:

- Quality `35595633294` — success
- Current-tree security `35595633363` — success
- frontend tests: 60/60
- backend tests: 43/43
- Release compiler warnings: 0
- npm audit: clean
- NuGet vulnerability audit: clean
- UnitOfWork SaveChanges surface: absent
- transaction authority: preserved
- direct production AppDbContext save sites: 32

## Characterization

Disposable characterization commit:

`2a6551b0bb409a9f09d2426c43ff3578234f77ae`

Characterization run:

`35596339947` — success

Exact-tree findings:

- direct AppDbContext constructor consumers: 12
- repository/direct infrastructure consumers: 9
- non-repository direct-context consumers: 3
- total direct context save sites: 32
- repository save sites: 23
- save sites outside repositories: 9

The exact outside-repository distribution was:

- `ImageService.cs`: 6
- `LikeService.cs`: 2
- `ContactoController.cs`: 1

No fourth non-repository persistence consumer existed.

## ImageService finding

ImageService had three mutating public operations:

- UploadImageAsync
- DeleteImageAsync
- DeleteImageByUrlAsync

Delete-by-id and delete-by-url each had one database flush.

UploadImageAsync contained four source-level save sites:

1. save Imagen immediately after Cloudinary upload;
2. save again if a Producto URL list was updated;
3. save again if a User image URL was updated;
4. save again if a Categoria image URL was updated.

Because those three association branches are mutually exclusive, a related
upload could perform two EF flushes during one service operation.

There is no behavior need for the first flush before the related entity change.
The new Imagen and the related tracked entity can be persisted together in one
EF SaveChangesAsync call.

Cloudinary remains an external side effect before the database write in both
the historical and maintained designs; B31 does not claim cross-system
transactionality.

## Maintained change

Primary implementation commit:

`8d625c6467ab67bc164bc3c525ad6ac8beb2dcf1`

Warning-free test correction:

`6d01d06063c926dbbbe8020b57eac4e518ae87fb`

### ImageService UploadImageAsync

The method now:

1. uploads the file to Cloudinary;
2. creates and tracks Imagen;
3. applies the mutually exclusive Producto/User/Categoria association update;
4. calls AppDbContext.SaveChangesAsync exactly once;
5. returns the created Imagen.

The resulting database persistence boundary is one flush for the entire local
database mutation set.

### Delete paths

DeleteImageAsync remains one save.

DeleteImageByUrlAsync remains one save when a matching database Imagen exists.

No Cloudinary delete semantics were changed.

### LikeService and ContactoController

B31 does not migrate these components to new repositories.

They remain explicitly allowed direct-context consumers:

- LikeService: two save sites, matching AddLike and RemoveLike;
- ContactoController: one save site, matching PostContacto.

This is intentional scope control. Future work can evaluate each boundary
separately instead of using B31 to create architecture-only abstractions.

## Permanent authority gate

Added:

`scripts/backend_direct_context_persistence_authority.py`

Quality now requires the exact non-repository AppDbContext consumer allowlist:

- ContactoController
- ImageService
- LikeService

It also requires the outside-repository save distribution to remain:

- ContactoController: 1
- ImageService: 3
- LikeService: 2

For ImageService specifically, each mutating public operation must contain
exactly one SaveChangesAsync site.

The upload gate also checks that:

- Imagen is tracked;
- Producto/User/Categoria association mutations remain present;
- all those database mutations occur before the single persistence flush.

Expected output:

- `outside-repository-context-consumer-count=3`
- `outside-repository-save-site-count=6`
- `image-service-upload-save-site-count=1`
- `image-service-delete-id-save-site-count=1`
- `image-service-delete-url-save-site-count=1`
- `image-service-db-flush-authority=one-per-mutating-operation`
- `backend-direct-context-persistence-authority=clean`

## Runtime boundary test

Added:

`DirectContextBoundaryTests.cs`

The test reflects over the production assembly and asserts that the complete
non-repository constructor-level AppDbContext consumer set is exactly:

- ContactoController
- ImageService
- LikeService

This makes accidental growth of direct database coupling visible at both the
runtime-contract and source-authority levels.

Backend maintained test count increases from 43 to 44.

## CI hardening during the lab

The first runtime-boundary fixture compiled but produced CS8602 because nullable
flow analysis does not carry a previous LINQ predicate into the next predicate
for Type.Namespace.

Quality correctly rejected that warning.

The fixture-only correction commit:

`6d01d06063c926dbbbe8020b57eac4e518ae87fb`

changed the reflection filter to a null-safe Namespace predicate. No production
code changed in that correction.

## Final lab validation

Quality:

`35596586057` — success

Current-tree security:

`35596586095` — success

Observed:

- outside-repository context consumers: 3
- outside-repository save sites: 6
- ImageService upload save sites: 1
- ImageService delete-by-id save sites: 1
- ImageService delete-by-url save sites: 1
- frontend tests: 60/60
- backend tests: 44/44
- Release compiler warnings: 0
- npm audit findings: 0
- NuGet vulnerability audit: clean

## Resulting persistence shape

After B31:

- repository save sites remain untouched;
- ImageService uses one local DB flush per mutating public operation;
- LikeService remains a bounded direct-context service;
- ContactoController remains the sole bounded direct-context controller;
- no new non-repository AppDbContext consumer can appear unnoticed.

The direct-context surface is therefore constrained rather than implicitly
sprawling.

## Explicit non-goals

B31 does not:

- create IImageRepository;
- create ILikeRepository;
- create ContactoService/Repository;
- remove AppDbContext from ImageService;
- remove AppDbContext from LikeService;
- remove AppDbContext from ContactoController;
- change Cloudinary request ordering;
- make Cloudinary and SQL participate in one distributed transaction;
- modify schema, routes or payloads.

Those remaining direct-context boundaries are now small enough to audit
individually in later blocks.

## Rollback boundary

The production rollback is limited to moving ImageService's first
SaveChangesAsync back immediately after adding Imagen and restoring the three
association-branch save calls.

No schema, data or deployment migration is involved.

Temporary characterization workflow and lab branch triggers are removed before
carrier promotion.


## Carrier promotion evidence

The cleaned B31 tree was promoted by non-forced fast-forward.

Promoted carrier SHA:

`287c8816952b5fe0f18f77e0757afc6b96e59d5f`

Exact-SHA carrier validation:

- Quality `35596817096` — success
- Current-tree security `35596816952` — success
- frontend tests: 60/60
- backend tests: 44/44
- Release compiler warnings: 0
- npm audit findings: 0
- NuGet vulnerability audit: clean
- `outside-repository-context-consumer-count=3`
- `outside-repository-save-site-count=6`
- `image-service-upload-save-site-count=1`
- `image-service-delete-id-save-site-count=1`
- `image-service-delete-url-save-site-count=1`
- `backend-direct-context-persistence-authority=clean`

A documentation-only closure commit follows this evidence. B31 is closed only
after permanent Quality and Current-tree security pass again on that exact
closure SHA.
