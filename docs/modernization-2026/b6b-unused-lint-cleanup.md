# B6b — Mechanical frontend lint cleanup

B6b starts from carrier:

`cc3b1c2cb7b9ff2bc40a96048aa73e85af865f5c` — B6a modern frontend static analysis.

## Scope

This block removes only mechanical `no-unused-vars` debt. It deliberately does not refactor Hook behavior, state synchronization, or React semantics.

Validated lab branch:

`modernize/b6b-lint-cleanup-lab`

Validated source candidate:

`f786c356f44da626e220a07af20ae13ec00d590e`

Permanent lint-ceiling candidate:

`766ae6606f19a6b56060bd046e4deb5f8445de79`

Final reproducible validation:

- workflow run: `35474797555`
- job: `105982082321`
- conclusion: success

## Result

B6a baseline:

- lint errors: 0
- lint warnings: 115
- `no-unused-vars`: 103
- `react-hooks/set-state-in-effect`: 7
- `react-hooks/exhaustive-deps`: 3
- `react-hooks/incompatible-library`: 2

B6b boundary:

- lint errors: 0
- lint warnings: 12
- `no-unused-vars`: 0
- `react-hooks/set-state-in-effect`: 7
- `react-hooks/exhaustive-deps`: 3
- `react-hooks/incompatible-library`: 2
- permanent Quality ceiling: `--max-warnings 12`

The cleanup removes redundant React default imports under the automatic JSX runtime and a small set of dead imports/variables. Setter/effect behavior was preserved where state writes still participate in existing behavior.

## Validation

The final run proved together:

- direct frontend production dependency authority: clean
- frontend tests: 38/38 green
- Vite production build: green
- backend Release build: green
- backend tests: 10/10 green
- current-tree security baseline: green
- frontend full and production npm audits: 0 advisories
- NuGet solution vulnerability report: clean
- rerunning the B6b cleanup on the already-clean tree produces no additional source commit

## Remaining lint debt

The remaining 12 warnings are intentionally semantic and belong to later behavior-protected work:

1. 7 synchronous state updates inside effects;
2. 3 missing Hook dependency warnings;
3. 2 incompatible-library warnings.

B6c should treat those groups separately. It should not use blanket autofix and should add focused behavior coverage before changing state/effect semantics.
