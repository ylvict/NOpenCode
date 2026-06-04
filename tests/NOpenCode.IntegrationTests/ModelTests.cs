namespace NOpenCode.IntegrationTests;

public class ModelTests
{
    [Fact]
    public async Task Free_Models_Should_Be_Available()
    {
        await using var ai = await OpenCode
            .Configure()
            .WithModel("opencode/deepseek-v4-flash-free")
            .Launch();

        var models = await ai.Models.List(Providers.OpenCode);
        var freeModels = models
            .Where(NOpenCodeBuilder.AnyFreeSelector)
            .ToList();

        Assert.NotEmpty(freeModels);
    }
}
