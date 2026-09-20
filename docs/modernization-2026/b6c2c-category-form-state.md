# B6c2c — Category form state cleanup

B6c2c starts from carrier:

`7d20b3e0be508c1b66a17ffae8c427991524a469` — B6c2b admin image-state cleanup.

## Scope

This block removes the `react-hooks/set-state-in-effect` warning from
`CategoryForm` without changing category persistence, authentication, profile
state, or React Hook Form behavior.

Validated lab branch:

`modernize/b6c2c-category-form-lab`

Behavior characterization commit:

`8fe642f24eb38c365e425ae76381aa399082f345`

Characterization workflow:

- run: `35482193534`
- result: success

The characterization added three focused form-state contracts while preserving
the six-warning B6c2b baseline.

Validated refactor commit:

`2ee30992196440e8b80211cb92514371902fdf12`

Final lab validation:

- workflow run: `35482255754`
- job: `106002039392`
- conclusion: success

## Change

`CategoryForm` previously copied the selected `category` prop into five local
state values from a synchronous effect. The form already has a natural identity
boundary: editing a different category or switching to create mode represents a
new form session.

B6c2c makes that identity explicit:

- local field and preview state initialize directly from the selected category;
- `CategoryListContainer` keys the form with
  `editingCategory?.idCategoria ?? 'new'`;
- changing the selected category therefore remounts the form with fresh initial
  state instead of synchronizing five setters after render;
- submit, image validation, preview removal, FormData shape and close/reset
  behavior remain unchanged.

## Behavior coverage

`CategoryForm.state.test.js` protects:

- edit-mode field and existing-image initialization;
- reinitialization when the keyed category changes;
- clean create-mode state when switching to the `new` key;
- the maintained edit FormData contract and close callback.

## Result

B6c2b boundary:

- lint errors: 0
- lint warnings: 6
- `react-hooks/set-state-in-effect`: 4
- `react-hooks/incompatible-library`: 2

B6c2c boundary:

- lint errors: 0
- lint warnings: 5
- `react-hooks/set-state-in-effect`: 3
- `react-hooks/incompatible-library`: 2
- permanent Quality ceiling: `--max-warnings 5`

## Validation

The final lab run proved together:

- direct frontend production dependency authority: clean;
- lint: 0 errors / 5 warnings;
- frontend tests: 50/50 green across 9 files;
- focused CategoryForm tests: 3/3 green;
- Vite production build: green;
- backend Release build: green;
- backend tests: 10/10 green;
- current-tree security baseline: green;
- frontend full npm audit: 0 advisories;
- frontend production npm audit: 0 advisories;
- NuGet solution vulnerability report: clean.

## Next boundary

Three synchronous state-effect findings remain and should continue to be treated
separately:

1. `UserAddresses` mirrors the current user into editable local state;
2. `Profile` mirrors the current user into editable local state;
3. `AuthContext` hydrates persisted token state during startup.

The profile/address cases can be characterized next because they share a
current-user synchronization pattern, but authentication hydration must remain
its own boundary due to localStorage, token expiry and session semantics.

The two `react-hooks/incompatible-library` findings in registration and
password reset remain B6c3 work.
