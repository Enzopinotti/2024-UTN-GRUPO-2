# B8c — JSX extension authority

## Objective

Normalize frontend source extensions so JSX-bearing modules use `.jsx` and plain JavaScript remains `.js`.

This removes the temporary Vite/Oxc compatibility transform that allowed JSX inside legacy `.js` files.

## Validated boundary

The B8c inventory established the target state:

- 23 remaining `.js` files under `Frontend/antigal.client/src`
- 0 JSX-bearing `.js` files
- 0 parser failures
- 0 explicit imports that hard-code a `.js` extension

The remaining `.js` files are plain JavaScript utilities, data modules, setup code, or intentionally empty placeholders.

## Changes

- Renamed JSX-bearing application, route, component, context, and test modules to `.jsx`.
- Updated the HTML entrypoint to `/src/index.jsx`.
- Updated production dependency traversal to start from `src/index.jsx`.
- Removed the custom `transformWithOxc` / `jsxInLegacyJs` compatibility plugin from `vite.config.mjs`.
- Added `scripts/jsx_extension_authority.mjs` to reject JSX inside future `.js` files.
- Added the JSX extension authority check to the main Quality workflow.
- Preserved extensionless local imports so the renames do not require broad source rewrites.

## Acceptance gates

B8c is acceptable only when all of the following pass on the normalized tree:

1. `npm ci`
2. production dependency authority
3. JSX extension authority
4. ESLint with zero warnings
5. Vitest
6. Vite production build
7. the existing carrier Quality contract after promotion

## Non-goals

B8c does not change application behavior, routing semantics, API contracts, component logic, styling, or backend behavior. It is a source-authority and build-tooling cleanup.
