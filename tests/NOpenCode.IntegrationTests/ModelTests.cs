namespace NOpenCode.IntegrationTests;

public class ModelTests
{
    [Fact]
    public async Task Free_Models_Should_Be_Available()
    {
        await using var ai = await OpenCode
            .Configure()
            .Launch();

        var models = await ai.Models.List("opencode");
        var freeModels = models
            .Where(m => m.Id != null && m.Id.EndsWith("-free", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.NotEmpty(freeModels);
    }
}
