# B6c2e — Authentication hydration state

B6c2e starts from carrier:

`90c0c1e43102aecda3e13c80ec3dd0f56968fea6` — B6c2d2 profile draft state authority.

## Scope

This block removes the final `react-hooks/set-state-in-effect` warning from
`AuthContext` without changing the public authentication API. It preserves
persisted-token hydration, expiry handling, malformed-token cleanup, login and
logout behavior.

Validated lab branch:

`modernize/b6c2e-auth-hydration-lab`

## Characterization

Behavior characterization commit:

`e7ceb549e911654a6774c33eced7e3e9f6de2e7a`

Characterization workflow:

- run: `35485257917`
- conclusion: success

The six focused contracts pass on the original implementation and protect:

1. startup without a persisted access token;
2. hydration from a valid persisted JWT;
3. cleanup of expired persisted credentials;
4. cleanup of malformed persisted credentials;
5. login state plus access/refresh-token persistence;
6. logout state plus credential cleanup.

## Change

The previous provider initialized unauthenticated state and then used an effect
to inspect `localStorage`, decode the token and synchronously call
`setAuth`.

B6c2e makes persisted authentication part of the state initialization boundary:

- `useState(readInitialAuth)` lazily reads persisted credentials exactly when
  the provider is initialized;
- a valid, unexpired JWT becomes the initial auth value directly;
- expired or malformed credentials are cleared before the unauthenticated
  initial state is returned;
- login and logout retain their existing callback API;
- credential cleanup is centralized so startup invalidation and logout share
  the same persistence behavior;
- the hydration effect and its extra render disappear completely.

## Result

B6c2d2 boundary:

- lint errors: 0
- lint warnings: 3
- `react-hooks/set-state-in-effect`: 1
- `react-hooks/incompatible-library`: 2

B6c2e boundary:

- lint errors: 0
- lint warnings: 2
- `react-hooks/set-state-in-effect`: 0
- `react-hooks/incompatible-library`: 2
- permanent Quality ceiling: `--max-warnings 2`

Final reproducible lab validation:

- workflow run: `35485295495`
- job: `106010426321`
- conclusion: success

The final run proved together:

- lint: 0 errors / 2 warnings;
- frontend tests: 62/62 green across 12 files;
- focused AuthContext tests: 6/6 green;
- Vite 8.2.2 production build: green;
- backend Release build: green;
- backend tests: 10/10 green;
- current-tree security baseline: green;
- frontend full npm audit: 0 advisories;
- frontend production npm audit: 0 advisories;
- NuGet solution vulnerability report: clean.

## Next boundary

No `set-state-in-effect` warnings remain.

B6c3 should address the two remaining
`react-hooks/incompatible-library` findings in the registration/password-reset
flows. Those findings involve React Hook Form and must be investigated as a
separate compatibility boundary rather than silenced as generic lint debt.
