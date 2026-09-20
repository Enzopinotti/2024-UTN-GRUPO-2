# B6c2a — Carousel state-effect cleanup

B6c2a starts from carrier:

`a991430f56cfa5f21ea31e443d13ad462b034526` — B6c1 Hook dependency cleanup.

## Scope

This block removes one behavior-redundant `react-hooks/set-state-in-effect`
warning from the home recommendation carousel. It does not touch image URL
lifecycle, form synchronization, profile/auth synchronization, or incompatible
library findings.

Validated lab branch:

`modernize/b6c2a-carousel-state-lab`

Final reproducible validation:

- workflow run: `35480335811`
- job: `105996870998`
- conclusion: success

## Change

`SegundaSection` initialized `currentIndex` to zero and then ran an effect that
set it to zero again whenever `productos` changed. In the maintained component,
`productos` is populated by the home-products fetch and the redundant effect
did not provide a distinct state transition.

B6c2a removes that synchronous state write and adds focused coverage proving
that the carousel still starts at the first product and expands responsive
visibility from one to three products after a desktop resize.

## Result

B6c1 boundary:

- lint errors: 0
- lint warnings: 9
- `react-hooks/exhaustive-deps`: 0
- `react-hooks/set-state-in-effect`: 7
- `react-hooks/incompatible-library`: 2

B6c2a boundary:

- lint errors: 0
- lint warnings: 8
- `react-hooks/exhaustive-deps`: 0
- `react-hooks/set-state-in-effect`: 6
- `react-hooks/incompatible-library`: 2
- permanent Quality ceiling: `--max-warnings 8`

## Validation

The final lab run proved together:

- direct frontend production dependency authority: clean
- frontend tests: 43/43 green across 7 files
- Vite production build: green
- backend Release build: green
- backend tests: 10/10 green
- current-tree security baseline: green
- frontend full npm audit: 0 advisories
- frontend production npm audit: 0 advisories
- NuGet solution vulnerability report: clean

## Next boundary

B6c2b should handle image-source synchronization separately. `CategoryItem`
and `AdminProductItem` need different treatment because the latter creates
object URLs and therefore requires explicit URL lifecycle cleanup. The remaining
form/auth synchronization warnings should stay isolated until they have focused
behavior coverage.
