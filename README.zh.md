# NOpenCode

[![NuGet](https://img.shields.io/nuget/v/NOpenCode?style=flat-square&logo=nuget)](https://www.nuget.org/packages/NOpenCode)
[![CI](https://img.shields.io/github/actions/workflow/status/ylvict/NOpenCode/ci.yml?style=flat-square&logo=github)](https://github.com/ylvict/NOpenCode/actions/workflows/ci.yml)
[![English](https://img.shields.io/badge/-English-dodgerblue?style=flat-square)](./README.md)

为你的 .NET 应用注入 [OpenCode](https://opencode.ai) 的 AI 引擎。用自然语言描述复杂的业务问题——NOpenCode 无缝桥接你的应用逻辑与 AI。

## 功能特性

- **零配置** — 自动发现或启动 `opencode serve`
- **自然语言 API** — 读起来就像自然语言
- **多轮对话** — 在上下文中追问后续问题
- **流式响应** — 逐块接收回复
- **文件和代码搜索** — ripgrep、文件名查找、符号搜索
- **会话管理** — 分支、分享、比较、回退、总结
- **提供方和模型发现** — 列出可用模型、提供方、智能体
- **MCP 服务器管理** — 添加和列出 MCP 服务器
- **事件监控** — 订阅实时 SSE 事件
- **DI 集成** — ASP.NET Core / 控制台主机使用 `AddNOpenCode()`
- **.NET Standard 2.0** — 兼容 .NET Framework 4.6.1+ 及所有现代 .NET

## 前置条件

- 已安装 [OpenCode CLI](https://opencode.ai/docs/cli/) (`npm install -g opencode-ai`)
- .NET SDK 8+（运行时）或 .NET 10（开发环境）

## 安装

```bash
dotnet add package NOpenCode
```

## 快速开始

```csharp
using NOpenCode;

string answer = await OpenCode.Ask("用中文解释 C# 中 async/await 的工作原理");
Console.WriteLine(answer);
```

## 示例

### 带配置的单次问答

```csharp
string review = await OpenCode.Ask(
    "法国的首都是哪里？",
    cfg => cfg.WithModel("opencode/deepseek-v4-flash-free")
);

// 或使用构建器获取完全控制
await using var ai = await OpenCode
    .Configure()
    .WithModel("opencode/deepseek-v4-flash-free")
    .Launch();

var reply = await ai
    .Ask("巴黎的人口是多少？")
    .Execute();

Console.WriteLine(reply);
```

### 多轮对话

```csharp
await using var ai = await OpenCode
    .Configure()
    .WithModel("opencode/mimo-v2.5-free")
    .Launch();

var session = await ai.NewSession("API 设计").Create();

var r1 = await session.Ask("REST 和 GraphQL 的主要区别是什么？");
var r2 = await session.Ask("什么时候应该选择哪一种？");
```

### 流式响应

```csharp
await session.AskStream(
    "为一个名为 NOpenCode 的 .NET 库写一个简短的 README。",
    onChunk: chunk => Console.Write(chunk),
    onComplete: reply => Console.WriteLine($"\n完成，Token 数：{reply.Usage?.Total}"),
    onError: ex => Console.WriteLine($"错误：{ex.Message}")
);
```

### 会话生命周期

```csharp
var session = await ai.NewSession("代码审查").Create();
await session.Ask("审查这段代码，找出潜在 Bug。");

// 创建分支
var fork = await session.Fork();
await fork.Ask("仅关注安全问题。");

// 获取差异
var diff = await fork.GetDiff();
foreach (var d in diff)
    Console.WriteLine($"[{d.Type}] {d.Path}");

// 分享会话
await fork.Share();

// 清理
await fork.Delete();
```

### 文件和代码搜索

```csharp
var todos = await ai.Files.Search("TODO|FIXME");
foreach (var match in todos)
    Console.WriteLine($"{match.Path}:{match.LineNumber}  {match.Lines?.Trim()}");

var files = await ai.Files.Find("*.cs");
var content = await ai.Files.Read("Program.cs");
```

### 发现

```csharp
var models = await ai.Models.List("opencode");
var providers = await ai.Providers.List();
var agents = await ai.Agents.List();
var health = await ai.Diagnostics.GetHealth();
```

### MCP 服务器管理

```csharp
await ai.Mcp.Add("filesystem", new
{
    type = "local",
    command = new[] { "npx", "-y", "@modelcontextprotocol/server-filesystem", "./" }
});

var servers = await ai.Mcp.List();
```

### DI 集成

```csharp
builder.Services.AddNOpenCode(cfg => cfg
    .WithModel("opencode/deepseek-v4-flash-free")
);

public class ReviewService(OpenCodeClient AI)
{
    public async Task RunAsync()
    {
        var reply = await AI
            .Ask("法国的首都是哪里？")
            .Execute();
    }
}
```

### 实时事件

```csharp
var count = 0;
var cts = new CancellationTokenSource();

await ai.Events.Subscribe(
    onEvent: evt =>
    {
        Console.WriteLine($"[{evt.Type}] {evt.Data}");
        if (++count >= 5) cts.Cancel();
    },
    onError: ex => Console.WriteLine($"错误：{ex.Message}")
);
```

## 项目结构

```
src/NOpenCode/              → .NET Standard 2.0 库
  OpenCode.cs               → 静态入口点
  NOpenCodeBuilder.cs       → Fluent 构建器
  OpenCodeClient.cs         → 主客户端，包含 11 个子客户端
  OpenCodeSession.cs        → 会话，支持完整生命周期
  AskOperation.cs           → 单次查询构建器
  SessionBuilder.cs         → 会话创建构建器
  ServerManager.cs          → 自动管理 opencode serve
  Clients/                  → 领域特定客户端
  Models/                   → 14 个 DTO
  Http/                     → HTTP 客户端 + SSE 读取器
  DependencyInjection/      → IServiceCollection 扩展
  Exceptions/               → 自定义异常层次

examples/                   → 10 个示例项目 (net10.0)
  01-HelloWorld
  02-OneShot
  03-MultiTurn
  04-Streaming
  05-DI
  06-SessionLifecycle
  07-FileSearch
  08-Discovery
  09-McpManagement
  10-EventMonitor

tests/NOpenCode.Tests/      → xUnit 测试
```

## 许可证

MIT
