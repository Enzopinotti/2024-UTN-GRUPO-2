# B4b — Routing behavior contracts

B4a reduced frontend production dependency risk without changing framework
majors. The only remaining production npm advisories are moderate findings in
React Router 6, whose offered remediation is React Router 7.

Before testing that major migration, B4b establishes explicit navigation
contracts around the current application.

## Scope

The tests exercise the real `App.js` router composition with the installed
`BrowserRouter`, `Routes`, `Route` and nested `Outlet` behavior.

UI-heavy leaf components, providers and external-integration surfaces are
mocked so routing tests do not require:

- backend/network availability;
- SQL Server;
- Mercado Pago;
- Cloudinary;
- SMTP;
- browser layout APIs.

No product source is changed by this block.

## Protected routes

The contract covers:

- shared header/main/footer shell;
- home;
- product list;
- product detail parameter matching;
- cart;
- profile index;
- profile orders/favorites/addresses;
- admin categories/products/users/messages;
- login;
- password reset/recovery;
- register/logout;
- about/store/contact/privacy;
- email confirmation;
- registration success;
- checkout;
- wildcard not-found behavior.

Nested profile/admin tests require their real router nesting semantics via
`Outlet`.

## Migration gate

A React Router 7 candidate is not eligible for promotion unless these route
contracts remain green together with the existing utility and backend behavior
suite.

Likewise, a future CRA replacement must preserve these application route
contracts independently of its build tooling.
