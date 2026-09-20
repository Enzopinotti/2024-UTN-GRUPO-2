# B6c3 — React Hook Form compiler compatibility

B6c3 starts from carrier:

`d764d6df9491305a5c7ff1fd683778109745c428` — B6c2e authentication hydration.

## Scope

This block removes the final two frontend lint warnings. Both were
`react-hooks/incompatible-library` findings caused by calling React Hook
Form's `watch()` API in the registration and password-reset forms.

The React lint rule identifies `watch()` as unsafe for automatic memoization
because it relies on interior mutability. The supported compatible alternative
is React Hook Form's dedicated `useWatch()` hook.

Validated lab branch:

`modernize/b6c3-react-hook-form-watch-lab`

## Characterization

Behavior characterization commit:

`671db408a6c253629b3a4f0d93f88a2cc0c1c5af`

Characterization workflow:

- run: `35485458213`
- conclusion: success

The focused contracts prove that both forms:

- mark a weak password's individual requirements correctly;
- react to password changes;
- replace the requirements list with the secure-password message once all
  conditions are met.

## Changes

### Compiler-safe field subscription

Both `Registro` and `ResetearContrasenia` now:

- expose `control` from `useForm()`;
- subscribe to the password with
  `useWatch({ control, name: 'password', defaultValue: '' })`;
- no longer call the incompatible `watch()` function.

### Derived password state

Password-strength conditions were previously duplicated as local state plus a
synchronization effect in both forms.

B6c3 centralizes the pure derivation in
`src/utils/passwordConditions.js`:

- minimum length;
- lowercase;
- uppercase;
- number;
- special character;
- aggregate all-conditions-met check.

The forms now derive these values directly from the watched password and no
longer maintain redundant condition state or password effects.

## Result

B6c2e boundary:

- lint errors: 0
- lint warnings: 2
- `react-hooks/incompatible-library`: 2

B6c3 boundary:

- lint errors: 0
- lint warnings: 0
- permanent Quality ceiling: `--max-warnings 0`

Final reproducible lab validation:

- workflow run: `35485516812`
- job: `106011039065`
- conclusion: success

The final run proved together:

- ESLint: zero warnings with `--max-warnings 0`;
- frontend tests: 64/64 green across 13 files;
- focused password reactivity tests: 2/2 green;
- Vite 8.2.2 production build: green;
- backend Release build: green;
- backend tests: 10/10 green;
- current-tree security baseline: green;
- frontend full npm audit: 0 advisories;
- frontend production npm audit: 0 advisories;
- NuGet solution vulnerability report: clean.

## Next boundary

The maintained frontend static-analysis baseline is now warning-free.

The next modernization boundary should move to the backend compiler warnings
that remain visible in every Release build. They are nullable-reference
findings such as CS8625, CS8604 and CS8618 and should be handled in small,
behavior-protected groups rather than suppressed globally.
