using Examples;
using NOpenCode;

var examples = new Dictionary<string, Func<string[], Task>>
{
    ["hello"]        = Example01_HelloWorld.Run,
    ["oneshot"]      = Example02_OneShot.Run,
    ["multiturn"]    = Example03_MultiTurn.Run,
    ["streaming"]    = Example04_Streaming.Run,
    ["di"]           = Example05_DI.Run,
    ["session"]      = Example06_SessionLifecycle.Run,
    ["filesearch"]   = Example07_FileSearch.Run,
    ["discovery"]    = Example08_Discovery.Run,
    ["mcp"]          = Example09_McpManagement.Run,
    ["eventmonitor"] = Example10_EventMonitor.Run,
};

var name = args.Length > 0 ? args[0].ToLowerInvariant() : null;

if (name is null || !examples.ContainsKey(name))
{
    Console.WriteLine("Available examples:");
    foreach (var (key, _) in examples)
        Console.WriteLine($"  {key}");
    Console.WriteLine();
    Console.WriteLine("Usage: dotnet run -- <example-name>");
    return;
}

await examples[name](args);
