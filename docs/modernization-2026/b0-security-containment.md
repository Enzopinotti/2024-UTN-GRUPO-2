# B0-S — Current-tree security and generated-state containment

## Authority

Baseline `main` before containment:

`b63af0a8092ae4ce69aad4013affc6159cf7fafa`

This is a collaborative repository. The 2026 maintenance lane does not rewrite
or reattribute historical work.

## Why security containment precedes framework maintenance

The public baseline contains non-empty tracked values in configuration slots
used for SQL Server, JWT signing, Cloudinary, SMTP/email and Mercado Pago.

No historical values are reproduced here.

The same tracked appsettings blob also existed in generated .NET output.

## Current-tree action

B0-S:

- clears secret/account-specific values from maintained `appsettings.json`;
- documents environment-variable names only;
- removes tracked `bin/` and `obj/` output;
- expands `.gitignore` for generated/local state;
- adds a deterministic guard against reintroducing those classes of files or
  non-empty sensitive appsettings slots.

## Generated-state measurement

The baseline contained **123 tracked files** under backend
`bin/` or `obj/` directories.

Those files are generated build output, not source authority.

## External rotation boundary

This commit does **not** rewrite Git history, prove a historical credential was
live, rotate/revoke any provider credential or disclose credential values.

Provider-side rotation/revocation, if required, must be verified externally.

## Known baseline defects retained for B0/B1

`Frontend/antigal.client/package.json` contains unresolved merge conflict
markers and is invalid JSON.

B0-S deliberately does not guess how that merge should be resolved.

B0 will reconstruct the intended frontend dependency authority from branch
history before B1 makes a manifest repair.
