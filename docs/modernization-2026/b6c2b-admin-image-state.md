# B6c2b — Admin image-state cleanup

B6c2b starts from carrier:

`fafb6943baaa6800fee7e68d587eb42b08f9e912` — B6c2a carousel state-effect cleanup.

## Scope

This block removes two remaining `react-hooks/set-state-in-effect` warnings from
admin image display components without changing form, authentication, profile, or
React Hook Form behavior.

Validated lab branch:

`modernize/b6c2b-image-state-lab`

Behavior characterization commit:

`7f42a464cb66fdd49939b4c6223591b8a01f4c74`

Characterization workflow:

- run: `35481988859`
- job: `106001327080`
- result: success

The characterization proved the four new image-item contracts on the original
implementation while the lint baseline remained at eight warnings.

Validated refactor commit:

`d4a0639e2a144519fd03b8d1ded5d0c327201933`

Final lab validation:

- workflow run: `35482038264`
- job: `106001455845`
- conclusion: success

## Changes

### CategoryItem

`CategoryItem` previously mirrored `category.imagenUrl` into local
`imageSrc` state inside an effect. The displayed value is fully derived from the
current prop, so B6c2b removes the synchronization effect and derives
`imageSrc` directly.

The existing image-error state remains unchanged, preserving the current
fallback behavior.

### AdminProductItem

`AdminProductItem` previously created a second `imageSrc` state and an effect
that copied a string URL or created an object URL. That state was never consumed
by the rendered image: the component already renders
`imagenUrls.$values[0]` directly.

B6c2b removes the dead state/effect and therefore also removes unnecessary
object-URL creation from this list item. It does not change the backend response
shape or the maintained rendered image contract.

### Behavior coverage

`AdminImageItems.state.test.js` adds four focused contracts covering:

- category image rendering from the current prop;
- category image-error fallback;
- product rendering from the first backend image URL;
- product image-error fallback;
- edit/delete callback identity and IDs.

## Result

B6c2a boundary:

- lint errors: 0
- lint warnings: 8
- `react-hooks/set-state-in-effect`: 6
- `react-hooks/incompatible-library`: 2

B6c2b boundary:

- lint errors: 0
- lint warnings: 6
- `react-hooks/set-state-in-effect`: 4
- `react-hooks/incompatible-library`: 2
- permanent Quality ceiling: `--max-warnings 6`

## Validation

The final lab run proved together:

- direct frontend production dependency authority: clean;
- lint: 0 errors / 6 warnings;
- frontend tests: 47/47 green across 8 files;
- focused admin image tests: 4/4 green;
- Vite production build: green;
- backend Release build: green;
- backend tests: 10/10 green;
- current-tree security baseline: green;
- frontend full npm audit: 0 advisories;
- frontend production npm audit: 0 advisories;
- NuGet solution vulnerability report: clean.

## Next boundary

B6c2c should keep the remaining four synchronous state-effect findings isolated:

1. `CategoryForm` form-state synchronization;
2. `UserAddresses` current-user synchronization;
3. `Profile` current-user synchronization;
4. `AuthContext` persisted-token hydration.

These should not be collapsed into one broad refactor. Form synchronization can
be handled first with focused modal/editing tests; profile/address state and
authentication hydration should remain separate because they have different
lifecycle and persistence semantics.

The two `react-hooks/incompatible-library` findings in registration/password
reset remain B6c3 work.
