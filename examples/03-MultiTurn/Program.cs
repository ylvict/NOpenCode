using NOpenCode;

// Multi-turn conversation — create a session, ask follow-up questions.
await using var ai = await OpenCode
    .Configure()
    .WithAnyFreeModel()
    .Launch();

var session = await ai
    .NewSession("C# discussion")
    .Create();

var r1 = await session.Ask(
    "What are the key differences between REST and GraphQL?"
);
Console.WriteLine($"[1] {r1.GetText()}\n");

var r2 = await session.Ask(
    "When would you choose one over the other?"
);
Console.WriteLine($"[2] {r2.GetText()}\n");

var r3 = await session.Ask(
    "Can you give a real-world example of migrating from REST to GraphQL?"
);
Console.WriteLine($"[3] {r3.GetText()}");

// Clean up
await session.Delete();
