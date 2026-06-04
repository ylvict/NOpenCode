namespace Examples;

static class Example08_Discovery
{
    public static async Task Run(string[] args)
    {
        await using var ai = await OpenCode
            .Configure()
            .Launch();

        var models = await ai.Models.List(Providers.OpenCode);
        Console.WriteLine("=== OpenCode Models ===");
        models.ForEach(m => Console.WriteLine($"  {m.Id}"));

        var providers = await ai.Providers.List();
        Console.WriteLine("\n=== Providers ===");
        providers.All?.ForEach(p =>
            Console.WriteLine($"  {p.Id}  (connected: {p.Connected})"));

        var agents = await ai.Agents.List();
        Console.WriteLine("\n=== Agents ===");
        agents.ForEach(a => Console.WriteLine($"  {a.Name}  — {a.Description}"));

        var commands = await ai.Commands.List();
        Console.WriteLine("\n=== Commands ===");
        commands.ForEach(c => Console.WriteLine($"  /{c.Name}  — {c.Description}"));

        var health = await ai.Diagnostics.GetHealth();
        Console.WriteLine($"\nServer health: {health.Healthy}  (v{health.Version})");
    }
}
