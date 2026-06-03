namespace NOpenCode.IntegrationTests;

public class ModelTests
{
    private static readonly string[] ExpectedFreeModels =
    [
        "opencode/deepseek-v4-flash-free",
        "opencode/mimo-v2.5-free",
        "opencode/minimax-m3-free",
        "opencode/nemotron-3-super-free",
    ];

    [Fact]
    public async Task Free_Models_Should_Be_Available()
    {
        await using var ai = await OpenCode
            .Configure()
            .Launch();

        var models = await ai.Models.List("opencode");
        var modelIds = models.Select(m => $"{m.Provider}/{m.Id}").ToHashSet();

        var missing = ExpectedFreeModels.Where(m => !modelIds.Contains(m)).ToList();

        Assert.True(missing.Count == 0, $"Missing free models: {string.Join(", ", missing)}");
    }
}
