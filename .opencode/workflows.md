# Local integration test verification

Integration tests resolve `NOpenCode` from NuGet (`Version="*"`). To verify
changes locally **without publishing first**:

```powershell
dotnet pack src/NOpenCode/NOpenCode.csproj -c Release -o nupkg
dotnet nuget add source "$pwd\nupkg" --name local-nupkg
dotnet restore tests/NOpenCode.IntegrationTests --force-evaluate
dotnet build tests/NOpenCode.IntegrationTests
dotnet nuget remove source local-nupkg
```

The `nupkg/` folder is gitignored (`*.nupkg` in `.gitignore`).

# Release process

1. Bump `<Version>` in `src/NOpenCode/NOpenCode.csproj`.
2. Commit and push to `master`.
3. Tag: `git tag v<version>` + `git push origin v<version>`.
4. The `release.yml` workflow packs, pushes to NuGet, creates a GitHub release.
5. Edit the GitHub release notes manually — the auto-generated output is just commit titles.
