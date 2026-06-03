using NOpenCode;

// Real-time event monitoring via SSE (Server-Sent Events).
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

// Subscribe first, then make a query to generate events.
var subscribed = ai.Events.Subscribe(
    onEvent: evt => Console.WriteLine(
        $"[{evt.Type}] {evt.Data[..Math.Min(evt.Data.Length, 120)]}"),
    onError: ex => Console.WriteLine($"Error: {ex.Message}"),
    onEnd: () => Console.WriteLine("Stream ended."),
    ct: cts.Token
);

var answer = await ai.Ask("What is 2+2?").Execute();
Console.WriteLine($"\nAnswer: {answer}");

// Give a moment for any remaining events, then stop.
await Task.Delay(2000);
cts.Cancel();

await subscribed;
Console.WriteLine("Done.");
