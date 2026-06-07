# Design decisions

## `Providers` is a static class, not an enum

The opencode CLI supports 75+ providers and the list grows. An enum would
lock the SDK to upstream's known list at compile time. Constants give
IntelliSense for common providers while raw strings still work for unlisted
ones.

## Default model selection is `AnyFree` (since 0.2.0)

Before 0.2.0, `Launch()` used the server's default model, which was
undefined and could break silently. Since 0.2.0, the SDK resolves the first
`-free` model at launch time, resilient to upstream renames. Pin with
`WithModel(selector)` or `WithModel(string)` (obsolete) to override.

## `WithModel(string)` is `[Obsolete]` — soft warning, not error

Kept so 0.1.x code compiles without changes. No plan to remove it.

## `NOpenCodeBuilder.AnyFreeSelector` is public

Consumers and tests filtering `Models.List()` results can reuse the same
predicate the SDK uses internally for `WithAnyFreeModel()` and the default
`Launch()`.
