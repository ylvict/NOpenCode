namespace NOpenCode.IntegrationTests;

public class AskTests
{
    [Theory]
    [InlineData("opencode/deepseek-v4-flash-free")]
    [InlineData("opencode/mimo-v2.5-free")]
    [InlineData("opencode/nemotron-3-super-free")]
    public async Task Free_Model_Should_Return_Reply(string model)
    {
        await using var ai = await OpenCode
            .Configure()
            .WithModel(model)
            .Launch();

        var reply = await ai
            .Ask("Say exactly 'ok' and nothing else.")
            .Execute();

        Assert.NotNull(reply);
        Assert.NotEmpty(reply);
    }
}
