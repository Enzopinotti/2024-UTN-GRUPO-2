# Modernización 2026

Este directorio contiene la evidencia técnica y las decisiones del carril de modernización mantenido.

El trabajo se ejecutó en bloques pequeños, normalmente con:

1. reconciliación contra el remoto real;
2. caracterización del comportamiento existente;
3. branch/lab aislado;
4. cambio mínimo;
5. gates permanentes cuando la decisión debía quedar protegida;
6. Quality + Current-tree security;
7. limpieza de triggers/workflows temporales;
8. promoción por fast-forward no forzado;
9. validación exacta del SHA promovido;
10. commit documental de cierre.

## Estado

El último bloque cerrado antes de este checkpoint es **B44 — Order contract surface reduction**.

Checkpoint:

- frontend: 60/60
- backend: 90/90
- Release build: 0 warnings
- npm audit: clean
- NuGet vulnerability audit: clean

## Bloques recientes

- B21 — Email DI authority
- B22 — JWT expiry authority
- B23 — System.IdentityModel.Tokens.Jwt refresh
- B24 — dead ServiceToken removal
- B25 — repository DI / ownership authority
- B26 — ProductService repository authority
- B27 — ResponseDto method-local authority
- B28 — product persistence authority
- B29 — Order/Sale persistence + transaction authority
- B30 — dead UnitOfWork SaveChanges contract removal
- B31 — direct AppDbContext persistence boundary
- B32 — Like repository authority
- B33 — Contacto persistence boundary
- B34 — Admin bootstrap authority
- B35 — Image repository authority
- B36 — Like concurrency integrity
- B37 — Dead email test surface removal
- B38 — ProductCategory authorization boundary
- B39 — Payment authorization boundary
- B40 — Dead payment notification surface removal
- B41 — Cart ownership boundary
- B42 — Dead Sale API surface removal
- B43 — Orders authorization boundary
- B44 — Order contract surface reduction

Los archivos `bXX-*.md` de este directorio contienen la evidencia detallada, decisiones de alcance, validaciones, no-goals y rollback boundary de cada bloque.

## Próximos candidatos

- B45 candidate — remove the permanently disabled backend placeholder branch from the profile Orders page
- future user-order API, if needed, must be owner-bound rather than reusing the Admin Orders surface
- a real Mercado Pago webhook integration with authenticity verification before any public callback returns
- upgrades mayores pendientes que requieran pruebas de comportamiento
