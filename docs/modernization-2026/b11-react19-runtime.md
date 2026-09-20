# B11 — React 19 runtime authority

B11 starts from the completed B10 warning-free Testing Library boundary.

The B10 carrier is:

`b0e84a54f2d2d9c558497a082821fcbe140c796f` — `B10: modernize warning-free Testing Library runtime`.

The B11 lab is developed on:

`modernize/b11-react19-lab`.

## Objective

Move the maintained frontend runtime from React 18.3.1 to React 19.3.0 without peer-dependency overrides, without weakening the existing behavior gates, and without carrying obsolete runtime packages only to satisfy historical package authority.

## First React 19 probe

The first B11 run intentionally attempted:

- `react 19.3.0`;
- `react-dom 19.3.0`;
- no `--force`;
- no `--legacy-peer-deps`.

The first candidate failed reproducibility at `npm ci`.

The blocker was not the application runtime. The exact peer conflict was:

`@mercadopago/sdk-react@0.0.19` only declared React/ReactDOM through version 18.

The same install evidence showed that the maintained Google Maps package already accepted React 19.

## Runtime dependency pruning

The current production dependency authority reports 95 reachable frontend production modules.

The production external-package set does not include:

- `@mercadopago/sdk-react`;
- `@react-google-maps/api`;
- `web-vitals`.

B11 also performs an explicit source-reference gate before pruning and proves that none of those package names has a caller under `Frontend/antigal.client/src`.

Because those packages are not part of the reachable product runtime, B11 removes them instead of upgrading or overriding unused dependencies.

This also resolves the stale Mercado Pago peer constraint without inventing a compatibility exception. Backend Mercado Pago integration is a separate .NET dependency boundary and is not changed by this frontend cleanup.

## React 19 package authority

B11 pins:

- `react 19.3.0`;
- `react-dom 19.3.0`.

The existing maintained frontend stack remains:

- React Router 7.18.4;
- Testing Library React 16.3.3;
- Vitest 5.0.1;
- Vite 8.2.2;
- ESLint 10.11.0.

## Compatibility checks

The B11 lab validates:

- installation without peer overrides;
- reproducible `npm ci`;
- a valid React 19 dependency tree;
- absence of the retired runtime packages at the root;
- production dependency authority;
- JSX extension authority;
- dead-source authority;
- ESLint with zero warnings;
- all frontend tests under React 19;
- absence of the deprecated `ReactDOMTestUtils.act` bridge;
- Vite production build under React 19;
- backend restore/build/tests;
- warning-free backend compilation;
- current-tree security baseline;
- zero npm advisories in both full and production dependency graphs.

The lab additionally scans the reachable carousel dependency `react-slick` for legacy ReactDOM APIs removed from modern React:

- `findDOMNode`;
- `ReactDOM.render`;
- `unmountComponentAtNode`.

The validated candidate produced no matches.

## Validated B11 evidence

The final B11 lab run `35540396268` completed successfully after reconciling the permanent B8b entry-chunk budget.

Validated signals include:

- React 19.3.0 / ReactDOM 19.3.0 installed;
- retired runtime callers: 0;
- retired runtime packages absent;
- 13 / 13 frontend test files green;
- 60 / 60 frontend tests green;
- Vite build green;
- backend build: 0 warnings;
- backend tests: 16 / 16 green;
- current-tree security baseline green;
- npm full graph: 0 advisories;
- npm production graph: 0 advisories;
- `b11_react19_security_boundary=accepted`;
- entry chunk: 153.41 KiB;
- isolated React runtime chunk: 213.70 KiB.

## Carrier reconciliation

The first carrier Quality run after the React 19 promotion, `35540310305`, correctly rejected the candidate because React 19 increased the Vite entry chunk to 367.47 KiB, above the permanent B8b ceiling of 320 KiB.

The ceiling was not relaxed. B11 instead added a Rolldown code-splitting group for the stable React runtime boundary (`react`, `react-dom` and `scheduler`). The final carrier verification then passed:

- Quality run `35540478667`: success;
- current-tree security run `35540478792`: success;
- frontend entry chunk: 153.41 KiB;
- React runtime chunk: 213.70 KiB;
- frontend tests: 60 / 60;
- backend tests: 16 / 16;
- backend warnings: 0;
- npm advisories: 0.

This preserves the B8b performance contract while making React 19 independently cacheable from application code.

## Permanent Quality boundary

The main Quality workflow now treats the following as maintained source authority:

- React 19.3.0;
- ReactDOM 19.3.0;
- Testing Library React 16.3.3;
- React Router 7.18.4.

It also rejects reintroduction of the B11 retired frontend runtime packages:

- `@mercadopago/sdk-react`;
- `@react-google-maps/api`;
- `web-vitals`.

The existing B10 warning gate remains active, so frontend tests must stay free of `ReactDOMTestUtils.act`.

## Scope boundary

B11 does not redesign checkout or add a new payment UI.

The frontend Mercado Pago React wrapper was not reachable from the maintained product graph. If a future checkout requires Mercado Pago Bricks or Secure Fields, that integration should be introduced as a deliberate feature with current SDK contracts and dedicated behavior tests rather than by preserving the old unused package declaration.


## Closure

B11 is complete on the modernization carrier. The temporary `.github/workflows/b11-react19.yml` laboratory workflow is retired at closure; the permanent `Quality` and current-tree security workflows remain the authority for React 19.
