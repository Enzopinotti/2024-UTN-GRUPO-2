# B8b — Route-level code splitting and entry chunk boundary

B8b starts from carrier:

`67194d911bf4503f058745fe8e6049ce98f99f53` — B8a native empty-cart animation and direct-eval-free frontend.

At that boundary the maintained Vite build was green and warning-free for direct `eval`, but the initial JavaScript bundle still emitted Vite's chunk-size warning.

B8a evidence:

- main JavaScript chunk: 582.81 kB / 176.11 kB gzip;
- Vite chunk-size warnings: 1;
- direct-eval warnings: 0.

The root cause was application-level: `src/App.js` imported every public, profile and admin route eagerly.

## Change

B8b keeps application shell and shared providers eager:

- Header;
- Main;
- Footer;
- CartProvider;
- FavoriteProvider;
- Router;
- ToastContainer.

Non-shell route modules now load through `React.lazy()` behind one `Suspense` boundary.

The maintained routing structure is unchanged:

- public routes keep the same paths;
- product detail remains `/products/:id`;
- profile routes remain nested below `/profile`;
- admin routes remain nested below `/admin/*`;
- authentication, checkout and fallback routes keep their previous URLs.

The shared route fallback is intentionally small and accessible through `role="status"` and `aria-live="polite"`.

Routing characterization was updated to await lazy module resolution rather than assuming synchronous route rendering.

Validated lab branch:

`modernize/b8b-route-splitting-lab`

## Result

Final measured B8b bundle:

- JavaScript chunks emitted: 34;
- Vite entry chunk: 297.15 kB / 93.06 kB gzip;
- filesystem entry size: 290.19 KiB;
- chunk-size warnings: 0;
- direct-eval warnings: 0.

Compared with B8a, the initial JavaScript payload dropped from 582.81 kB to 297.15 kB, approximately a 49% reduction before gzip.

Representative lazy chunks include:

- Home: 11.13 kB;
- Orders: 22.89 kB;
- TiendaFisica: 67.11 kB;
- validationSchemas: 79.24 kB;
- ProductListContainer: 7.66 kB;
- ProductDetailContainer: 5.51 kB.

## Permanent gate

Quality now captures the Vite build and enforces all of the following:

- direct-eval warnings must remain 0;
- chunk-size warnings above Vite's 500 kB threshold must remain 0;
- exactly one `index-*.js` entry chunk must be emitted;
- the entry chunk must remain at or below 320 KiB.

The 320 KiB ceiling is intentionally above the validated 290.19 KiB result to allow small application growth while still catching a regression back to eager route loading.

The gate checks the built artifact itself rather than relying on source-pattern matching or increasing `build.chunkSizeWarningLimit`.

## Validation

Measured characterization run:

- workflow run: `35527191889`;
- job: `106121299198`;
- conclusion: success.

Final enforced-ceiling run:

- workflow run: `35527317305`;
- job: `106121638760`;
- conclusion: success.

The final run proved together:

- frontend direct production dependency authority: clean;
- frontend ESLint: 0 warnings;
- frontend tests: 65/65 green across 14 files;
- Vite production build: green;
- JavaScript chunks: 34;
- entry chunk: 290.19 KiB;
- chunk-size warnings: 0;
- direct-eval warnings: 0;
- backend Release build: green with warnings treated as errors;
- backend tests: 16/16 green;
- current-tree security baseline: green;
- frontend full npm audit: 0 advisories;
- frontend production npm audit: 0 advisories;
- NuGet solution vulnerability report: clean.

## Next boundary

B8c should focus on the remaining legacy frontend source-format compatibility layer.

The maintained Vite configuration still needs a custom Oxc pre-transform because the historical source tree contains JSX inside `.js` files. A safe next block should inventory JSX-bearing source files, rename them to `.jsx` in behavior-protected groups, update imports without changing module semantics, and remove the custom `jsxInLegacyJs` adapter only after the normal Vite/React pipeline builds the complete application without it.
