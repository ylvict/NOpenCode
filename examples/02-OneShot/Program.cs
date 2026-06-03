using NOpenCode;

// One-shot query with configuration — choose a model.
var answer = await OpenCode.Ask(
    "What is the capital of France?",
    cfg => cfg.WithModel("opencode/deepseek-v4-flash-free")
);

Console.WriteLine("=== Answer ===");
Console.WriteLine(answer);

// Or use the builder for more control:
var api = await OpenCode
    .Configure()
    .WithModel("opencode/deepseek-v4-flash-free")
    .Launch();

var detailed = await api
    .Ask("What is the population of Paris?")
    .ExecuteFull();

Console.WriteLine($"\n=== Detailed (tokens: {detailed.GetUsage()?.Total}) ===");
Console.WriteLine(detailed.GetText());
