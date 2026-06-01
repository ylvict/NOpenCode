using NOpenCode;

// File and code search operations — search, find, read files.
await using var ai = await OpenCode
    .Configure()
    .WithModel("opencode/mimo-v2.5-free")
    .Launch();

// 1. Text search (ripgrep) — find all TODOs
var todos = await ai.Files.Search("TODO|FIXME|HACK");
Console.WriteLine($"Found {todos.Count} items:\n");

foreach (var match in todos)
{
    Console.WriteLine($"  {match.Path}:{match.LineNumber}  {match.Lines?.Trim()}");
}

// 2. Find files by name
var csFiles = await ai.Files.Find("*.cs");
Console.WriteLine($"\nC# files: {csFiles.Count}");

// 3. List directory tree
var srcTree = await ai.Files.List("./src");
var icon = srcTree.Count > 0 && srcTree[0].Type == "directory" ? "[DIR]" : "[FILE]";
Console.WriteLine($"\n{icon} {srcTree[0].Name}");

// 4. Read a specific file
var content = await ai.Files.Read("Program.cs");
Console.WriteLine($"\n--- Program.cs ({content.Content?.Length} chars) ---");
Console.WriteLine(content.Content?[..Math.Min(200, content.Content?.Length ?? 0)]);
