# Coding conventions

- **No comments in code.** Make the code self-documenting or put context in the commit message.
- **Use `Providers.OpenCode`** instead of the string `"opencode"` in all call sites (examples, tests, SDK internals). Exception: do NOT change `ServerManager.cs` — the `"opencode"` there is the CLI executable name, not a provider id.
- **New code uses `WithModel(selector)` or `WithAnyFreeModel()`.** Do NOT use `WithModel(string)` in new code — it's `[Obsolete]` and triggers CS0618. The string overload exists only for backward compat.
- **Suppress obsolete warnings in csproj, not with `#pragma`.** Use `<NoWarn>$(NoWarn);CS0618</NoWarn>` in the project file if a test project intentionally uses the obsolete API.
- **Use `NOpenCodeBuilder.AnyFreeSelector`** to filter `Models.List()` results by free tier — don't inline the `-free` suffix predicate.
- **Assert with `Assert.False(string.IsNullOrWhiteSpace(...))`**, not `Assert.NotNull` + `Assert.NotEmpty`.

# Common pitfalls

- **`ServerManager.cs` line 144 (`return "opencode"`).** This is the CLI executable name, NOT the provider id `Providers.OpenCode`. Do NOT change it to `Providers.OpenCode`.
- **Don't add `#pragma warning disable` to test source files.** Use `<NoWarn>` in the `.csproj` instead.
- **`integration-tests.yml` uses `XDG_DATA_HOME`** to isolate the opencode CLI database per run. If you change the server-start logic, preserve the isolation or the SQLite WAL migration can fail intermittently on fresh CI runners.
