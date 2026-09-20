# B6c1 — Hook dependency cleanup

B6c1 starts from carrier:

`2dad3940ca7c4f156b203dec2c7806c7023d97ef` — B6b mechanical frontend lint cleanup.

## Scope

This block resolves only the three remaining `react-hooks/exhaustive-deps`
warnings. It does not refactor synchronous state updates inside effects and does
not change the two incompatible-library findings.

Validated lab branch:

`modernize/b6c-hook-deps-lab`

Final reproducible validation:

- workflow run: `35480125039`
- job: `105996322811`
- conclusion: success

## Changes

- `SearchBar` stabilizes `handleSearch` with `useCallback` and declares the
  debounce effect dependencies explicitly.
- `SearchBarMobile` applies the same maintained 500ms debounce contract.
- `FavoriteContext.removeFavorite` is stable with `useCallback`.
- `Product` declares the real favorite synchronization dependencies.
- focused tests protect both search debounce behavior and favorite
  synchronization before the permanent lint ceiling is reduced.

## Result

B6b boundary:

- lint errors: 0
- lint warnings: 12
- `react-hooks/exhaustive-deps`: 3
- `react-hooks/set-state-in-effect`: 7
- `react-hooks/incompatible-library`: 2

B6c1 boundary:

- lint errors: 0
- lint warnings: 9
- `react-hooks/exhaustive-deps`: 0
- `react-hooks/set-state-in-effect`: 7
- `react-hooks/incompatible-library`: 2
- permanent Quality ceiling: `--max-warnings 9`

## Validation

The final lab run proved together:

- direct frontend production dependency authority: clean
- frontend tests: 42/42 green across 6 files
- Vite production build: green
- backend Release build: green
- backend tests: 10/10 green
- current-tree security baseline: green
- frontend full npm audit: 0 advisories
- frontend production npm audit: 0 advisories
- NuGet solution vulnerability report: clean

## Next boundary

B6c2 should address the seven `react-hooks/set-state-in-effect` warnings in
behavior-protected subgroups. Derived state and URL/object-image state should be
reviewed separately from authentication/profile synchronization. The two
`react-hooks/incompatible-library` warnings remain a separate B6c3 boundary.
