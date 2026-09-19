# B6a — Modern frontend static-analysis baseline

B6a starts from the post-B5 carrier:

`63bc2641096068fa7c61001b6ce5d91b48c98a41`

At that point the frontend had already moved from CRA to Vite/Vitest, React Router
was on 7.18.4, frontend npm audit was clean, NuGet audit was clean, and the
maintained behavior suites were green.

Removing CRA also removed its implicit ESLint integration. B6a restores static
analysis explicitly instead of relying on build-tool side effects.

## Characterization

Lab branch:

`modernize/b6-eslint-lab`

Initial characterization run:

`35463824632` — success.

The probe used the maintained Node 24.20.0 runtime and the current ESLint flat
configuration model.

Observed toolchain:

- ESLint 10.11.0;
- @eslint/js 10.0.1;
- eslint-plugin-react-hooks 7.1.1;
- globals 17.12.0.

Initial source findings:

- fatal parser/configuration errors: 0;
- blocking errors: 10;
- warnings: 108;
- files with findings: 94.

The 10 blocking findings were:

- 7 `react-hooks/set-state-in-effect`;
- 2 `no-undef` in an unused browser-side Mercado Pago service;
- 1 irregular whitespace finding in the search banner.

The warning surface was primarily historical cleanup debt:

- 103 `no-unused-vars`;
- 3 `react-hooks/exhaustive-deps`;
- 2 `react-hooks/incompatible-library`.

## Maintained B6a boundary

B6a deliberately does not mix static-analysis adoption with a broad React state
refactor.

The seven synchronous effect-state findings are retained as visible warnings.
They identify future refactor candidates but are not changed without dedicated
behavior coverage.

B6a instead closes the objective blockers:

- removes the unused frontend `mercadoPagoService.js`, which referenced
  undeclared browser globals and duplicated payment authority already owned by
  the backend;
- normalizes the irregular whitespace in the search heading;
- adds `eslint.config.mjs` using flat config;
- enables the ESLint recommended JavaScript rules and the official React Hooks
  recommended rules;
- records `react-hooks/set-state-in-effect` as warning-level migration debt;
- keeps unused variables as warnings for incremental cleanup;
- adds a maintained `npm run lint` command;
- pins the ESLint toolchain as dev dependencies;
- updates permanent Quality to run:
  `npm run lint -- --max-warnings 115`.

The warning ceiling prevents new lint debt from being added while allowing the
existing 2024 source to be modernized incrementally.

## Candidate evidence

Validated package-authority candidate:

`51c8b830f11d5cf2e5b974c5a9415b628818036c`

Reproducible candidate run:

`35464099158` — success.

Evidence:

- lint fatal errors: 0;
- lint blocking errors: 0;
- lint warnings: 115;
- frontend tests: 38/38 green;
- Vite production build: green;
- backend tests: 10/10 green;
- backend Release build: green;
- current-tree security baseline: green;
- frontend npm audit: 0 advisories;
- NuGet solution vulnerability report: clean.

Permanent Quality validation:

`35464203241` — success.

Quality proved the committed package/lock authority, direct frontend dependency
authority, lint warning ceiling, tests, production build, backend build/tests,
current-tree security boundary and zero-advisory dependency graph together.

## Next boundary

B6b can reduce the 115-warning baseline in small behavior-protected groups.

The highest-value follow-up is not a blanket auto-fix. It is to separate:

1. safe unused-import/unused-variable deletion;
2. missing Hook dependency review;
3. React state synchronization refactors currently reported by
   `set-state-in-effect`;
4. incompatible-library warnings that may affect a future React major upgrade.

React 19 remains a separate migration decision and should not be bundled into
B6a.
