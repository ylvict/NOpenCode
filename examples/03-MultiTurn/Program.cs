using NOpenCode;

// Multi-turn conversation — create a session, ask follow-up questions.
await using var ai = await OpenCode
    .Configure()
    .WithModel("opencode/mimo-v2.5-free")
    .Launch();

var session = await ai
    .NewSession("Architecture discussion")
    .Create();

var r1 = await session.Ask(
    "What's the high-level architecture of this project?"
);
Console.WriteLine($"[1] {r1.GetText()}\n");

var r2 = await session.Ask(
    "What are the main entry points and how do they connect?"
);
Console.WriteLine($"[2] {r2.GetText()}\n");

var r3 = await session.Ask(
    "Are there any architectural smells or improvement opportunities?"
);
Console.WriteLine($"[3] {r3.GetText()}");

// Clean up
await session.Delete();
