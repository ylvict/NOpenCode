using NOpenCode;

// Streaming — receive the response in chunks as it's generated.
await using var ai = await OpenCode
    .Configure()
    .WithModel("opencode/nemotron-3-super-free")
    .Launch();

Console.WriteLine("Generating documentation...\n");

var session = await ai.NewSession("Docs generation").Create();

await session.AskStream(
    "Write a brief README for a .NET library called NOpenCode.",
    onChunk: chunk => Console.Write(chunk),
    onComplete: reply => Console.WriteLine($"\n\n--- Done (tokens: {reply.Usage?.Total}) ---"),
    onError: ex => Console.WriteLine($"\nError: {ex.Message}")
);

await session.Delete();
