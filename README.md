# NOpenCode

[![NuGet](https://img.shields.io/nuget/v/NOpenCode?style=flat-square&logo=nuget)](https://www.nuget.org/packages/NOpenCode)
[![CI](https://img.shields.io/github/actions/workflow/status/ylvict/NOpenCode/ci.yml?style=flat-square&logo=github)](https://github.com/ylvict/NOpenCode/actions/workflows/ci.yml)
[![Examples](https://img.shields.io/github/actions/workflow/status/ylvict/NOpenCode/examples.yml?style=flat-square&logo=github&label=examples)](https://github.com/ylvict/NOpenCode/actions/workflows/examples.yml)
[![Integration](https://img.shields.io/github/actions/workflow/status/ylvict/NOpenCode/integration-tests.yml?style=flat-square&logo=github&label=integration)](https://github.com/ylvict/NOpenCode/actions/workflows/integration-tests.yml)
[![README](https://img.shields.io/badge/README-%E4%B8%AD%E6%96%87-blue)](./README.zh.md)

Empower your .NET applications with [OpenCode](https://opencode.ai)'s AI engine. Express your intent in natural language — NOpenCode bridges your application logic with AI.

## ✨ Features

- **Zero setup** — auto-discovers or starts `opencode serve` for you
- **Plain-English API** — reads like natural language
- **Multi-turn sessions** — ask follow-up questions in context
- **Streaming** — receive responses chunk by chunk
- **File & code search** — ripgrep, filename lookup, symbol search
- **Session management** — fork, share, diff, revert, summarize
- **Provider & model discovery** — list available models, providers, agents
- **MCP server management** — add and list MCP servers
- **Event monitoring** — subscribe to real-time SSE events
- **DI integration** — `AddNOpenCode()` for ASP.NET Core / console hosts
- **.NET Standard 2.0** — compatible with .NET Framework 4.6.1+ and all modern .NET

## 📋 Prerequisites

- [OpenCode CLI](https://opencode.ai/docs/cli/) installed (`npm install -g opencode-ai`)
- .NET SDK 8+ (for consumption) or .NET 10 (for development)

## 📦 Installation

```bash
dotnet add package NOpenCode
```

## 🚀 Quick Start

```csharp
using NOpenCode;

string answer = await OpenCode.Ask("Explain how async/await works in C#");
Console.WriteLine(answer);
```

## 💡 Examples

### 🎯 One-shot

```csharp
string review = await OpenCode.Ask("What is the capital of France?");

// Or use the builder for full control
await using var ai = await OpenCode
    .Configure()
    .Launch();

var reply = await ai
    .Ask("What is the population of Paris?")
    .Execute();

Console.WriteLine(reply);
```

### 💬 Multi-turn

```csharp
await using var ai = await OpenCode
    .Configure()
    .Launch();

var session = await ai.NewSession("API design").Create();

var r1 = await session.Ask("What are the key differences between REST and GraphQL?");
var r2 = await session.Ask("When would you choose one over the other?");
```

### 🔄 Streaming

```csharp
await session.AskStream(
    "Write a brief README for a .NET library called NOpenCode.",
    onChunk: chunk => Console.Write(chunk),
    onComplete: reply => Console.WriteLine($"\nDone, tokens: {reply.GetUsage()?.Total}"),
    onError: ex => Console.WriteLine($"Error: {ex.Message}")
);
```

### 📋 Session lifecycle

```csharp
var session = await ai.NewSession("Code review").Create();
await session.Ask("Review this code for potential bugs.");

// Fork into a new branch
var fork = await session.Fork();
await fork.Ask("Focus on security issues only.");

// Get the diff
var diff = await fork.GetDiff();
foreach (var d in diff)
    Console.WriteLine($"[{d.Type}] {d.Path}");

// Share the session
await fork.Share();

// Clean up
await fork.Delete();
```

### 🔍 File & code search

```csharp
var todos = await ai.Files.Search("TODO|FIXME");
foreach (var match in todos)
    Console.WriteLine($"{match.Path}:{match.LineNumber}  {match.Lines?.Trim()}");

var files = await ai.Files.Find("*.cs");
var content = await ai.Files.Read("Program.cs");
```

### 🗺️ Discovery

```csharp
var models = await ai.Models.List(Providers.OpenCode);
var providers = await ai.Providers.List();
var agents = await ai.Agents.List();
var health = await ai.Diagnostics.GetHealth();
```

### 🔌 MCP server management

```csharp
await ai.Mcp.Add("filesystem", new
{
    type = "local",
    command = new[] { "npx", "-y", "@modelcontextprotocol/server-filesystem", "./" }
});

var servers = await ai.Mcp.List();
```

### 🧩 DI integration

```csharp
builder.Services.AddNOpenCode();

public class ReviewService(OpenCodeClient AI)
{
    public async Task RunAsync()
    {
        var reply = await AI
            .Ask("What is the capital of France?")
            .Execute();
    }
}
```

### 📡 Real-time events

```csharp
var subscribed = ai.Events.Subscribe(
    onEvent: evt => Console.WriteLine($"[{evt.Type}] {evt.Data}"),
    onError: ex => Console.WriteLine($"Error: {ex.Message}")
);

var answer = await ai.Ask("What is 2+2?").Execute();
Console.WriteLine($"Answer: {answer}");

await Task.Delay(2000);
// subscribed completes when cancelled
```

## 📁 Project Structure

```
src/NOpenCode/              → .NET Standard 2.0 library
  OpenCode.cs               → Static entry point
  NOpenCodeBuilder.cs       → Fluent builder
  OpenCodeClient.cs         → Main client with 11 sub-clients
  OpenCodeSession.cs        → Session with full lifecycle
  AskOperation.cs           → One-shot query builder
  SessionBuilder.cs         → Session creation builder
  ServerManager.cs          → Auto-manages opencode serve
  Clients/                  → Domain-specific clients
  Models/                   → 14 DTOs
  Http/                     → HTTP client + SSE reader
  DependencyInjection/      → IServiceCollection extensions
  Exceptions/               → Custom exception hierarchy

examples/                   → 10 standalone example programs (net10.0)
  HelloWorld               → Simplest possible usage
  OneShot                  → Single query with builder
  MultiTurn                → Multi-turn conversation
  Streaming                → Streaming responses
  DI                       → DI integration with AddNOpenCode
  SessionLifecycle         → Fork, share, diff, delete
  FileSearch               → ripgrep, find, list, read
  Discovery                → List models, providers, agents, commands
  McpManagement            → Add and list MCP servers
  EventMonitor             → Real-time SSE events

tests/NOpenCode.Tests/      → xUnit tests
```

## 📄 License

MIT
