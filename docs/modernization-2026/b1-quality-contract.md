# B1 — Reproducible quality contract

B1 promotes only the source reconstruction already proven in the disposable
lab.

## Frontend runtime

The maintained compatibility contract is:

- Node `24.20.0`;
- npm `11.19.0`;
- exact `.nvmrc` pin for local/CI parity.

Node 20 was used only for historical reproduction and is not retained as the
maintained runtime.

The historical frontend `package.json` remains unchanged. Its lockfile is
normalized for npm 11 so `npm ci` is deterministic on Node 24.

## Backend runtime

The reconstructed historical backend remains on its declared `net8.0`
target during B1.

B1 does not yet migrate the application framework. The permanent Quality
workflow proves restore/build on the .NET 8 SDK line before later runtime
maintenance is considered.

## Permanent CI

`Quality` now blocks maintained delivery on:

- exact Node/npm contract;
- valid/matching frontend package + lock root declarations;
- required reconstructed frontend/backend source presence;
- absence of unresolved Git conflict markers;
- exact `npm ci`;
- frontend production build;
- .NET solution restore;
- .NET release build;
- current-tree secret/generated-state security guard;
- clean Git tree after build.

Dependency advisories are captured as evidence but are **not yet treated as a
green audit**. B0 measured known frontend and NuGet vulnerabilities; B3 will
reduce/block them after behavior tests exist.

Likewise, B1 does not pretend the obsolete CRA starter test or the absent
backend test project provide coverage. Test authority belongs to B2.

## Action supply-chain boundary

Permanent workflow action revisions are SHA-pinned.

The temporary B0/B1 lab workflows are not part of the promoted carrier.
