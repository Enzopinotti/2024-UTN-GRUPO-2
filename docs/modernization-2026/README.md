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

El último bloque cerrado antes de este checkpoint es **B33 — Contacto persistence boundary**.

Checkpoint:

- frontend: 60/60
- backend: 52/52
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

Los archivos `bXX-*.md` de este directorio contienen la evidencia detallada, decisiones de alcance, validaciones, no-goals y rollback boundary de cada bloque.

## Próximos candidatos

- DbInitializer development bootstrap authority
- Likes unique constraint/concurrency integrity, previa caracterización de datos
- ImageService direct-context boundary
- upgrades mayores pendientes que requieran pruebas de comportamiento
