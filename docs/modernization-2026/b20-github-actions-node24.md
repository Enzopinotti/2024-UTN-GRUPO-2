# B20 — GitHub Actions Node 24 runtime authority

## Why this block exists

The post-B19 reconciliation found no current npm or NuGet vulnerability debt, but
the maintained CI workflows were still pinned to action revisions whose
`action.yml` runtime was Node 20. GitHub's hosted runner was already forcing
those actions to Node 24 and emitted an explicit Node 20 deprecation warning.

This is a CI-runtime maintenance block, not an application dependency refresh.

Carrier characterized before B20:

- carrier: `modernize/2026-authorship-security-baseline`
- starting SHA: `5dc8b36f9109657ac475802d9cbb5f59ae22c58e`
- Quality: `35554127233` — success
- Current-tree security: `35554127232` — success
- carrier vs main: 169 commits ahead, 0 behind
- no open modernization PRs
- no post-B19 carrier commit to reconcile

## Characterization evidence

A disposable post-B19 diagnostic branch ran:

- commit: `27cf18bf211d7037124efbadca58b2cd32981bf5`
- run: `35556489203` — success
- npm audit: 0 findings
- production npm audit: 0 findings
- NuGet vulnerability audit: 0 vulnerable packages
- production dependency authority: clean across 95 reachable modules
- unexpected unreachable frontend source: 0
- current-tree security baseline: clean

The same run emitted:

`Node.js 20 is deprecated. The following actions target Node.js 20 but are being forced to run on Node.js 24`

The affected maintained pins were checkout, setup-node and setup-dotnet. The old
setup-node cleanup also emitted Node deprecation noise from `punycode` and
`url.parse()`.

## Maintained action boundary

B20 preserves SHA pinning and moves to current released action revisions that
declare `using: node24`:

| Action | Previous pin | B20 pin | Release |
| --- | --- | --- | --- |
| actions/checkout | v4/v5-era pins | `3d3c42e5aac5ba805825da76410c181273ba90b1` | v7.0.1 |
| actions/setup-node | `49933ea5288caeca8642d1e84afbd3f7d6820020` | `820762786026740c76f36085b0efc47a31fe5020` | v7.0.0 |
| actions/setup-dotnet | `67a3573c9a986a3f9c594539f4ab511d57bb3ce9` | `a98b56852c35b8e3190ac28c8c2271da59106c68` | v6.0.0 |
| actions/upload-artifact | `ea165f8d65b6e75b540449e92b4886f43607fa02` | `043fb46d1a93c77aae656e7c1c64a875d1fc6a0a` | v7.0.1 |

The exact Node/npm/.NET product toolchain is unchanged:

- Node 24.20.0
- npm 11.19.0
- .NET SDK 10.0.401

## Permanent Quality contract

Quality now checks the action `uses:` entries in both maintained workflow files
and requires the Node-24-compatible SHA pins above. It rejects the retired pins.

The gate deliberately parses only real YAML `uses:` entries. Earlier lab
iterations incorrectly scanned its own literal evidence or over-escaped the
regex; those failures were classified as CI-harness failures, not application
or dependency failures.

## Lab validation

Validated implementation head before cleanup:

- SHA: `d9e408538cb14530834b690a6ccd911c441e8157`
- Quality: `35556856359` — success
- Current-tree security: `35556856348` — success
- frontend tests: 60/60
- backend tests: 22/22
- frontend build: success
- backend Release build: warning-free
- npm audits: clean
- NuGet vulnerability audit: clean
- action authority gate: `github-actions-runtime-authority=node24`
- Node 20 deprecation warning: absent
- `DEP0040` punycode warning from old actions: absent
- `DEP0169` url.parse warning from old actions: absent

The failed intermediate Quality runs were:

- `35556686536`: static gate matched obsolete SHA literals inside its own script
- `35556763660`: action-use parser did not yet match the YAML form correctly
- `35556804278`: embedded regex escaping still represented literal backslashes

No product code or dependency version was changed in response to those failures.

## Explicit non-goals

B20 does not:

- upgrade Vite or jsdom;
- upgrade System.IdentityModel.Tokens.Jwt;
- reopen NPOI 2.8;
- change React, Router, Sass or application source;
- change Node/npm/.NET runtime versions;
- relax any existing Quality/security boundary;
- change deployment behavior or merge the carrier to main.

## Rollback boundary

The block is fully reversible by restoring the previous workflow action SHAs and
removing the B20 action-authority check. No application data, schema or runtime
behavior is migrated.

Temporary lab branch triggers are removed before promotion. Final closure still
requires Quality and Current-tree security to pass again on the exact promoted
carrier SHA.


## Carrier promotion evidence

The cleaned B20 tree was promoted by non-forced fast-forward to the maintained
carrier.

Promoted carrier SHA:

`7cc0ab397e4cbc88559d9e975bdbb21c047adc77`

Exact-SHA carrier validation:

- Quality `35557044268` — success
- Current-tree security `35557044194` — success
- CI action runtime authority: Node 24
- frontend tests: 60/60
- backend tests: 22/22
- frontend build: success
- backend Release build: 0 warnings
- npm audits: 0 findings
- NuGet vulnerability audit: clean

A documentation-only closure commit follows this evidence. B20 is considered
closed only after the permanent Quality and Current-tree security workflows pass
again on that exact closure SHA.
