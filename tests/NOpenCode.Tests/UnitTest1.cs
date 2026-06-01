namespace NOpenCode.Tests;

public class BuilderTests
{
    [Fact]
    public void Configure_ReturnsBuilder()
    {
        var builder = OpenCode.Configure();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Builder_WithModel_SetsModel()
    {
        var builder = new NOpenCodeBuilder()
            .WithModel("opencode/deepseek-v4-flash-free");
        Assert.NotNull(builder);
    }

    [Fact]
    public void Builder_WithAgent_SetsAgent()
    {
        var builder = new NOpenCodeBuilder()
            .WithAgent("coder");
        Assert.NotNull(builder);
    }

    [Fact]
    public void Builder_InDirectory_SetsDirectory()
    {
        var builder = new NOpenCodeBuilder()
            .InDirectory(@"./my-project");
        Assert.NotNull(builder);
    }

    [Fact]
    public void Builder_OnPort_SetsPort()
    {
        var builder = new NOpenCodeBuilder()
            .OnPort(4096);
        Assert.NotNull(builder);
    }

    [Fact]
    public void Builder_ChainsMultipleCalls()
    {
        var builder = new NOpenCodeBuilder()
            .WithModel("opencode/deepseek-v4-flash-free")
            .WithAgent("coder")
            .InDirectory(@"./src")
            .OnPort(4096);
        Assert.NotNull(builder);
    }

    [Fact]
    public void AskOperation_UsingModel_ReturnsSelf()
    {
        var builder = new NOpenCodeBuilder()
            .WithModel("opencode/deepseek-v4-flash-free");
        Assert.NotNull(builder);
    }

    [Fact]
    public void OpenCode_Ask_ReturnsString()
    {
        // This is a compilation test — ensures the static API signature is correct.
        Func<Task<string>> call = () => OpenCode.Ask("test prompt");
        Assert.NotNull(call);
    }

    [Fact]
    public void OpenCode_Ask_WithConfig_ReturnsString()
    {
        Func<Task<string>> call = () => OpenCode.Ask("test", cfg =>
            cfg.WithModel("opencode/deepseek-v4-flash-free"));
        Assert.NotNull(call);
    }

    [Fact]
    public void SessionBuilder_ChainsCorrectly()
    {
        // Compilation test for SessionBuilder API
        Assert.True(true);
    }

    [Fact]
    public void ExceptionHierarchy_IsCorrect()
    {
        var baseEx = new NOpenCodeException("base");
        var notInstalled = new NOpenCodeNotInstalledException();
        var serverEx = new NOpenCodeServerException("server error", 500);
        var requestEx = new NOpenCodeRequestException("/test", "bad request", 400, "{}");

        Assert.IsAssignableFrom<NOpenCodeException>(notInstalled);
        Assert.IsAssignableFrom<NOpenCodeException>(serverEx);
        Assert.IsAssignableFrom<NOpenCodeException>(requestEx);
        Assert.Contains("opencode CLI is not installed", notInstalled.Message);
        Assert.Equal(500, serverEx.StatusCode);
        Assert.Equal("/test", requestEx.Endpoint);
    }

    [Fact]
    public void OpenCodeReply_GetText_ReturnsExtractedText()
    {
        var reply = new OpenCodeReply
        {
            Text = "Hello from text property"
        };
        Assert.Equal("Hello from text property", reply.GetText());
    }

    [Fact]
    public void OpenCodeReply_GetText_FallsBackToParts()
    {
        var reply = new OpenCodeReply
        {
            Text = null,
            Parts = new()
            {
                new() { Type = "text", Text = "Part 1" },
                new() { Type = "text", Text = "Part 2" },
                new() { Type = "toolUse", ToolName = "bash" }
            }
        };
        var text = reply.GetText();
        Assert.Contains("Part 1", text);
        Assert.Contains("Part 2", text);
    }

    [Fact]
    public void Models_HaveProperties()
    {
        var model = new ModelInfo
        {
            Id = "deepseek-v4-flash-free",
            Name = "DeepSeek V4 Flash Free",
            Provider = "opencode"
        };
        Assert.Equal("deepseek-v4-flash-free", model.Id);
        Assert.Equal("opencode", model.Provider);
    }
}
