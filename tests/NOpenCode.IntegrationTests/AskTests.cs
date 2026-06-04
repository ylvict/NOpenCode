namespace NOpenCode.IntegrationTests;

public class AskTests
{
    [Fact]
    public async Task Free_Model_Should_Return_Reply()
    {
        await using var ai = await OpenCode
            .Configure()
            .WithAnyFreeModel()
            .Launch();

        var reply = await ai
            .Ask("Say exactly 'ok' and nothing else.")
            .Execute();

        Assert.False(string.IsNullOrWhiteSpace(reply));
    }
}
