# B1 — Reconstruct the intended final integrated source

## Why reconstruction is required

B0 proved that historical `main` was not a coherent integration result.

The final 2024 merge directly combined:

- backend/integration parent `ac10f8007217685379460c0222ec9ced892199ed`;
- frontend parent `00692d5492dc21df8e3e86054d41bb980fc14564`.

The merge then lost required files from both domains and committed conflict
markers in the frontend manifests.

B1 repairs that integration defect without redesigning product behavior.

## Frontend authority

The complete `Frontend/antigal.client` tree is reconstructed from final
frontend parent `00692d5...`.

One maintenance-only difference is then applied to its lockfile:

- `package.json` remains the exact historical parent blob;
- npm 11 adds 272 lockfile lines for platform-specific optional packages that
  newer `npm ci` requires;
- direct dependency and devDependency declarations remain unchanged.

The npm 11-normalized lock was generated and validated only in disposable lab
`modernize/b1-integration-reconstruction-lab`.

Exact lab candidate:

`48db9497105d9948ad2766f2aa6e67aad7e1dced`

Validation run:

`35379502085` — success.

The lab proves on Node `24.20.0` / npm `11.19.0`:

- manifest/lock root declarations match;
- `npm ci` succeeds;
- CRA production build succeeds.

## Backend authority

The final merge retained all surviving backend source files bit-for-bit from
`ac10f8...`, but lost 21 paths.

B1 restores only the 16 missing **source-authority** files required by the
integrated application:

- models and DTOs;
- `ProductoCategoria` relationship;
- `IImageService`;
- product/category validators.

It deliberately does **not** restore:

- historical publish profile;
- `launchSettings.json`;
- `*.csproj.user`;
- generated `bin/` / `obj/`;
- historical `.http` scratch file;
- local/editor state.

The reconstructed backend restores and builds successfully on .NET 8 in the
same exact-head lab run.

## Security composition

B1 is layered on top of B0-S.

Therefore the reconstructed product still keeps:

- sanitized tracked `appsettings.json`;
- environment-backed secret boundary;
- no tracked `bin/` / `obj/`;
- no tracked user project state;
- permanent security baseline guard.

No historical credential value is restored.

## Product-boundary rule

This commit is restoration, not feature authorship.

Historical files are restored as exact collaborative blobs from the direct
parents of the broken merge. New 2026 work is limited to integration repair,
security containment, documentation and subsequent maintenance.
