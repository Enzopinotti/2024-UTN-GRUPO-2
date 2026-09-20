# B9 — Dead frontend source authority

B9 starts from the completed B8c frontend source-format boundary.

The final B8c carrier is:

`b709eb6e9c65721c024ecb895bdff6a665a22e55` — `B8c: retire completed JSX inventory workflow`.

The B9 lab was developed on:

`modernize/b9-dead-source-cleanup-lab`.

## Objective

Remove frontend JavaScript/JSX source that is tracked but no longer participates in the maintained product, while preserving Vitest roots and the production import graph.

B9 does not redesign authentication, routing, checkout, profiles, or persistence. It only removes source authority proven unreachable and freezes that cleanliness as a permanent gate.

## Characterization

The initial B9 inventory measured:

- reachable production source files: 95;
- unreachable source files: 32;
- tracked empty source files: 6.

The unreachable set mixed legitimate external roots with real dead code.

Legitimate external roots are:

- Vitest test files discovered by filename;
- `src/setupTests.js`, loaded by Vitest configuration.

Everything else requires evidence before it can remain outside the production graph.

## Removed empty placeholders

Six tracked zero-byte modules had no runtime authority and were removed:

- `src/components/admin/AdminLayout.js`;
- `src/components/carts/Cart.js`;
- `src/components/carts/CartContainer.js`;
- `src/components/checkout/Checkout.js`;
- `src/components/checkout/CheckoutContainer.js`;
- `src/components/common/Button.js`.

## Removed orphaned legacy islands

A second pass added reverse-import evidence for every unreachable source file.

The following modules had no importers anywhere in the tracked JavaScript/JSX source graph and were removed:

- `src/api.js`;
- `src/components/Callback.jsx`;
- `src/components/auth/Login.jsx`;
- `src/components/auth/Logout.jsx`;
- `src/components/auth/Profile.jsx`;
- `src/components/auth/Registro.jsx`;
- `src/components/common/PasswordInput.jsx`;
- `src/components/common/ProtectedRoute.jsx`;
- `src/components/common/UserIcon.jsx`;
- `src/data/initialCategories.js`;
- `src/pages/Login.jsx`;
- `src/pages/Logout.jsx`;
- `src/pages/Perfil.jsx`;
- `src/pages/Registro.jsx`;
- `src/pages/auth/Unauthorized.jsx`;
- `src/utils/onSaleMock.js`.

This removes the abandoned Auth0 prototype sources without adding `@auth0/auth0-react` to package authority. The maintained application continues to use its current authentication implementation.

## Removed test-only utility island

`src/utils/productUtils.js` was not reachable from the product and was imported only by `src/utils/productUtils.test.js`.

Both files were removed together.

This reduces the frontend suite from 65 to 60 tests because five tests characterized a utility that no longer had any product caller. No reachable product behavior contract was removed.

## Final B9 boundary

Validated lab run:

- workflow run: `35536681927`;
- conclusion: success.

Final inventory:

- frontend source files: 109;
- production-reachable source files: 95;
- unreachable source files: 14;
- unexpected unreachable source files: 0;
- empty source files: 0;
- unresolved local imports: 0.

The remaining 14 unreachable files are exactly:

- 13 Vitest test modules;
- `src/setupTests.js`.

Validation also proved:

- direct production dependency authority: clean;
- JSX extension authority: clean;
- ESLint: zero warnings;
- frontend test files: 13/13 green;
- frontend tests: 60/60 green;
- Vite production build: green.

## Permanent gate

`scripts/dead_source_inventory.mjs` now fails if any of the following appear:

- a tracked empty JavaScript/JSX source module;
- an unresolved local JavaScript/JSX import;
- an unreachable source module that is not a Vitest test root or `setupTests.js`.

The main Quality workflow executes this gate after production dependency and JSX-extension authority checks.

## Follow-up boundary

The test run still emits React's deprecation warning for `ReactDOMTestUtils.act`.

A safe next block should modernize the frontend testing-library/runtime boundary first, eliminate the warning under React 18, and only then evaluate a React 19 migration separately.
