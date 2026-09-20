# B8a — Native empty-cart animation and direct-eval-free frontend

B8a starts from carrier:

`4fff0bbfcade4e77d0a0312e847a037b5d2ed420` — B7b2 warning-free backend.

At that boundary the maintained Vite build was functionally green but still emitted
two frontend build warnings:

- a main JavaScript chunk above Vite's 500 kB warning threshold;
- a direct `eval` warning inside
  `@lottiefiles/react-lottie-player/dist/lottie-react.esm.js`.

## Characterization

The Lottie runtime was reachable from exactly one maintained UI component:

`src/components/carts/EmptyCart.js`

It existed only to render:

`public/json/empty-cart-animation.json`

That JSON payload is a small vector animation of an orange empty shopping cart.
The component already receives its outer bounce motion from the existing
`.empty-cart-image` CSS class.

The runtime dependency chain was:

- `@lottiefiles/react-lottie-player@3.6.0`;
- `lottie-web@5.13.0`.

No other package in the lock depended on either runtime.

Validated lab branch:

`modernize/b8a-empty-cart-lottie-removal-lab`

## Change

B8a replaces the Lottie player with a native inline SVG that preserves:

- the orange cart illustration;
- animated line drawing;
- animated wheel appearance;
- the existing responsive size and bounce class;
- the existing empty-cart heading and explanatory copy.

The obsolete Lottie JSON payload is removed.

The package and lock authorities no longer contain either:

- `@lottiefiles/react-lottie-player`;
- `lottie-web`.

A focused `EmptyCart` contract verifies that the native illustration remains
present and accessible together with the maintained empty-cart copy.

## Permanent gates

Quality now additionally asserts:

- neither Lottie runtime may return to dependencies or devDependencies;
- neither Lottie package may return to the lockfile;
- the deleted JSON payload may not return as stale source authority;
- the Vite production build is captured and must contain zero `[EVAL]` warnings.

The bundle-size warning is intentionally not suppressed or folded into this block.
It remains visible for the separate route/code-splitting boundary.

## Validation

Final reproducible lab validation:

- workflow run: `35522931378`;
- job: `106110039413`;
- conclusion: success.

The run proved together:

- frontend ESLint: 0 warnings;
- frontend tests: 65/65 green across 14 files;
- Vite 8.2.2 production build: green;
- direct-eval build warnings: 0;
- main JavaScript chunk: 582.81 kB / 176.11 kB gzip;
- backend Release build: green with 0 compiler warnings;
- backend tests: 16/16 green;
- current-tree security baseline: green;
- frontend full npm audit: 0 advisories;
- frontend production npm audit: 0 advisories;
- NuGet solution vulnerability report: clean;
- tracked repository state: clean.

For comparison, the pre-B8a build emitted a 994.74 kB / 273.64 kB gzip main
JavaScript chunk. Removing the single-use Lottie runtime therefore materially
reduced the initial bundle while also removing the direct-eval warning.

## Next boundary

B8b should address the remaining chunk-size warning through application-level
code splitting rather than increasing `build.chunkSizeWarningLimit`.

The current router still imports every public, profile and admin route eagerly in
`App.js`. The next block should preserve the existing routing contracts while
loading non-shell route modules lazily, then freeze the resulting initial-chunk
boundary in Quality.
