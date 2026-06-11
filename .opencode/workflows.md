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
2. Build + pack to verify: `dotnet pack src/NOpenCode/NOpenCode.csproj -c Release -o nupkg`
3. Build tests: `dotnet build tests/NOpenCode.Tests`
4. Commit: `git add -A && git commit -m "bump version to X.Y.Z"`
5. Tag: `git tag vX.Y.Z` + `git push origin master && git push origin vX.Y.Z`
6. Wait for the `release.yml` workflow to finish (check with `gh run list --workflow release.yml`).
7. Write release notes to a temp file, then update: `gh release edit vX.Y.Z --notes-file /tmp/release-notes.md`
8. The workflow packs, pushes to NuGet, and creates the GitHub release with auto-generated notes — override them in step 7.
9. If push is rejected (Dependabot pushed meanwhile), rebase first: `git pull --rebase origin master`, then force-push the tag.

## Git author check

Before committing, verify `git config user.name` and `user.email` are correct. A noreply GitHub email publishes under a truncated username. Fix a wrong author after push with:

```
git commit --amend --author="Your Name <email@example.com>" --no-edit
git push --force-with-lease origin master
git push --force-with-lease origin vX.Y.Z
```
