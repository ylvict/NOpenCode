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

## DI registration uses `Task.Run` to avoid sync context deadlock

`AddNOpenCode()` calls `builder.Launch()` (async) during service registration.
Because `IServiceCollection` registration is synchronous, the async call is
resolved with `.GetAwaiter().GetResult()`. To avoid deadlocking in ASP.NET
Core's synchronization context, the call is wrapped in `Task.Run(...)` to
offload it to the thread pool.

```csharp
services.AddSingleton<OpenCodeClient>(_ =>
{
    return Task.Run(builder.Launch).GetAwaiter().GetResult();
});
```

## SSE streaming uses `PostStream` + `SseReader`, not `Post<T>`

The `AskStream` family of methods performs real SSE streaming. The request
body includes `"stream": true` and the response is read via
`HttpCompletionOption.ResponseHeadersRead`. Each SSE `event:` / `data:` line
is parsed by `SseReader`. Three event types are supported:

| Event | Handling |
|---|---|
| `chunk` | Deserialized as `Part`; if `type == "text"`, `onChunk(text)` is called |
| `complete` | Deserialized as `OpenCodeReply`; `onComplete(reply)` is called |
| `error` | `NOpenCodeException` is thrown |

The old implementation (v0.2.x) called the non-streaming `Post<T>` endpoint
and invoked `onChunk` once with the full text — this was fake streaming and
has been removed.
