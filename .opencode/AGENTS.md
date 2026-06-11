# NOpenCode

## Project structure

| Path | Description |
|---|---|
| `src/NOpenCode/` | SDK source (netstandard2.0) |
| `tests/NOpenCode.Tests/` | Unit tests — project reference, run on every CI |
| `tests/NOpenCode.IntegrationTests/` | Integration tests — `<PackageReference Version="*" />` against published NuGet |
| `examples/` | Example projects (also NuGet reference) |

## CI workflows

| Workflow | Trigger | What it does |
|---|---|---|
| `ci.yml` | Push/PR to master | Builds solution, runs unit tests |
| `integration-tests.yml` | Daily + manual | Tests via published NuGet package |
| `release.yml` | Tag `v*` | Packs, pushes to NuGet, creates release |

---

See sub-files for detail:

- [conventions.md](./conventions.md) — coding conventions & common pitfalls
- [design.md](./design.md) — design decisions & model selection
- [workflows.md](./workflows.md) — local integration test verification & release process

## Session notes (2026-06-11)

### Git author identity
- Check `git config user.name` / `user.email` before committing; noreply addresses publish under a truncated username.
- Amend + force-push is needed to fix a wrong author after push.

### Cross-platform CLI discovery
- `where` is Windows-only; use `which` on Unix. Factor out OS detection with `RuntimeInformation.IsOSPlatform`.

### Error handling
- Empty `catch { }` blocks silently swallow failures. At minimum log the exception via the configured callback.
- When threading a logger through static methods, use a static field (`_globalLog`) set at entry point.

### DI & async deadlock
- `.GetAwaiter().GetResult()` on async code can deadlock in ASP.NET Core sync context.
- Wrap in `Task.Run(...)` to offload to thread pool as mitigation.

### SSE streaming
- Real streaming requires `HttpCompletionOption.ResponseHeadersRead` on the HTTP request.
- Use `SseReader` to parse `event:` / `data:` lines — do NOT buffer the entire response and call `onChunk` once.

### Testability
- Add `<InternalsVisibleTo Include="NOpenCode.Tests" />` to the SDK `.csproj` to enable testing internal types without public API exposure.

### Release steps
1. Bump `<Version>` in `src/NOpenCode/NOpenCode.csproj`
2. `dotnet pack -c Release -o nupkg` (verifies build)
3. `git add -A && git commit -m "bump version to X.Y.Z"`
4. `git tag vX.Y.Z`
5. `git push origin master && git push origin vX.Y.Z`
6. Wait for release workflow (check with `gh run list --workflow release.yml`)
7. Write release notes to a temp file, then `gh release edit vX.Y.Z --notes-file /tmp/release-notes.md`
8. If push rejected (dependabot pushed meanwhile): `git pull --rebase origin master`, then force-push the tag

### Pre-build verification
- Always build both SDK and test projects before committing: `dotnet build src/NOpenCode && dotnet build tests/NOpenCode.Tests`
