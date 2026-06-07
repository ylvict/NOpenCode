# Coding conventions

- **Keep code clean and self-documenting.** Comments are acceptable when the intent isn't obvious from the code alone — prefer explaining *why* over *what*.
- **Use `Providers.OpenCode`** instead of the string `"opencode"` in all call sites (examples, tests, SDK internals). Exception: do NOT change `ServerManager.cs` — the `"opencode"` there is the CLI executable name, not a provider id.
- **New code uses `WithModel(selector)` or `WithAnyFreeModel()`.** Do NOT use `WithModel(string)` in new code — it's `[Obsolete]` and triggers CS0618. The string overload exists only for backward compat.
- **Suppress obsolete warnings in csproj, not with `#pragma`.** Use `<NoWarn>$(NoWarn);CS0618</NoWarn>` in the project file if a test project intentionally uses the obsolete API.
- **Use `NOpenCodeBuilder.AnyFreeSelector`** to filter `Models.List()` results by free tier — don't inline the `-free` suffix predicate.
- **Assert with `Assert.False(string.IsNullOrWhiteSpace(...))`**, not `Assert.NotNull` + `Assert.NotEmpty`.

# Code style (preferences)

- **Early return to reduce nesting.** When logic has 3+ levels of nesting, flatten with guard clauses. Each early `return` / `continue` / `break` goes on its own line:
  ```csharp
  // good
  if (result?.Providers == null)
      return new List<ModelInfo>();

  // not
  if (result?.Providers == null) return new List<ModelInfo>();
  ```
- **Prefer `List<T>.ForEach` over `foreach`** when iterating a concrete `List<T>`:
  ```csharp
  // good
  list.ForEach(x => { ... });

  // avoid
  foreach (var x in list) { ... }
  ```
- **Prefer `SelectMany` + `.ToList()` over nested `foreach` + `Add`.** Build collections with LINQ fluent chains, not mutable accumulation:
  ```csharp
  // good
  return result.Providers
      .Where(p => p.Models != null)
      .SelectMany(p => p.Models!.Values.Select(m => new ModelInfo { ... }))
      .ToList();

  // avoid
  var models = new List<ModelInfo>();
  foreach (var p in result.Providers) { models.Add(...); }
  return models;
  ```

# Common pitfalls

- **`ServerManager.cs` line 144 (`return "opencode"`).** This is the CLI executable name, NOT the provider id `Providers.OpenCode`. Do NOT change it to `Providers.OpenCode`.
- **Don't add `#pragma warning disable` to test source files.** Use `<NoWarn>` in the `.csproj` instead.
- **`integration-tests.yml` uses `XDG_DATA_HOME`** to isolate the opencode CLI database per run. If you change the server-start logic, preserve the isolation or the SQLite WAL migration can fail intermittently on fresh CI runners.
