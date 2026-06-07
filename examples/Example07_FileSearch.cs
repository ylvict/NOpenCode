namespace Examples;

static class Example07_FileSearch
{
    public static async Task Run(string[] args)
    {
        await using var ai = await OpenCode
            .Configure()
            .Launch();

        var todos = await ai.Files.Search("TODO|FIXME|HACK");
        Console.WriteLine($"Found {todos.Count} items:\n");

        todos.ForEach(match =>
            Console.WriteLine($"  {match.Path}:{match.LineNumber}  {match.Lines?.Trim()}"));

        var csFiles = await ai.Files.Find("*.cs");
        Console.WriteLine($"\nC# files: {csFiles.Count}");

        var srcTree = await ai.Files.List("./src");
        var icon = srcTree.Count > 0 && srcTree[0].Type == "directory" ? "[DIR]" : "[FILE]";
        Console.WriteLine($"\n{icon} {srcTree[0].Name}");

        var content = await ai.Files.Read("Program.cs");
        Console.WriteLine($"\n--- Program.cs ({content.Content?.Length} chars) ---");
        Console.WriteLine(content.Content?[..Math.Min(200, content.Content?.Length ?? 0)]);
    }
}
