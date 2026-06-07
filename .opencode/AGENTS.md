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
