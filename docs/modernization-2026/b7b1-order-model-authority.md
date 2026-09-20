# B7b1 — Canonical order model authority

B7b1 starts from the post-B7a carrier:

`f8bf921960d654aab62c43536fe8fad8e40f5247`

B7a had reduced backend nullable warnings from 17 unique findings to 11,
all of them `CS8618`. Five of those warnings came from three English-named
order types that duplicated the active Spanish order domain.

## Characterization

The active persistence and service authority is:

- `Models/Orden.cs`;
- `Models/OrdenDetalle.cs`;
- `Models/Dto/OrdenDto.cs`.

`AppDbContext` maps `Orden` and `OrdenDetalle`, and the maintained order,
cart and sale flows use those Spanish domain types.

The repository also still contained:

- `Models/Order.cs`;
- `Models/OrderItem.cs`;
- `Models/Dto/OrderDto.cs`.

Those English types were not part of the active EF model or maintained order
flow and generated five nullable-reference warnings.

Validated lab branch:

`modernize/b7b1-order-model-authority-lab`

## Change

B7b1 removes the three duplicate English order types instead of hiding their
warnings with nullable annotations or null-forgiving initializers.

The permanent Quality contract now asserts both sides of this boundary:

- `Orden`, `OrdenDetalle` and `OrdenDto` must exist;
- `Order`, `OrderItem` and `OrderDto` must not return.

This turns the cleanup into an explicit source-authority decision.

## Validation

The B7b1 lab compiles and exercises the full maintained application without the
three removed files.

Expected compiler boundary after the removal:

- total unique backend warnings: 6;
- warning code: `CS8618` only;
- `Contacto.cs`: 4;
- `Envio.cs`: 2.

The lab also validates together:

- frontend ESLint with zero warnings;
- all maintained frontend tests;
- Vite production build;
- backend Release build and tests;
- current-tree security baseline;
- frontend full and production npm audits;
- NuGet vulnerability report;
- clean tracked repository state.

Permanent Quality is tightened from the B7a baseline of 11 `CS8618` warnings
to exactly 6, and additionally freezes the two remaining warning-bearing files.

## Next boundary

B7b2 can close the final six compiler warnings in `Contacto` and `Envio`.

Those fields represent required scalar data rather than optional relationships,
so the next change should make their non-null initialization contract explicit
without weakening their domain/API semantics merely to silence the compiler.
