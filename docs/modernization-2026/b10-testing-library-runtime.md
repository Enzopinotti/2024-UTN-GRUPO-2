# B10 — Warning-free Testing Library runtime

B10 starts from the completed B9 carrier:

`8097c807c18a8330f6d414336116fe24aa737523` — `B9: enforce dead frontend source authority`.

Validated lab branch:

`modernize/b10-testing-library-runtime-lab`.

## Objective

Modernize the frontend testing runtime without changing React application behavior or moving to React 19.

The B9 test suite was green, but React 18.3.1 emitted repeated runtime deprecation warnings:

`ReactDOMTestUtils.act is deprecated in favor of React.act`.

The warnings came from the old React Testing Library 13.x boundary rather than from product source.

## Dependency authority

B10 changes only development/test dependencies:

- `@testing-library/react`: `^13.4.0` → `16.3.3`;
- adds direct `@testing-library/dom` `10.4.2`;
- removes unused `@testing-library/user-event` `^13.5.0`.

No runtime dependency or npm script changes in this block.

All surviving frontend suites were inspected before the change:

- 11 suites import `@testing-library/react`;
- 2 utility suites do not use Testing Library;
- 0 suites import `@testing-library/user-event`.

Therefore B10 removes unused test tooling instead of upgrading it without a caller.

## Isolated validation

Initial B10 characterization:

- workflow run: `35536961229`;
- job: `106147598996`;
- conclusion: success;
- candidate package commit: `1af3235ba6bf9f7258a9321cb7d2aab81fc7f961`.

The run proved:

- `@testing-library/react@16.3.3`;
- `@testing-library/dom@10.4.2`;
- `@testing-library/user-event` absent from manifest and lock;
- frontend source authority: clean;
- dead-source authority: 0 unexpected unreachable sources;
- ESLint: zero warnings;
- frontend test files: 13/13 green;
- frontend tests: 60/60 green;
- `ReactDOMTestUtils.act` warnings: 0;
- Vite production build: green;
- full npm audit: 0 advisories;
- production npm audit: 0 advisories.

## Permanent Quality boundary

Quality now pins the maintained test-runtime authority and rejects regressions:

- React Testing Library must remain exactly `16.3.3`;
- DOM Testing Library must remain exactly `10.4.2`;
- unused `@testing-library/user-event` may not return to manifest or lock;
- frontend test output is captured as evidence;
- any future `ReactDOMTestUtils.act` warning fails Quality.

The captured test output is retained with the normal Quality evidence.

## Non-goals

B10 does not:

- upgrade React or React DOM;
- change component behavior;
- change route behavior;
- rewrite tests to new interaction APIs;
- add new product functionality.

React remains `18.3.1` in this block.

## Next boundary

With the test runtime clean under React 18, a subsequent block can characterize React 19 separately against the full Quality contract. That migration must not be inferred as safe merely because Testing Library is current.
