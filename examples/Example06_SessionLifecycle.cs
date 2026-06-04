namespace Examples;

static class Example06_SessionLifecycle
{
    public static async Task Run(string[] args)
    {
        await using var ai = await OpenCode
            .Configure()
            .Launch();

        var session = await ai.NewSession("Bug analysis").Create();
        Console.WriteLine($"Created session: {session.Id}");

        var r1 = await session.Ask("Explain the concept of nullable reference types in C#.");
        Console.WriteLine($"First reply: {r1.GetText()[..100]}...\n");

        var fork = await session.Fork();
        Console.WriteLine($"Forked session: {fork.Id}");

        var r2 = await fork.Ask("What are the dangers of async void methods?");
        Console.WriteLine($"Fork reply: {r2.GetText()[..100]}...\n");

        var diff = await fork.GetDiff();
        diff.ForEach(d => Console.WriteLine($"  [{d.Type}] {d.Path}"));

        await fork.Share();
        Console.WriteLine($"\nShared session: {fork.Id}");

        await fork.Delete();
        await session.Delete();
        Console.WriteLine("Sessions deleted.");
    }
}
