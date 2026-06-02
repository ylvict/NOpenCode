using NOpenCode;

// Full session lifecycle — create, fork, share, diff, delete.
await using var ai = await OpenCode
    .Configure()
    .WithModel("opencode/deepseek-v4-flash-free")
    .Launch();

// 1. Create a session
var session = await ai.NewSession("Bug analysis").Create();
Console.WriteLine($"Created session: {session.Id}");

// 2. Ask a question
var r1 = await session.Ask("Explain the concept of nullable reference types in C#.");
Console.WriteLine($"First reply: {r1.GetText()[..100]}...\n");

// 3. Fork the session (creates a branch)
var fork = await session.Fork();
Console.WriteLine($"Forked session: {fork.Id}");

// 4. Continue on the fork with a different angle
var r2 = await fork.Ask("What are the dangers of async void methods?");
Console.WriteLine($"Fork reply: {r2.GetText()[..100]}...\n");

// 5. Get the diff from the fork
var diff = await fork.GetDiff();
foreach (var d in diff)
    Console.WriteLine($"  [{d.Type}] {d.Path}");

// 6. Share the session
await fork.Share();
Console.WriteLine($"\nShared session: {fork.Id}");

// 7. Clean up
await fork.Delete();
await session.Delete();
Console.WriteLine("Sessions deleted.");
