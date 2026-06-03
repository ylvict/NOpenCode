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

// Stop if no events arrive for 5 seconds.
var idleTimer = new Timer(_ => cts.Cancel());

await ai.Events.Subscribe(
    onEvent: evt =>
    {
        idleTimer.Change(5000, Timeout.Infinite);
        Console.WriteLine(
            $"[{evt.Type}] {evt.Data[..Math.Min(evt.Data.Length, 120)]}");
    },
    onError: ex => Console.WriteLine($"Error: {ex.Message}"),
    onEnd: () =>
    {
        Console.WriteLine("Stream ended.");
        cts.Cancel();
    },
    ct: cts.Token
);

idleTimer.Dispose();
Console.WriteLine("Done.");
