namespace Examples;

static class Example02_OneShot
{
    public static async Task Run(string[] args)
    {
        var answer = await OpenCode.Ask(
            "What is the capital of France?"
        );

        Console.WriteLine("=== Answer ===");
        Console.WriteLine(answer);

        var api = await OpenCode
            .Configure()
            .Launch();

        var detailed = await api
            .Ask("What is the population of Paris?")
            .ExecuteFull();

        Console.WriteLine($"\n=== Detailed (tokens: {detailed.GetUsage()?.Total}) ===");
        Console.WriteLine(detailed.GetText());
    }
}
