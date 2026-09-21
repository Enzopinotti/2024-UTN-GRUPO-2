# B12 — Native Sass source authority

B12 starts from the completed B11 React 19 carrier:

`b02f0e39420baa43d7f22391ca24cbea52c6b07d` — `B11: close React 19 runtime modernization`.

The B12 laboratory is developed on:

`modernize/b12-sass-authority-lab`.

## Objective

Make the maintained SCSS tree the only versioned stylesheet source authority and let Vite compile Sass directly.

B12 must not redesign the UI or alter intended visual semantics. It removes duplicated generated-state authority and modernizes only the Sass syntax required to keep the native pipeline warning-free.

## Previous style authority

Before B12 the frontend kept two parallel style authorities:

- source SCSS under `src/styles/scss`;
- generated CSS and source maps under `src/styles/css`.

Development also ran two parallel processes:

- a standalone `sass --watch` process;
- Vite.

The package graph therefore carried `concurrently` only to coordinate the historical Sass watcher.

The tracked generated style set contained:

- `src/styles/css/index.css`;
- `src/styles/css/index.css.map`;
- `src/styles/css/main.css`;
- `src/styles/css/main.css.map`;
- `src/styles/css/components/resetearContrasenia.css.map`.

## Native Sass composition

B12 changes the frontend entrypoint from:

`./styles/css/index.css`

to:

`./styles/scss/index.scss`.

Vite 8.2.2 and the maintained `sass` development dependency now own Sass compilation.

The legacy `sass` watcher script is removed, `dev` becomes plain `vite`, and `concurrently` is removed from package and lockfile authority.

The retired `src/styles/css/` path is ignored so generated styles cannot silently return to version control.

A permanent `scripts/style_authority.mjs` gate rejects:

- a second standalone Sass watcher;
- `concurrently` returning to package or lock authority;
- entrypoint references to generated CSS;
- tracked generated CSS under `src/styles/css`;
- disappearance of the maintained Sass source tree.

## Slick stylesheet deduplication

The first native-Sass probe exposed an incompatible `@charset` merge between Sass output and `slick-theme.css`.

The investigation also proved the slick styles were duplicated:

- globally from `src/styles/scss/index.scss`;
- route-scoped from `src/pages/TiendaFisica.jsx`.

B12 removes the global Sass imports and preserves the existing `TiendaFisica` imports.

This keeps slick styling attached to the route that uses `react-slick` and removes it from the initial stylesheet.

The measured production CSS changes from the B11 baseline:

- B11 entry CSS: 150.65 kB in Vite output;
- B12 entry CSS: 137.36 kB in Vite output / 134.15 KiB by file size;
- B12 `TiendaFisica` CSS: 13.29 kB in Vite output / 12.98 KiB by file size.

The size shift is therefore explained by moving the duplicated slick CSS out of the global entry bundle, not by dropping application styles.

## Dart Sass modernization

Direct Sass compilation exposed deprecations that the precompiled CSS path had hidden.

B12 replaces legacy color helpers with the module API:

- `darken(...)` → `color.adjust(..., $lightness: -...)`;
- `lighten(...)` → `color.adjust(..., $lightness: ...)`.

The final remaining global builtin:

`map-get(...)`

is replaced with:

`map.get(...)`.

The maintained SCSS now imports `sass:color` and `sass:map` where required.

The final native Sass build emits zero deprecation warnings.

## Validated laboratory evidence

Final B12 laboratory run:

`35546114012` — success on the final lab composition (including the permanent Quality contract and this B12 documentation).

Validated signals:

- exact Node 24.20.0 toolchain;
- reproducible npm lock reconciliation;
- production dependency authority clean;
- JSX extension authority clean;
- dead-source authority clean;
- native Sass authority clean;
- frontend lint: zero warnings;
- frontend tests: 13 / 13 files green;
- frontend tests: 60 / 60 tests green;
- Vite native Sass build green;
- entry stylesheet: 134.15 KiB;
- route-scoped `TiendaFisica` stylesheet: 12.98 KiB;
- Sass deprecation warnings: 0;
- backend build green;
- backend tests: 16 / 16 green;
- current-tree security baseline green;
- npm full graph: 0 advisories;
- npm production graph: 0 advisories;
- `b12_native_sass_bundle_boundary=accepted`;
- `b12_dependency_security_boundary=accepted`.

The validated lockfile was committed as:

`a02ed998934e4c39d49052ce40ae4e21a4ec0866` — `B12 lab: reconcile lock after Sass watcher retirement`.

## Permanent Quality boundary

B12 extends the permanent Quality workflow to require:

- `dev` and `start` remain Vite-owned;
- no standalone `sass` watcher script;
- no `concurrently` package or lock authority;
- direct SCSS entrypoint authority;
- no tracked generated CSS authority;
- native Sass authority gate;
- entry CSS between 132 and 136 KiB;
- route-scoped `TiendaFisica` CSS between 12 and 14 KiB;
- zero Sass deprecation warnings.

Existing B11 and earlier runtime, test, lint, backend, security and zero-advisory gates remain unchanged.

## Scope boundary

B12 does not redesign layouts, colors, typography, breakpoints or component styling.

The color-function changes preserve the legacy `darken` / `lighten` semantics through equivalent `color.adjust` lightness deltas.

Any future visual redesign or deeper Sass architecture cleanup belongs in a separate block.

## Closure

B12 was promoted to the maintenance carrier as:

`020ae03c6898dbacb9c79c697a2bc1e060086c57` — `B12: adopt native Sass source authority`.

The promotion was composed directly on top of the B11 carrier and copied only validated B12 product, authority, Quality and documentation blobs. The temporary `.github/workflows/b12-sass-authority.yml` laboratory workflow was deliberately excluded from the carrier tree.

Carrier validation for the promotion:

- Quality run `35546207410`: success;
- Current-tree security baseline run `35546207384`: success;
- frontend lint: zero warnings;
- frontend tests: 60 / 60 green;
- native Sass bundle boundary: accepted;
- Sass deprecation warnings: 0;
- backend compiler warnings: 0;
- backend tests: 16 / 16 green;
- npm full graph: 0 advisories;
- npm production graph: 0 advisories.

B12 is therefore closed on the carrier. Native SCSS is the maintained stylesheet authority; generated CSS and the parallel Sass watcher are no longer part of the repository contract.
