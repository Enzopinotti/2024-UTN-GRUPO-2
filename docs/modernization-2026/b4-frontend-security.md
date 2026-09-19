# B4 — Frontend dependency-security reduction

B4 begins after the backend NuGet graph is clean on `net8.0`.

The frontend remains a reconstructed React 18 / Create React App 5 application.
This block does not migrate the build system or router major version without
first proving the security and behavior boundaries.

## B4 characterization

Baseline carrier before frontend reduction:

`47d37dedc40459a2b997504e244e96b060ec6628`

B4 characterization lab:

- commit: `2977adaf5f78a1d53eee90e683ac70851a3428a9`;
- run: `35442527218` — success;
- artifact: `10584680317`;
- digest:
  `sha256:66b88409d931bbd42029ce56d02308e6e7072c20f04c4a9f16fbe480d08462be`.

Measured baseline:

- all npm findings: 70;
- low: 15;
- moderate: 18;
- high: 34;
- critical: 3;
- `npm audit --omit=dev` reported the same 70 findings.

The production-only result was misleading because historical `package.json`
classified CRA/build/test tooling as runtime dependencies.

Direct vulnerable surface included runtime packages such as
`react-router-dom`, `dompurify` and `sweetalert2`, plus build/tooling
packages such as `react-scripts`, minifiers and webpack development tooling.

## B4a — Compatible reduction without framework migration

A dedicated candidate lab reclassifies only unambiguous tooling as
`devDependencies`:

- `@rollup/plugin-terser`;
- `@svgr/webpack`;
- Testing Library packages;
- `css-minimizer-webpack-plugin`;
- `json-server`;
- `react-scripts`;
- `sass`.

No declared version range is changed by that classification.

The lab then runs only compatible lockfile maintenance:

- `npm install --package-lock-only`;
- `npm audit fix --package-lock-only`;
- **no `--force`**.

Within already-declared semver ranges, the resulting lockfile resolves examples
such as:

- `react-router-dom`: 6.26.1 → 6.30.6;
- `react-router`: 6.26.1 → 6.30.6;
- `dompurify`: 3.1.7 → 3.4.15;
- `sweetalert2`: 11.7.12 → 11.26.25;
- `webpack-dev-server`: 5.1.0 → 5.2.6.

`react-scripts` remains 5.0.1 and is explicitly dev-only.

### Candidate evidence

Synthesis run:

- source commit: `930279769399f3c6eaf16fb949a1fc77264da733`;
- generated candidate commit:
  `d2685339863e55a521edd347eb7f415723b028e1`;
- run: `35442647387` — success;
- artifact: `10584108876`;
- digest:
  `sha256:20af99c7803f2e11722090d225443b0eb2fed5e968ce36a7b48c4e5695eaffbc`.

Candidate result:

- frontend tests: 13/13;
- production build: green;
- all npm findings: 32;
- low: 9;
- moderate: 8;
- high: 15;
- critical: 0;
- production-only findings: 2;
- production high: 0;
- production critical: 0.

Reproducibility verification:

- verification head: `09b3b232bc91188fce737d94c439054c64c47095`;
- run: `35442782303` — success;
- artifact: `10584024200`;
- digest:
  `sha256:0f138b21b6a3c46f170cdebab8759ccc3067105852321b6c858d2e15050e68ab`.

The two remaining production findings are moderate and belong to:

- `react-router-dom`;
- transitive `react-router`.

npm's available remediation points to `react-router-dom 7.18.4`, which is a
major-version migration. B4a deliberately does not hide that change inside a
lockfile/security cleanup.

## Maintained B4a contract

Permanent Quality now requires:

- build/test tooling remains dev-only;
- exact install still succeeds;
- 13 frontend tests remain green;
- frontend production build remains green;
- production npm audit has 0 high findings;
- production npm audit has 0 critical findings;
- production audit does not exceed the validated two-moderate boundary;
- NuGet solution audit remains clean.

The complete dev/build dependency graph still contains CRA-era advisory debt.
That debt is visible in CI evidence rather than being presented as clean.

## Next decision boundary

B4b must decide separately whether to:

1. migrate React Router 6 → 7 to clear the remaining production moderates; and
2. replace CRA 5 to remove the remaining high-severity dev/build advisories.

Neither migration should occur before adding route/rendering contracts that
protect the application's actual navigation behavior.

The next work block is therefore test coverage for routing/application bootstrap,
followed by an isolated Router/build-system migration lab.
