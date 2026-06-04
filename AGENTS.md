# NOpenCode — Agent Guide

## Project structure

- `src/NOpenCode/` — SDK source (netstandard2.0)
- `tests/NOpenCode.Tests/` — unit tests (project reference, run on every CI)
- `tests/NOpenCode.IntegrationTests/` — integration tests against the **published NuGet package** (`<PackageReference Version="*" />`)
- `examples/` — example projects (also use `<PackageReference Version="*" />`)

## Integration tests & local verification

Integration tests resolve `NOpenCode` from NuGet (the `*` wildcard picks the
latest published version). If you change the SDK and want to verify the
integration tests locally **without publishing to NuGet first**:

```powershell
# 1. Bump the version (or keep existing dev version)
# 2. Pack the SDK to the local nupkg folder
dotnet pack src/NOpenCode/NOpenCode.csproj -c Release -o nupkg

# 3. Add the local folder as a NuGet source
dotnet nuget add source "$pwd\nupkg" --name local-nupkg

# 4. Restore & build the integration tests (they'll pick up the local package)
dotnet restore tests/NOpenCode.IntegrationTests --force-evaluate
dotnet build tests/NOpenCode.IntegrationTests

# 5. Clean up the local source when done
dotnet nuget remove source local-nupkg
```

The `nupkg/` folder is gitignored (`*.nupkg` in `.gitignore`). Team members
who don't run integration tests locally can skip this setup entirely.

## CI workflows

| Workflow | Trigger | What it does |
|---|---|---|
| `ci.yml` | Push/PR to master | Builds solution, runs unit tests only |
| `integration-tests.yml` | Scheduled (daily) + manual | Builds integration tests against latest NuGet, starts opencode server, runs all tests |
| `release.yml` | Tag `v*` | Packs, pushes to NuGet, creates GitHub release |

## Default model selection (since 0.2.0)

`OpenCode.Configure().Launch()` auto-selects the first free-tier model
(`-free` suffix) from the opencode provider at launch time. To pin a
specific model, use `WithModel(string)` (obsolete but kept for backward
compatibility) or `WithModel(Func<ModelInfo, bool>, string provider)`.
