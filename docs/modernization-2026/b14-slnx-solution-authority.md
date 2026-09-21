# B14 — SLNX solution authority

B14 starts from the completed B13 carrier:

`58656dbc26d47d4cd9909f3286e21b05fe054de0` — `B13: record carrier validation and closure`.

The B14 laboratory is developed on:

`modernize/b14-slnx-lab`.

## Objective

Replace the legacy Visual Studio `.sln` file with the modern XML-based `.slnx` solution format while preserving exactly the same three maintained .NET projects and all existing build, test, security and frontend contracts.

B14 is a solution-file authority migration only. It does not modify application code, project package references, target frameworks, business behavior, database migrations or API contracts.

## Why SLNX now

The carrier already runs on .NET SDK 10.0.401.

.NET 10 creates SLNX solutions by default, and the .NET CLI supports both reading SLNX files and migrating legacy SLN files through `dotnet sln <solution.sln> migrate`.

The repository therefore no longer needs the opaque legacy SLN representation as its maintained solution authority.

## Previous authority

Before B14 the repository tracked:

`Backend/antigal.server.sln`.

It contained exactly three projects:

- `Backend/antigal.server/antigal.server.csproj`;
- `Backend/EmailService/EmailService.csproj`;
- `Backend/antigal.server.Tests/antigal.server.Tests.csproj`.

The permanent Quality workflow used the legacy solution for:

- restore;
- release build;
- NuGet vulnerability auditing.

## New authority

B14 introduces:

`Backend/antigal.server.slnx`.

The SLNX authority contains exactly the same three projects and no solution folders or additional build-time projects.

The legacy:

`Backend/antigal.server.sln`

is removed so there cannot be two competing solution authorities.

Historical modernization documentation may continue mentioning the old SLN when describing the repository state at that earlier point in time.

## Permanent Quality boundary

B14 updates the permanent Quality workflow so:

- `Backend/antigal.server.slnx` must exist;
- `Backend/antigal.server.sln` must not exist;
- all three maintained project paths must be present in the SLNX;
- restore uses the SLNX;
- release build uses the SLNX;
- NuGet vulnerability auditing uses the SLNX;
- the .NET 10, warning-free compiler and 16/16 backend behavior gates remain unchanged.

## Laboratory validation

The B14 laboratory must prove:

- exact SDK 10.0.401 remains active;
- `dotnet sln Backend/antigal.server.slnx list` resolves exactly three projects;
- the listed projects are the server, EmailService and test project;
- frontend dependency, JSX, dead-source and Sass authorities remain clean;
- frontend lint/tests/build remain green;
- `dotnet restore Backend/antigal.server.slnx` succeeds;
- `dotnet build Backend/antigal.server.slnx -c Release --no-restore` succeeds with zero compiler warnings;
- backend tests remain 16 / 16 green;
- NuGet vulnerability audit through the SLNX is clean;
- current-tree security remains green;
- the repository remains clean after validation.

## Scope boundary

B14 does not:

- alter project references;
- upgrade NuGet packages;
- change target frameworks;
- change source files;
- introduce solution folders;
- alter Debug/Release behavior;
- modify historical documentation merely to replace old `.sln` mentions.

Any future project-structure or package modernization belongs to a separate block.

## Closure condition

B14 is ready for carrier promotion only when the laboratory is fully green, the permanent Quality contract owns the SLNX path, the temporary B14 workflow is retired, and carrier Quality plus current-tree security pass on the promoted commit.
