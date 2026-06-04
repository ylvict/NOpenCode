namespace NOpenCode.IntegrationTests;

public class AskTests
{
    [Fact]
    public async Task Free_Model_Should_Return_Reply()
    {
        await using var ai = await OpenCode
            .Configure()
            .Launch();

        var freeModels = (await ai.Models.List("opencode"))
            .Where(m => m.Id != null && m.Id.EndsWith("-free", StringComparison.OrdinalIgnoreCase))
            .Select(m => $"{m.Provider}/{m.Id}")
            .ToList();

        if (freeModels.Count == 0)
        {
            Assert.True(true, "No free models currently exposed; skipping.");
            return;
        }

        foreach (var model in freeModels)
        {
            await using var sessionAi = await OpenCode
                .Configure()
                .WithModel(model)
                .Launch();

            var reply = await sessionAi
                .Ask("Say exactly 'ok' and nothing else.")
                .Execute();

            Assert.False(string.IsNullOrWhiteSpace(reply), $"Empty reply for model {model}");
        }
    }
}
