# B38 — ProductCategory authorization boundary

## Goal

Protect catalog-structure mutations without breaking the public category/product read surface.

## Characterization

Repository search showed maintained frontend consumption of:

- GET `api/ProductCategory/categorias/{idProducto}`
- GET `api/ProductCategory/productos/{idCategoria}`

No maintained frontend consumer was found for:

- POST `api/ProductCategory/asignar`
- DELETE `api/ProductCategory/desasignar`

Both mutations change catalog structure and were previously reachable without an explicit authorization policy.

## Maintained policy

`ProductCategoryController` is now:

`[Authorize(Roles = "Admin")]`

by default.

The two read endpoints explicitly override that default with `[AllowAnonymous]`.

Therefore:

- POST asignar: Admin
- DELETE desasignar: Admin
- GET categorias: anonymous
- GET productos: anonymous

Routes and response behavior were not changed.

## Runtime and source proof

Added:

- `ProductCategoryAuthorizationTests.cs`
- `scripts/backend_product_category_authorization_authority.py`

The runtime tests prove:

1. controller default role is Admin;
2. mutation endpoints do not override authorization;
3. the two maintained read endpoints remain explicitly anonymous.

The source gate requires exactly two anonymous actions and protects route templates.

## Validation

Final B38 head:

`cf6c9a8d4bbb499f37b785eda34850f37e914b22`

PR #28:

- Quality `35614490185` — success
- Security `35614490209` — success

Published SHA validation:

- Quality `35614660701` — success
- Security `35614660612` — success

## Result

Catalog reads stay public while catalog relationship mutations now require Admin authorization.
