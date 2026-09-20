# B6c2d2 — Profile draft state authority

B6c2d2 starts from carrier:

`8eb2295b12952503847bbd1f97e8d2ceb005dab5` — B6c2d1 user-address state authority.

## Scope

This block removes the remaining profile `react-hooks/set-state-in-effect`
warning and fixes the profile Outlet setter contract. Authentication hydration
remains isolated for B6c2e, and the two React Hook Form
`react-hooks/incompatible-library` findings remain B6c3 work.

Validated lab branch:

`modernize/b6c2d2-profile-state-lab`

Initial characterization run:

- workflow run: `35482562337`
- job: `106002875405`
- conclusion: expected failure

The characterization added three focused profile contracts and exposed the
historical correctness bug directly: `Profile` read `setUser` from the
Outlet while `UserLayout` actually publishes `setUserData`.

## Changes

### Explicit editable draft

`Profile` no longer mirrors the current Outlet user into local state from a
synchronous effect.

Instead:

- the Outlet `user` is authoritative whenever the profile is not being edited;
- entering edit mode snapshots the current user into a local `draftUser`;
- field changes mutate only that draft;
- `Guardar` validates and publishes the draft through the real
  `setUserData` Outlet setter;
- after saving, the local draft is discarded and the Outlet user becomes the
  display authority again.

This keeps edit-in-progress values isolated without needing a prop-to-state
synchronization effect.

### Profile picture publication

Profile-picture completion now publishes through the real
`setUserData` Outlet setter. When an edit draft is active, the draft picture
is updated as well so the editing view stays coherent.

### Birth-date input correctness

`UserDetail` used `e.targets.value` for date inputs. B6c2d2 fixes this to
`e.target.value` and protects the behavior in the profile editing contract.

## Behavior coverage

`Profile.state.test.js` protects:

- rendering the current Outlet user and following a changed user while not
  editing;
- keeping field edits local until `Guardar`;
- editing and publishing the birth date correctly;
- publishing the saved profile through `setUserData`;
- publishing profile-picture completion through the same canonical setter.

## Result

B6c2d1 boundary:

- lint errors: 0
- lint warnings: 4
- `react-hooks/set-state-in-effect`: 2
- `react-hooks/incompatible-library`: 2

B6c2d2 boundary:

- lint errors: 0
- lint warnings: 3
- `react-hooks/set-state-in-effect`: 1
- `react-hooks/incompatible-library`: 2
- permanent Quality ceiling: `--max-warnings 3`

Final reproducible lab validation:

- workflow run: `35485062632`
- job: `106009801085`
- conclusion: success

The final run proved together:

- direct frontend production dependency authority: clean;
- lint: 0 errors / 3 warnings;
- frontend tests: 56/56 green across 11 files;
- focused Profile tests: 3/3 green;
- Vite 8.2.2 production build: green;
- backend Release build: green;
- backend tests: 10/10 green;
- current-tree security baseline: green;
- frontend full npm audit: 0 advisories;
- frontend production npm audit: 0 advisories;
- NuGet solution vulnerability report: clean.

## Next boundary

One synchronous state-effect finding remains:

1. `AuthContext` hydrates persisted authentication from `localStorage`
   during startup.

That work must remain its own B6c2e boundary because it combines browser
persistence, JWT decoding, expiry handling and logout semantics.

After B6c2e, the two remaining `react-hooks/incompatible-library` warnings in
registration/password-reset flows form the separate B6c3 boundary.
