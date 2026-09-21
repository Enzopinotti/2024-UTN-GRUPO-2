# B36 — Like concurrency integrity

## Goal

B32 moved favorite persistence behind `LikeRepository`, but the maintained add flow still used a classic check-then-insert sequence:

1. query whether `(UserId, ProductoId)` exists;
2. if absent, insert;
3. save.

Two concurrent requests can both observe absence before either save completes. The application-level check therefore did not provide a database invariant.

B36 closes that race at the schema and repository levels while preserving the public controller behavior: a favorite that already exists returns `false`, which the controller maps to the existing "already in favorites" response.

## Starting checkpoint

Main before B36:

`e141ae75233c57eac7500050b3252454d35d4d95`

B35 closure:

- frontend: 60/60
- backend: 58/58
- Release compiler warnings: 0
- npm audit: clean
- NuGet vulnerability audit: clean
- non-repository AppDbContext consumers: 0
- outside-repository SaveChanges sites: 0

## Data characterization boundary

The repository does not provide an authoritative live production database from which duplicate counts can be measured.

B36 therefore does **not** claim that historical duplicates existed or did not exist.

Instead, the migration is written to be safe for both states:

- zero existing duplicates;
- one or more duplicate authenticated favorites.

The cleanup is deterministic and executes before the unique index is created.

## Schema authority

`AppDbContext` now declares a unique filtered composite index:

`(UserId, ProductoId)`

with filter:

`[UserId] IS NOT NULL`

This choice is deliberate.

Authenticated API writes always supply a non-null user id, so those rows now receive the invariant.

Historical/null-user rows are not newly constrained, avoiding an unrelated data-policy change in B36.

## Migration

Added:

`20260921141500_LikeConcurrencyIntegrity`

Before creating the unique index, its `Up` migration:

1. considers only rows with non-null `UserId`;
2. partitions by `UserId, ProductoId`;
3. orders each partition by `Id`;
4. keeps the oldest row;
5. deletes rows with row number greater than one;
6. creates `IX_Likes_UserId_ProductoId` as a unique filtered index.

The cleanup and index creation run as ordered migration operations.

`Down` removes the index. It does not recreate deleted duplicate rows; that historical cleanup is intentionally irreversible.

## Runtime add authority

`LikeRepository.AddLikeAsync` no longer performs a pre-insert existence query.

The maintained flow is:

1. construct and track the Like;
2. attempt one `SaveChangesAsync`;
3. return true on success;
4. on `DbUpdateException`, detach the failed insert;
5. query whether that exact `UserId, ProductoId` pair now exists;
6. return false if it does;
7. rethrow if it does not.

This handles the loser of a concurrent duplicate insert while refusing to silently swallow unrelated database failures.

The controller/service contract remains unchanged.

## Relational tests

Added `Microsoft.EntityFrameworkCore.Sqlite 10.0.12` to the test project only.

Added `LikeRepositoryConcurrencyTests` with four relational tests:

1. the EF model exposes the unique filtered composite index;
2. an existing pair produces a real uniqueness violation and `AddLikeAsync` returns false;
3. an unrelated foreign-key/database failure is rethrown;
4. duplicate historical rows with null `UserId` remain allowed by the filter.

These tests exercise relational constraint behavior rather than relying on EF's non-relational InMemory provider.

## EF migration runtime proof

A second B36 validation cut added `LikeMigrationRuntimeTests`.

It proves without a live database that:

- EF runtime discovers the historical migration;
- EF runtime discovers `20260921141500_LikeConcurrencyIntegrity`;
- SQL Server `IMigrator.GenerateScript` can materialize the migration;
- generated SQL contains duplicate cleanup;
- generated SQL creates the unique favorite index;
- cleanup appears before index creation.

This catches migration discovery/designer errors that a normal C# build alone would not detect.

## Permanent gates

Added:

`scripts/backend_like_concurrency_authority.py`

It protects:

- model unique/filter authority;
- snapshot authority;
- migration id and designer authority;
- deterministic deduplication SQL;
- deduplication-before-index ordering;
- unique filtered index creation and rollback;
- insert-first repository flow;
- failed insert detachment;
- exact-pair duplicate verification;
- false result for the duplicate path;
- rethrow for unrelated DB failures;
- SQLite relational test authority;
- EF runtime migration-discovery proof;
- SQL Server migration-script proof.

The earlier Like repository gate was also tightened: a missing composite unique filtered index now fails Quality.

## Initial B36 validation

PR #23 implementation head:

`603572edb5eafb177bf4582de60d66c8293367ac`

PR validation:

- Quality `35610825069` — success
- Current-tree security `35610825660` — success
- frontend: 60/60
- backend: 62/62
- Release compiler warnings: 0
- npm audit: clean
- NuGet vulnerability audit: clean

Published implementation SHA:

`603572edb5eafb177bf4582de60d66c8293367ac`

Exact published-SHA validation:

- Quality `35611032108` — success
- Current-tree security `35611031422` — success

## Migration proof follow-up

PR #24 final head:

`84ff389ace9de20af36953ea2efd2081dd94ab71`

PR validation:

- Quality `35611203711` — success
- Current-tree security `35611203831` — success
- backend: 64/64
- `like-migration-runtime-discovery=covered`
- `like-migration-sql-generation=covered`
- `backend-like-concurrency-authority=clean`

The proof was promoted by non-forced fast-forward.

Final published B36 SHA:

`84ff389ace9de20af36953ea2efd2081dd94ab71`

Exact final published-SHA validation:

- Quality `35611361086` — success
- Current-tree security `35611361300` — success
- frontend: 60/60
- backend: 64/64
- Release compiler warnings: 0
- npm audit: 0 findings
- production npm audit: 0 findings
- NuGet vulnerability audit: clean

## Deployment boundary

B36 introduces a real database migration.

Existing databases must apply pending EF migrations before the new application code receives favorite writes.

B36 deliberately does not auto-run schema migrations during API startup; deployment-time migration execution remains an operational responsibility.

## Explicit non-goals

B36 does not:

- make `UserId` non-nullable in the model;
- add a Producto foreign key to Like;
- remove historical null-user rows;
- change LikesController routes or response messages;
- change the remove/get favorites flows;
- auto-run migrations on application startup;
- claim knowledge of live production duplicate counts.

## Result

Favorite uniqueness is now a database invariant for authenticated rows rather than a best-effort application check.

Concurrent duplicate adds converge on one persisted favorite and the existing "already present" application result instead of permitting duplicate rows or leaking a database exception.
