using NOpenCode;

// MCP server management — list and add MCP servers.
await using var ai = await OpenCode
    .Configure()
    .Launch();

// 1. List current MCP servers
var servers = await ai.Mcp.List();
Console.WriteLine("=== MCP Servers ===");
foreach (var kv in servers)
    Console.WriteLine($"  {kv.Key}: {kv.Value.Status}");

// 2. Add a filesystem MCP server
var status = await ai.Mcp.Add("filesystem", new
{
    type = "local",
    command = new[] { "npx", "-y", "@modelcontextprotocol/server-filesystem", "./" }
});
Console.WriteLine($"\nAdded '{status.Name}': {status.Status}");

// 3. List again to confirm
servers = await ai.Mcp.List();
Console.WriteLine("\n=== Updated MCP Servers ===");
foreach (var kv in servers)
    Console.WriteLine($"  {kv.Key}: {kv.Value.Status}");
