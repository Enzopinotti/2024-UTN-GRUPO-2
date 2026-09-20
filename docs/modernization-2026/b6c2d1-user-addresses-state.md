# B6c2d1 — User address state authority

B6c2d1 starts from carrier:

`250c512e6ad4834982bc923eef264c7fa4dd7b2b` — B6c2c category-form state cleanup.

## Scope

This block removes one `react-hooks/set-state-in-effect` warning from
`UserAddresses`. It does not change profile draft editing, authentication
hydration, or React Hook Form behavior.

Validated lab branch:

`modernize/b6c2d1-user-addresses-lab`

Behavior characterization commit:

`2baed6664ee0c92f3750310f0bfabc945970d9f5`

Characterization workflow:

- run: `35482397797`
- job: `106002423441`
- conclusion: success

The characterization proved the new address contracts while the lint baseline
remained at five warnings.

Validated refactor commit:

`b21053008b675c4dc5dfcac0b166bf376c30a368`

Final lab validation:

- workflow run: `35482441378`
- job: `106002544589`
- conclusion: success

## Change

`UserAddresses` previously copied `currentUser` from the Outlet into a second
`userData` state value and synchronized that copy with an effect.

That second authority was unnecessary:

- the Outlet user is already the canonical current user;
- address edits and deletions already publish their updated user through
  `setUserData`;
- the local component only needs transient address-edit UI state.

B6c2d1 therefore:

- removes the `currentUser -> userData` synchronization effect;
- uses the Outlet user directly as `userData`;
- removes duplicate local-user writes after edit/delete;
- preserves `editingAddress`, `newAddress`, validation and SweetAlert flows.

## Behavior coverage

`UserAddresses.state.test.js` protects:

- rendering the current Outlet user;
- following a changed Outlet user on rerender;
- edit publication through `setUserData`;
- confirmed deletion publication through `setUserData`;
- maintained success notification behavior.

## Result

B6c2c boundary:

- lint errors: 0
- lint warnings: 5
- `react-hooks/set-state-in-effect`: 3
- `react-hooks/incompatible-library`: 2

B6c2d1 boundary:

- lint errors: 0
- lint warnings: 4
- `react-hooks/set-state-in-effect`: 2
- `react-hooks/incompatible-library`: 2
- permanent Quality ceiling: `--max-warnings 4`

## Validation

The final lab run proved together:

- direct frontend production dependency authority: clean;
- lint: 0 errors / 4 warnings;
- frontend tests: 53/53 green across 10 files;
- focused UserAddresses tests: 3/3 green;
- Vite production build: green;
- backend Release build: green;
- backend tests: 10/10 green;
- current-tree security baseline: green;
- frontend full npm audit: 0 advisories;
- frontend production npm audit: 0 advisories;
- NuGet solution vulnerability report: clean.

## Next boundary

Two synchronous state-effect findings remain:

1. `Profile` keeps an editable local draft synchronized from the Outlet user;
2. `AuthContext` hydrates persisted authentication from localStorage on startup.

They must remain separate. `Profile` also needs explicit behavior coverage
because the current code reads `setUser` from Outlet context while
`UserLayout` actually exposes `setUserData`. That historical mismatch must be
handled as a profile-specific correctness fix rather than hidden inside the
authentication block.

The two `react-hooks/incompatible-library` findings remain B6c3.
