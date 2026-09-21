# B17 — Remove direct ImageSharp authority

B17 starts from the completed B16 carrier:

`9930aa41174d07c0945c8629b33a2a16384e9b4a` — `B16: record carrier validation and closure`.

The B17 laboratory is developed on:

`modernize/b17-remove-imagesharp-lab`.

## Objective

Remove the backend's direct `SixLabors.ImageSharp` dependency when the maintained application source does not use ImageSharp APIs, while preserving the ImageSharp version that NPOI legitimately brings transitively.

B15 had deferred a direct ImageSharp major upgrade from 2.1.13 to 4.x. B17 revisits that assumption before performing a breaking upgrade.

## Source authority finding

The maintained backend contains no direct ImageSharp usage.

The image application service is implemented with Cloudinary:

- `CloudinaryDotNet`;
- `CloudinaryDotNet.Actions`;
- `ImageUploadParams`;
- `FileDescription`;
- `DeletionParams`.

The B17 lab rejects maintained C# source containing direct ImageSharp namespace/API markers such as:

- `SixLabors.ImageSharp`;
- `using SixLabors`;
- `Image.Load`;
- typed `Image<TPixel>` usage.

The laboratory gate reports:

- `b17_imagesharp_direct_authority=absent`;
- `b17_imagesharp_source_usage=absent`.

## Dependency graph finding

Removing the direct project reference does not remove ImageSharp completely from the resolved NuGet graph.

NPOI 2.7.6 declares `SixLabors.ImageSharp >= 2.1.11`, so the restored graph resolves:

- direct ImageSharp authority: absent;
- transitive owner: NPOI 2.7.6;
- transitive ImageSharp version: 2.1.11.

This is intentional.

B17 does not pin or upgrade that transitive dependency independently of NPOI because the application does not consume ImageSharp directly and B15 already established NPOI 2.7.6 as the maintained dependency boundary.

## Why removal is preferable to a direct 4.x upgrade

The application does not need a direct ImageSharp API surface.

Keeping a direct package only to override NPOI would:

- increase application-owned dependency surface;
- make the project responsible for a major API boundary it does not use;
- couple the application to a transitive implementation detail of NPOI;
- create an unnecessary direct ImageSharp 4.x licensing/build boundary.

ImageSharp 4.x licensing enforcement applies to projects that directly depend on ImageSharp. B17 therefore keeps ownership aligned with actual usage: NPOI owns its transitive requirement, and Antigal does not declare ImageSharp directly.

## Permanent Quality boundary

Quality now requires:

- no `SixLabors.ImageSharp` PackageReference in `Backend/antigal.server/antigal.server.csproj`;
- the package to remain in the forbidden direct-server-package set;
- `backend-imagesharp-authority=absent` in the integrated authority evidence;
- all B16 and earlier frontend, backend, runtime, security and dependency gates to remain green.

## Laboratory evolution

Initial B17 run:

`35549671127` — failed only in the graph-inspection helper.

Before that helper failed, the candidate had already demonstrated:

- no maintained source usage of ImageSharp;
- frontend authorities green;
- frontend tests and build green;
- backend restore green;
- backend release build warning-free;
- backend tests 18 / 18 green.

The helper incorrectly treated every `dotnet package list --include-transitive` row prefixed with `>` as top-level. The command uses the same row marker in separate Top-level and Transitive sections.

The gate was corrected to parse those sections explicitly.

## Validated laboratory evidence

Final B17 laboratory run:

`35549759039` — success on `a6adcf97255483af39577e750ba07de59caa1398`.

Validated signals:

- ImageSharp direct PackageReference absent;
- ImageSharp maintained source usage absent;
- frontend lint green;
- frontend tests: 60 / 60 green across 13 files;
- Vite 8.2.2 production build green, 460 modules transformed;
- backend restore green through `antigal.server.slnx`;
- backend release build: 0 warnings, 0 errors;
- backend tests: 18 / 18 green;
- ImageSharp transitive owner recorded as NPOI 2.7.6;
- ImageSharp transitive version recorded as 2.1.11;
- NuGet vulnerability audit clean for server, tests and EmailService;
- current-tree security baseline green;
- repository remains clean after validation.

## Scope boundary

B17 does not:

- change Cloudinary behavior;
- change upload/delete endpoints;
- alter image persistence;
- upgrade NPOI;
- force ImageSharp 4.x into the transitive graph;
- change Mercado Pago;
- change Swashbuckle;
- alter application behavior.

## Closure

B17 is ready for promotion after the temporary laboratory workflow is retired and the carrier passes permanent Quality and current-tree security.
