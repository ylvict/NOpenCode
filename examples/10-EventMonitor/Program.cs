using NOpenCode;

// Real-time event monitoring via SSE (Server-Sent Events).
await using var ai = await OpenCode
    .Configure()
    .Launch();

Console.WriteLine("Listening for events...\n");

// Subscribe to the event stream and stop after 5 events.
var count = 0;
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) =>
{
    Console.WriteLine("\nShutting down...");
    cts.Cancel();
};

await ai.Events.Subscribe(
    onEvent: evt =>
    {
        Console.WriteLine(
            $"[{evt.Type}] {evt.Data[..Math.Min(evt.Data.Length, 120)]}");
        if (++count >= 5) cts.Cancel();
    },
    onError: ex => Console.WriteLine($"Error: {ex.Message}"),
    onEnd: () => Console.WriteLine("Stream ended."),
    ct: cts.Token
);

Console.WriteLine("Done.");
