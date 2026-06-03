namespace NOpenCode.IntegrationTests;

public class HealthTests
{
    [Fact]
    public async Task Server_Should_Be_Healthy()
    {
        await using var ai = await OpenCode
            .Configure()
            .Launch();

        var health = await ai.Diagnostics.GetHealth();

        Assert.True(health.Healthy, "OpenCode server should report healthy");
        Assert.NotNull(health.Version);
        Assert.NotEmpty(health.Version);
    }
}
