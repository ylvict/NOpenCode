using NOpenCode;

// One-shot query — no model specified, so the SDK auto-picks a free model
// from the opencode provider at launch time.
var answer = await OpenCode.Ask(
    "What is the capital of France?"
);

Console.WriteLine("=== Answer ===");
Console.WriteLine(answer);

// Or use the builder for more control:
var api = await OpenCode
    .Configure()
    .Launch();

var detailed = await api
    .Ask("What is the population of Paris?")
    .ExecuteFull();

Console.WriteLine($"\n=== Detailed (tokens: {detailed.GetUsage()?.Total}) ===");
Console.WriteLine(detailed.GetText());
