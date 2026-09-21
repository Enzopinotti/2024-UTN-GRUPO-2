# B33 — Contacto persistence boundary

## Goal

B32 reduced the complete non-repository `AppDbContext` consumer set to:

- `ContactoController`
- `ImageService`

B33 removes persistence from the controller while preserving the existing HTTP and authorization contract.

## Starting authority

Main before B33:

`1c1e73e1572c1d60dda9ad22658f0c635303a3a1`

B32 checkpoint:

- frontend: 60/60
- backend: 48/48
- Release compiler warnings: 0
- npm audit: clean
- NuGet vulnerability audit: clean
- non-repository AppDbContext consumers: 2
- outside-repository SaveChanges sites: 4

## Characterization

The historical `ContactoController` owned all three data-access paths directly:

1. POST added a Contacto, assigned `Fecha = DateTime.Now`, and saved once.
2. GET listed all Contactos.
3. GET by id used `FindAsync` and returned 404 when absent.

The controller also carried the authorization boundary:

- controller-wide `[Authorize(Roles = "Admin")]`
- POST override with `[AllowAnonymous]`

No schema change is required for the extraction.

## Maintained architecture

The maintained path is now:

`ContactoController -> IContactoService -> ContactoService -> IUnitOfWork.Contactos -> ContactoRepository -> AppDbContext`

### Controller

`ContactoController` now depends only on `IContactoService`.

It no longer imports or injects `AppDbContext`.

The following HTTP/auth behavior remains protected:

- Admin authorization remains controller-wide.
- POST remains anonymous.
- POST still returns `CreatedAtAction(nameof(GetContacto), ...)`.
- GET list remains unchanged.
- GET by id still returns NotFound when the service returns null.

### Service

`ContactoService` depends only on `IUnitOfWork`.

It owns the application behavior that existed in the controller:

`contacto.Fecha = DateTime.Now`

and delegates storage/query work through `IUnitOfWork.Contactos`.

### Repository

Added:

- `IContactoRepository`
- `ContactoRepository`

Repository operations:

- `AddAsync`
- `GetAllAsync`
- `GetByIdAsync`

The write path contains exactly one `SaveChangesAsync`.

`ContactoRepository` is UnitOfWork-owned and is intentionally not registered directly in DI.

## UnitOfWork authority

`IUnitOfWork` now exposes:

`IContactoRepository Contactos { get; }`

`UnitOfWork` lazily owns the implementation:

`public IContactoRepository Contactos => _contactoRepository ??= new ContactoRepository(_context);`

The UnitOfWork-owned repository set becomes:

`Products,Orders,Sales,Categories,ProductCategories,Carts,Likes,Contactos`

## Permanent gates

Added:

`scripts/backend_contacto_repository_authority.py`

It requires:

- no direct AppDbContext state in ContactoController;
- preserved controller auth/route/CreatedAtAction contracts;
- ContactoService repository delegation;
- preserved DateTime.Now Fecha authority;
- the three repository operations;
- exactly one repository mutation save site;
- UnitOfWork ownership;
- no direct IContactoRepository DI registration;
- ContactoService DI registration.

Existing repository ownership and direct-context gates were tightened as well.

After B33 the complete allowed non-repository AppDbContext set is exactly:

`ImageService`

and the complete outside-repository save distribution is:

`ImageService: 3`

Therefore there are now **zero controllers** with direct AppDbContext access.

## Tests

Added:

`ContactoServiceRepositoryAuthorityTests.cs`

Four maintained tests cover:

1. create assigns Fecha and delegates the same Contacto;
2. GetAll delegates and preserves the repository list;
3. GetById delegates the id and preserves the result;
4. ContactoService's only constructor dependency is IUnitOfWork.

Backend maintained test count increases from 48 to 52.

## PR validation

PR #17 head:

`26a6b13f4824fd563e26fc9123f349ac91b7b335`

Quality:

`35603716493` — success

Current-tree security:

`35603716532` — success

Observed:

- frontend: 60/60
- backend: 52/52
- Release compiler warnings: 0
- npm audit: 0 findings
- production npm audit: 0 findings
- NuGet vulnerability audit: clean
- `backend-contacto-repository-authority=clean`
- `contactocontroller-direct-context=absent`
- `contactocontroller-http-contract=preserved`
- `outside-repository-context-consumer-count=1`
- `outside-repository-context-consumers=Backend/antigal.server/Services/ImageService.cs`
- `outside-repository-save-site-count=3`

## Main promotion

B33 was promoted to main by non-forced fast-forward.

Published main SHA:

`26a6b13f4824fd563e26fc9123f349ac91b7b335`

Exact published-SHA validation:

- Quality `35603960458` — success
- Current-tree security `35603960486` — success

PR #17 is recorded by GitHub as merged at the same commit SHA; no synthetic merge commit was introduced.

## Explicit non-goals

B33 does not:

- alter Contacto schema or migrations;
- change frontend contact behavior;
- change Contacto authorization policy;
- replace DateTime.Now with another time authority;
- modify ImageService;
- add Likes uniqueness constraints;
- change production bootstrap behavior.

## Result

B33 closes controller-level persistence leakage completely.

The remaining deliberate non-repository direct-context exception is now only ImageService, where external Cloudinary behavior and relational updates should be characterized together before extraction.
