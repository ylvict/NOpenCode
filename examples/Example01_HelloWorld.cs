namespace Examples;

static class Example01_HelloWorld
{
    public static async Task Run(string[] args)
    {
        var answer = await OpenCode.Ask(
            "Write a one-sentence explanation of dependency injection."
        );

        Console.WriteLine(answer);
    }
}
