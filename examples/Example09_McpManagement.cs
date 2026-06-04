namespace Examples;

static class Example09_McpManagement
{
    public static async Task Run(string[] args)
    {
        await using var ai = await OpenCode
            .Configure()
            .Launch();

        var servers = await ai.Mcp.List();
        Console.WriteLine("=== MCP Servers ===");
        foreach (var kv in servers)
            Console.WriteLine($"  {kv.Key}: {kv.Value.Status}");

        var status = await ai.Mcp.Add("filesystem", new
        {
            type = "local",
            command = new[] { "npx", "-y", "@modelcontextprotocol/server-filesystem", "./" }
        });
        Console.WriteLine($"\nAdded '{status.Name}': {status.Status}");

        servers = await ai.Mcp.List();
        Console.WriteLine("\n=== Updated MCP Servers ===");
        foreach (var kv in servers)
            Console.WriteLine($"  {kv.Key}: {kv.Value.Status}");
    }
}
