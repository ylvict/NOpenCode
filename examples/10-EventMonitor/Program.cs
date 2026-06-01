using NOpenCode;

// Real-time event monitoring via SSE (Server-Sent Events).
await using var ai = await OpenCode
    .Configure()
    .Launch();

Console.WriteLine("Listening for events (press Ctrl+C to stop)...\n");

// Subscribe to the event stream and print each event.
// In practice you'd filter for specific event types.
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) =>
{
    Console.WriteLine("\nShutting down...");
    cts.Cancel();
};

await ai.Events.Subscribe(
    onEvent: evt => Console.WriteLine(
        $"[{evt.Type}] {evt.Data[..Math.Min(evt.Data.Length, 120)]}"),
    onError: ex => Console.WriteLine($"Error: {ex.Message}"),
    onEnd: () => Console.WriteLine("Stream ended."),
    ct: cts.Token
);
