using NOpenCode;

// Discovery — list available models, providers, agents, and commands.
await using var ai = await OpenCode
    .Configure()
    .Launch();

// 1. List models for a specific provider
var models = await ai.Models.List(Providers.OpenCode);
Console.WriteLine("=== OpenCode Models ===");
models.ForEach(m => Console.WriteLine($"  {m.Id}"));

// 2. List all providers
var providers = await ai.Providers.List();
Console.WriteLine("\n=== Providers ===");
providers.All?.ForEach(p =>
    Console.WriteLine($"  {p.Id}  (connected: {p.Connected})"));

// 3. List available agents
var agents = await ai.Agents.List();
Console.WriteLine("\n=== Agents ===");
agents.ForEach(a => Console.WriteLine($"  {a.Name}  — {a.Description}"));

// 4. List slash commands
var commands = await ai.Commands.List();
Console.WriteLine("\n=== Commands ===");
commands.ForEach(c => Console.WriteLine($"  /{c.Name}  — {c.Description}"));

// 5. Health check
var health = await ai.Diagnostics.GetHealth();
Console.WriteLine($"\nServer health: {health.Healthy}  (v{health.Version})");
