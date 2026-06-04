using NOpenCode;

// One-shot query with configuration — pick any free model at launch.
var answer = await OpenCode.Ask(
    "What is the capital of France?",
    cfg => cfg.WithAnyFreeModel()
);

Console.WriteLine("=== Answer ===");
Console.WriteLine(answer);

// Or use the builder for more control:
var api = await OpenCode
    .Configure()
    .WithAnyFreeModel()
    .Launch();

var detailed = await api
    .Ask("What is the population of Paris?")
    .ExecuteFull();

Console.WriteLine($"\n=== Detailed (tokens: {detailed.GetUsage()?.Total}) ===");
Console.WriteLine(detailed.GetText());
