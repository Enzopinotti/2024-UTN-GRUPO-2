# B0 — Collaborative provenance and reproducibility baseline

## Scope

Repository: `Enzopinotti/2024-UTN-GRUPO-2`

Historical integrated `main` authority:

`b63af0a8092ae4ce69aad4013affc6159cf7fafa`

2026 maintenance carrier at B0 close:

`a03cdc5b6fe87e4202cabfa0ddf642c8b6d63fc7`

This repository is a 2024 UTN-FRLP **Desarrollo de Software** team project.
The 2026 lane is maintenance by Enzo Pinotti; it does not reattribute the
historical product to one person.

## Provenance

The 217 commits reachable from historical `main` were inspected through commit
metadata.

Observed author identities/counts:

- Matias Rau Bekerman: 73;
- Natasha Cadabon identities: 48 combined;
- Enzo Pinotti: 44;
- Patricio Borda identities: 37 combined;
- Lucio Borda: 14;
- Franco Beneforti: 1.

These are provenance counts, not a ranking of contribution value.

## Final 2024 merge anatomy

The final historical `main` commit is:

`b63af0a8092ae4ce69aad4013affc6159cf7fafa`

Its direct parents are:

- backend/integration parent `ac10f8007217685379460c0222ec9ced892199ed`;
- final frontend parent `00692d5492dc21df8e3e86054d41bb980fc14564`.

Both parents contain valid frontend `package.json` and `package-lock.json`
files.

The merge commit itself contains unresolved Git conflict markers in both
frontend manifests.

This is therefore an **integration defect introduced by the final merge**, not
evidence that the parent manifests were inherently invalid.

## Lost backend authority

Comparing the final merge with `ac10f8...` while excluding generated
`bin/` / `obj/` state:

- parent backend files: 124;
- final-main backend files: 103;
- files present in both: byte-identical;
- files lost by the merge: 21.

The missing set includes compile-authoritative types such as:

- `Models/Categoria.cs`;
- `Models/Contacto.cs`;
- `Models/Imagen.cs`;
- `Models/Role.cs`;
- `Models/Order.cs`;
- `Models/OrderItem.cs`;
- DTOs including `ResponseDto.cs` and `CreateRoleDto.cs`;
- `Relationships/ProductoCategoria.cs`;
- `Services/IImageService.cs`;
- category/product validators.

It also includes local/tooling/deployment-adjacent files that are **not**
automatically eligible for restoration, such as a publish profile and launch
settings.

### Backend reproduction

The backend declares `.NET 8` / `net8.0`.

B0 on GitHub Actions:

- `dotnet restore Backend/antigal.server.sln`: succeeds;
- `dotnet build ... -c Release --no-restore`: fails;
- failure class: missing historical source types/namespaces, primarily CS0234
  and CS0246;
- `dotnet test --no-build`: exits 0 but there is no actual test project,
  therefore this is **not** test coverage.

Representative missing authorities proven by compiler output:

- `antigal.server.Relationships`;
- `ProductoCategoria`;
- `ResponseDto`;
- `Categoria`;
- `Contacto`;
- `Role`;
- `CreateRoleDto`;
- `Imagen`;
- `IImageService`.

This matches the files proven present in the backend parent and absent from the
merge result.

## Lost frontend authority

Comparing the final merge with final frontend parent `00692d5...`:

- frontend-parent tracked files: 234;
- final-main frontend files: 137;
- approximately 97 frontend files/assets were lost;
- 42 surviving frontend files differ from the final frontend parent.

Lost files include required application modules and assets, for example:

- `src/pages/Home.js`;
- `src/components/layout/Main.js`;
- `src/contexts/CartContext.js`;
- admin dashboard/components;
- cart/checkout/product components;
- public images/icons;
- `src/App.test.js` and `src/setupTests.js`.

The integrated `App.js` still imports some of these missing modules, proving
that the final merge tree is internally incomplete.

## Frontend package authority

The final frontend parent `00692d5...` has:

- valid `package.json`;
- valid lockfile v3;
- 28 runtime dependencies;
- 8 dev dependencies;
- manifest dependencies exactly equal to lockfile root dependencies;
- manifest devDependencies exactly equal to lockfile root devDependencies.

B0 used Node 20 only as a compatibility probe; the repository had no explicit
historical Node pin, so Node 20 is **not claimed as historical authority**.

Results against the complete historical frontend parent:

- `npm ci`: succeeds;
- normal CRA production build: succeeds;
- `CI=true` build: fails because historical ESLint warnings are promoted to
  build errors;
- npm audit: 70 findings — 15 low, 18 moderate, 34 high, 3 critical.

## Test baseline

There is no maintained product test suite.

The final frontend parent contains only the default-style CRA
`src/App.test.js` plus `setupTests.js`.

The placeholder test still expects the text `learn react`, while the actual
application has long since replaced the starter UI.

B0 result:

- Jest suite: 1 failed;
- executed tests: 0;
- the suite fails before providing meaningful product coverage.

B2 must replace this placeholder with behavior contracts instead of merely
editing the assertion to make it green.

The backend has no test project.

## Dependency-security baseline

### Frontend

Historical final frontend authority:

- total npm audit findings: 70;
- low: 15;
- moderate: 18;
- high: 34;
- critical: 3.

### Backend

`dotnet list package --vulnerable --include-transitive` reports known
vulnerabilities including:

- AutoMapper 13.0.1 — high;
- MailKit 4.8.0 — moderate;
- MimeKit 4.8.0 — moderate;
- Azure.Identity 1.10.3 — moderate transitive;
- Microsoft.Build 17.8.3 — high transitive;
- SixLabors.ImageSharp 2.1.8 — high transitive.

The audit command itself exits successfully; that exit code does not mean
there are zero advisories.

## Current-tree security containment already completed

B0-S precedes this document.

Maintained 2026 authority already:

- removed 123 tracked backend `bin/` / `obj/` artifacts;
- removed tracked `*.csproj.user` state;
- emptied secret/account-specific tracked appsettings slots;
- made sanitized `appsettings.json` valid JSON;
- documented environment-variable names without values;
- added a permanent current-tree security guard.

Historical values remain in Git history. B0-S does not claim provider-side
rotation or revocation.

## B1 decision

B1 is **integration reconstruction first**, not framework modernization.

The safe reconstruction rule is:

1. restore missing backend **source authority** from exact blobs in
   `ac10f800...`;
2. do not restore generated output, user-specific IDE state or historical
   publish profiles by default;
3. reconstruct frontend from final frontend parent `00692d5...` in a
   disposable lab first;
4. preserve B0-S sanitized configuration and generated-state hygiene;
5. prove `npm ci`, frontend build, `dotnet restore` and backend build on the
   reconstructed composition;
6. only after that composition is green may it be promoted to the carrier;
7. dependency upgrades and real tests are separate subsequent blocks.

This preserves the collaborative historical sources and repairs the broken
integration rather than rewriting the product.
