using NOpenCode;

// One-shot query with full configuration — choose model, attach files, etc.
var answer = await OpenCode.Ask(
    "Review this code for potential bugs.",
    cfg => cfg
        .WithModel("opencode/deepseek-v4-flash-free")
        .InDirectory(@"./src")
);

Console.WriteLine("=== Review result ===");
Console.WriteLine(answer);

// Or use the builder for more control:
var api = await OpenCode
    .Configure()
    .WithModel("opencode/deepseek-v4-flash-free")
    .Launch();

var detailed = await api
    .Ask("What improvements would you suggest for error handling?")
    .WithFiles("Program.cs")
    .ExecuteFull();

Console.WriteLine($"\n=== Detailed review (tokens: {detailed.Usage?.Total}) ===");
Console.WriteLine(detailed.GetText());
