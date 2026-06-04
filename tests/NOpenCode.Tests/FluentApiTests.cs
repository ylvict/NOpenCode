namespace NOpenCode.Tests;

public class FluentApiTests
{
    [Fact]
    public void OpenCode_StaticConfigure_ReturnsBuilder()
    {
        var builder = OpenCode.Configure();
        Assert.IsType<NOpenCodeBuilder>(builder);
    }

    [Fact]
    public void OpenCode_AskLambda_Compiles()
    {
        Func<Task<string>> ask = () => OpenCode.Ask("hello", cfg =>
        {
#pragma warning disable CS0618 // 'WithModel(string)' is obsolete
            cfg.WithModel("provider/model");
#pragma warning restore CS0618
            cfg.WithAgent("agent");
            cfg.InDirectory("./src");
            cfg.OnPort(5000);
            cfg.WithCredentials("user", "pass");
            cfg.WithLogging(Console.WriteLine);
        });
        Assert.NotNull(ask);
    }

    [Fact]
    public void Builder_WithCredentials_SetsCredentials()
    {
        var builder = new NOpenCodeBuilder()
            .WithCredentials("user", "pass");
        Assert.NotNull(builder);
    }

    [Fact]
    public void Builder_WithLogging_SetsCallback()
    {
        var builder = new NOpenCodeBuilder()
            .WithLogging(msg => { });
        Assert.NotNull(builder);
    }

    [Fact]
    public void Builder_WithAutoServer_ReturnsSelf()
    {
        var builder = new NOpenCodeBuilder().WithAutoServer();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Builder_FluentChain_ReturnsSelf()
    {
        var builder = new NOpenCodeBuilder();
#pragma warning disable CS0618 // 'WithModel(string)' is obsolete
        Assert.Same(builder, builder.WithModel("m"));
#pragma warning restore CS0618
        Assert.Same(builder, builder.WithAgent("a"));
        Assert.Same(builder, builder.InDirectory("d"));
        Assert.Same(builder, builder.OnPort(1));
        Assert.Same(builder, builder.WithCredentials("u", "p"));
        Assert.Same(builder, builder.WithLogging(_ => { }));
        Assert.Same(builder, builder.WithAutoServer());
    }
}
