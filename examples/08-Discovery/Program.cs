using NOpenCode;

// Discovery — list available models, providers, agents, and commands.
await using var ai = await OpenCode
    .Configure()
    .Launch();

// 1. List models for a specific provider
var models = await ai.Models.List(Providers.OpenCode);
Console.WriteLine("=== OpenCode Models ===");
foreach (var m in models)
    Console.WriteLine($"  {m.Id}");

// 2. List all providers
var providers = await ai.Providers.List();
Console.WriteLine("\n=== Providers ===");
if (providers.All != null)
{
    foreach (var p in providers.All)
        Console.WriteLine($"  {p.Id}  (connected: {p.Connected})");
}

// 3. List available agents
var agents = await ai.Agents.List();
Console.WriteLine("\n=== Agents ===");
foreach (var a in agents)
    Console.WriteLine($"  {a.Name}  — {a.Description}");

// 4. List slash commands
var commands = await ai.Commands.List();
Console.WriteLine("\n=== Commands ===");
foreach (var c in commands)
    Console.WriteLine($"  /{c.Name}  — {c.Description}");

// 5. Health check
var health = await ai.Diagnostics.GetHealth();
Console.WriteLine($"\nServer health: {health.Healthy}  (v{health.Version})");
