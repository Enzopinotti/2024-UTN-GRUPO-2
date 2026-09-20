# B7b2 — Required scalar model contracts

B7b2 starts from carrier:

`11fdc7a83ab3069a0ba17855db30ecc423b9c32a` — B7b1 canonical order model authority.

At that point the maintained Release build was already reduced to exactly six
compiler warnings, all `CS8618`:

- `Models/Contacto.cs`: 4;
- `Models/Envio.cs`: 2.

Both models are persisted by EF Core and their corresponding migration/model
snapshot columns are non-nullable. The remaining warnings therefore represented
missing C# initialization contracts rather than optional domain data.

## Scope

B7b2 addresses only the six required scalar string properties:

`Contacto`:

- `Name`;
- `Email`;
- `Asunto`;
- `Mensaje`.

`Envio`:

- `Destinatario`;
- `Direccion`.

Validated lab branch:

`modernize/b7b2-required-scalar-models-lab`

## Change

The six properties now use the existing repository convention:

`public required string ... { get; set; }`

This keeps their reference type non-nullable and makes construction/deserialization
requirements explicit without:

- weakening the properties to nullable strings;
- manufacturing empty-string defaults;
- suppressing warnings with null-forgiving initializers;
- changing the database nullability contract.

## Behavioral protection

`RequiredScalarModelTests` protects the JSON boundary for both models.

The focused tests prove that:

- omitting a required `Contacto` member is rejected;
- omitting a required `Envio` member is rejected;
- explicitly supplied empty strings still deserialize as empty strings.

That last point intentionally preserves the previous non-nullable MVC contract
instead of silently adding stricter non-empty validation in a compiler-cleanup
block.

## Validation

Final reproducible lab validation:

- workflow run: `35522573553`;
- job: `106109078327`;
- conclusion: success.

The run proved together:

- frontend ESLint: 0 warnings;
- frontend tests: 64/64 green across 13 files;
- Vite 8.2.2 production build: green;
- backend Release build: green;
- backend compiler warnings: 0;
- backend tests: 16/16 green;
- current-tree security baseline: green;
- frontend full npm audit: 0 advisories;
- frontend production npm audit: 0 advisories;
- NuGet solution vulnerability report: clean;
- tracked repository state: clean.

Permanent Quality now requires a warning-free backend build. Any future C# compiler
warning causes the gate to fail instead of being absorbed into a warning-count
baseline.

## Result

B7 closes the nullable/compiler-warning cleanup lane:

- B7a removed the optional product-filter contract mismatch;
- B7b1 removed duplicate English order model authority;
- B7b2 makes the final required scalar initialization contracts explicit.

The maintained backend now compiles with zero warnings.

## Next boundary

The remaining visible modernization signals are no longer C# compiler warnings.
The production frontend build still reports two separate classes of tooling debt:

- the main JavaScript chunk is larger than Vite's 500 kB warning threshold;
- `@lottiefiles/react-lottie-player` contains direct `eval`, reported by Rolldown.

These should be handled separately because bundle code-splitting is an application
architecture concern while the `eval` warning originates in a third-party
runtime dependency.
