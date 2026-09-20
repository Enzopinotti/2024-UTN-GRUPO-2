# B10 — Warning-free Testing Library runtime

B10 starts from the completed B9 dead-source authority boundary.

The B9 carrier is:

`8097c807c18a8330f6d414336116fe24aa737523` — `B9: enforce dead frontend source authority`.

The B10 lab is developed on:

`modernize/b10-testing-runtime-lab`.

## Objective

Remove the React 18 test-runtime deprecation warning:

`ReactDOMTestUtils.act is deprecated in favor of React.act`

without changing application behavior and without combining this work with a React 19 migration.

B10 modernizes only the frontend testing runtime. React and ReactDOM remain on the maintained React 18 line during this block.

## Baseline

The B9 Quality run is fully green:

- frontend source authority: clean;
- ESLint: zero warnings;
- frontend tests: 13 files / 60 tests green;
- Vite build: green;
- backend build: zero warnings;
- backend tests: 16 green;
- npm full graph: zero advisories;
- npm production graph: zero advisories;
- NuGet vulnerability report: clean.

The remaining test-runtime noise appears whenever React Testing Library renders components because the previous authority was:

- `@testing-library/react 13.4.0`;
- `@testing-library/user-event 13.5.0`.

## B10 package authority

The lab updates and pins:

- `@testing-library/react 16.3.3`;
- `@testing-library/user-event 14.6.7`;
- `@testing-library/dom 10.4.2`.

Existing maintained test/runtime packages remain:

- `@testing-library/jest-dom 7.0.1`;
- `vitest 5.0.1`;
- `jsdom 30.0.1`;
- `react 18.3.1`;
- `react-dom 18.3.1`.

The DOM package is declared directly because it is part of the maintained Testing Library peer/runtime boundary rather than being allowed to arrive accidentally through another package.

## Validation boundary

The B10 lab must prove all of the following before promotion:

- exact install with `npm ci`;
- production dependency authority is clean;
- JSX extension authority is clean;
- dead-source authority is clean;
- ESLint has zero warnings;
- all 13 frontend test files remain green;
- all 60 frontend tests remain green;
- test output contains zero occurrences of `ReactDOMTestUtils.act`;
- Vite production build remains green;
- backend build remains warning-free;
- all backend tests remain green;
- current-tree security baseline remains green;
- npm full dependency graph has zero advisories;
- npm production dependency graph has zero advisories.

## Permanent Quality gate

The main Quality workflow now:

1. pins the B10 Testing Library versions as source authority;
2. captures frontend test stdout/stderr;
3. fails if `ReactDOMTestUtils.act` appears anywhere in the test output;
4. publishes the captured frontend test output with the existing Quality evidence.

This prevents a future dependency downgrade or test harness regression from silently reintroducing the deprecated ReactDOM test-utils bridge.

## React 19 boundary

React 19 is intentionally excluded from B10.

A React major upgrade changes the application runtime itself and may affect component semantics, third-party packages and rendering behavior. It should be evaluated only after the test harness is modern and warning-free under React 18, with its own characterization and rollback boundary.
