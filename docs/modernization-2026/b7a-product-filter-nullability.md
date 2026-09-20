# B7a — Product filter nullability boundary

B7a starts from carrier:

`a5cb3aea78b60f931fb68042d8a9f22e4ee2ec0b` — B6c3 React Hook Form compatibility.

At that point frontend static analysis was warning-free, frontend and NuGet dependency
audits were clean, and the Release backend build still exposed 17 unique nullable
reference warnings:

- 4 × `CS8625`;
- 2 × `CS8604`;
- 11 × `CS8618`.

## Scope

B7a addresses only the optional product-list filter contract.

The public endpoint already accepted omitted query parameters:

- `orden`;
- `precio`.

However, the controller exposed them as nullable while the service and repository
contracts still described them as non-nullable strings whose default value was
`null`. That mismatch produced the `CS8625` and `CS8604` warnings.

Validated lab branch:

`modernize/b7a-product-filter-nullability-lab`

## Changes

The optional filters are now explicitly nullable through the full call chain:

- `ProductController.GetProduct(string? orden = null, string? precio = null)`;
- `IProductService.GetProducts(string? orden = null, string? precio = null)`;
- `ProductService.GetProducts(string? orden = null, string? precio = null)`;
- `IProductRepository.GetProductsAsync(string? orden, string? precio)`;
- `ProductRepository.GetProductsAsync(string? orden, string? precio)`.

No filtering behavior changes:

- omitted filters still mean no ordering constraint;
- recognized values still apply the existing ordering rules;
- unknown values still fall through without applying that ordering.

Two focused controller contracts protect the boundary:

- omitted filters are forwarded as `null`;
- supplied filter values are forwarded unchanged.

## Validation

Final reproducible lab run:

- workflow run: `35516129719`
- job: `106092269993`
- conclusion: success

The run proved together:

- frontend ESLint: 0 warnings;
- frontend tests: 64/64 green across 13 files;
- Vite production build: green;
- backend Release build: green;
- backend tests: 12/12 green;
- current-tree security baseline: green;
- frontend full npm audit: 0 advisories;
- frontend production npm audit: 0 advisories;
- NuGet solution vulnerability report: clean;
- backend unique compiler warnings: 11;
- remaining warning code: `CS8618` only;
- `CS8625`: 0;
- `CS8604`: 0.

Permanent Quality now freezes the remaining compiler-warning boundary at exactly
11 unique `CS8618` findings. This prevents the removed nullable-contract warnings
from returning while the remaining model warnings are handled incrementally.

## Next boundary

The 11 remaining `CS8618` findings are concentrated in five historical model/DTO
files:

- `Models/Order.cs` — 2;
- `Models/OrderItem.cs` — 2;
- `Models/Dto/OrderDto.cs` — 1;
- `Models/Contacto.cs` — 4;
- `Models/Envio.cs` — 2.

The English `Order` / `OrderItem` / `OrderDto` types should first be checked
against the active Spanish `Orden` / `OrdenDetalle` model authority and removed
if they are dead compatibility leftovers. `Contacto` and `Envio` should be
handled separately so their non-null API/database semantics are made explicit
without weakening them to nullable values merely to silence the compiler.
