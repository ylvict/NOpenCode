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

## Coding conventions

- **No comments in code.** If something needs explanation, make the code self-documenting or put context in the commit message.
- **Use `Providers.OpenCode`** instead of the string `"opencode"` in all call sites (examples, tests, SDK internals). Exception: do NOT change `ServerManager.cs` — the `"opencode"` there is the CLI executable name, not a provider id.
- **New code uses `WithModel(selector)` or `WithAnyFreeModel()`.** Do NOT use `WithModel(string)` in new code — it's `[Obsolete]` and triggers CS0618. The string overload exists only for backward compat.
- **Suppress obsolete warnings in csproj, not with `#pragma`.** Use `<NoWarn>$(NoWarn);CS0618</NoWarn>` in the project file if a test project intentionally uses the obsolete API.
- **Use `NOpenCodeBuilder.AnyFreeSelector`** to filter `Models.List()` results by free tier — don't inline the `-free` suffix predicate.
- **Assert with `Assert.False(string.IsNullOrWhiteSpace(...))`**, not `Assert.NotNull` + `Assert.NotEmpty`.

## Design decisions

- **`Providers` is a `static class` with `const string` fields, not an `enum`.** The opencode CLI supports 75+ providers and the list grows. An enum would lock the SDK to upstream's known list at compile time. Constants give IntelliSense for common providers while raw strings still work for unlisted ones.
- **Default model selection is `AnyFree`.** Before 0.2.0, `Launch()` used the server's default model, which was undefined and could break silently. Since 0.2.0, the SDK resolves the first `-free` model at launch time, which is resilient to upstream model renames. Users who want a specific model call `WithModel(selector)` or the obsolete `WithModel(string)`.
- **`WithModel(string)` is `[Obsolete]` (soft warning, not error).** The overload is kept so existing 0.1.x code compiles without changes. There is no plan to remove it.
- **`NOpenCodeBuilder.AnyFreeSelector` is public.** Consumers and tests filtering `Models.List()` results can reuse the same predicate the SDK uses internally for `WithAnyFreeModel()` and the default `Launch()`.

## Common pitfalls

- **`ServerManager.cs` line 144 (`return "opencode"`).** This is the CLI executable name, NOT the provider id `Providers.OpenCode`. Do NOT change it to `Providers.OpenCode`.
- **Don't add `#pragma warning disable` to test source files.** Use `<NoWarn>` in the `.csproj` instead — it's cleaner and applies to the whole project.
- **`configuration-tests.yml` uses `XDG_DATA_HOME`** to isolate the opencode CLI database per run. If you change the server-start logic, make sure to preserve the isolation (or the SQLite WAL migration can fail intermittently on fresh CI runners).

## Release process

1. Bump `<Version>` in `src/NOpenCode/NOpenCode.csproj`.
2. Commit and push to `master`.
3. Create and push a tag: `git tag v<version>` + `git push origin v<version>`.
4. The `release.yml` workflow packs, pushes to NuGet, and creates a GitHub release (auto-generated notes).
5. Edit the GitHub release notes manually for clarity — the auto-generated `--generate-notes` output is just commit titles.
