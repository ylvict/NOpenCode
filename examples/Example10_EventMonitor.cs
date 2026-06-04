namespace Examples;

static class Example10_EventMonitor
{
    public static async Task Run(string[] args)
    {
        await using var ai = await OpenCode
            .Configure()
            .Launch();

        Console.WriteLine("Listening for events...\n");

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, _) =>
        {
            Console.WriteLine("\nShutting down...");
            cts.Cancel();
        };

        var subscribed = ai.Events.Subscribe(
            onEvent: evt => Console.WriteLine(
                $"[{evt.Type}] {evt.Data[..Math.Min(evt.Data.Length, 120)]}"),
            onError: ex => Console.WriteLine($"Error: {ex.Message}"),
            onEnd: () => Console.WriteLine("Stream ended."),
            ct: cts.Token
        );

        var answer = await ai.Ask("What is 2+2?").Execute();
        Console.WriteLine($"\nAnswer: {answer}");

        await Task.Delay(2000);
        cts.Cancel();

        await subscribed;
        Console.WriteLine("Done.");
    }
}
