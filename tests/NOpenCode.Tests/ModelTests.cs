using System.Text.Json;

namespace NOpenCode.Tests;

public class OpenCodeReplyTests
{
    [Fact]
    public void GetText_NullParts_ReturnsEmpty()
    {
        var reply = new OpenCodeReply { Parts = null };
        Assert.Equal("", reply.GetText());
    }

    [Fact]
    public void GetText_EmptyParts_ReturnsEmpty()
    {
        var reply = new OpenCodeReply { Parts = new() };
        Assert.Equal("", reply.GetText());
    }

    [Fact]
    public void GetText_ReturnsFromTextParts()
    {
        var reply = new OpenCodeReply
        {
            Parts = new() { new() { Type = "text", Text = "hello" } }
        };
        Assert.Equal("hello", reply.GetText());
    }

    [Fact]
    public void GetText_SkipsNonTextParts()
    {
        var reply = new OpenCodeReply
        {
            Parts = new()
            {
                new() { Type = "toolUse", ToolName = "bash", Text = "should skip" },
                new() { Type = "text", Text = "valid" }
            }
        };
        Assert.Equal("valid", reply.GetText());
    }

    [Fact]
    public void GetText_MultipleTextParts_JoinsWithNewline()
    {
        var reply = new OpenCodeReply
        {
            Parts = new()
            {
                new() { Type = "text", Text = "first" },
                new() { Type = "text", Text = "second" }
            }
        };
        Assert.Equal("first\nsecond", reply.GetText());
    }

    [Fact]
    public void Properties_AreSettable()
    {
        var usage = new TokenUsage { Input = 10, Output = 20, Total = 30 };
        var reply = new OpenCodeReply
        {
            Parts = new() { new() { Type = "text", Text = "hello" } },
            Usage = usage,
            MessageId = "msg_123"
        };
        Assert.Equal("hello", reply.GetText());
        Assert.Equal(usage, reply.Usage);
        Assert.Equal("msg_123", reply.MessageId);
    }
}

public class GetUsageTests
{
    [Fact]
    public void ReturnsUsage_WhenTopLevel()
    {
        var reply = new OpenCodeReply { Usage = new() { Input = 1, Output = 2, Total = 3 } };
        Assert.Equal(1, reply.GetUsage()?.Input);
        Assert.Equal(3, reply.GetUsage()?.Total);
    }

    [Fact]
    public void FallsBackToInfoTokens()
    {
        var reply = new OpenCodeReply
        {
            Info = new() { Tokens = new() { Input = 10, Output = 20 } }
        };
        Assert.Equal(10, reply.GetUsage()?.Input);
        Assert.Equal(30, reply.GetUsage()?.Total);
    }

    [Fact]
    public void ReturnsNull_WhenNoUsage()
    {
        var reply = new OpenCodeReply();
        Assert.Null(reply.GetUsage());
    }
}

public class PartTests
{
    [Fact]
    public void DefaultConstructor_SetsDefaults()
    {
        var part = new Part();
        Assert.Equal("", part.Type);
        Assert.Null(part.Text);
        Assert.Null(part.ToolName);
        Assert.Null(part.ToolArgs);
        Assert.Null(part.Result);
    }

    [Fact]
    public void Properties_AreSettable()
    {
        var part = new Part
        {
            Type = "toolUse",
            Text = "output",
            ToolName = "bash",
            ToolArgs = "echo hi",
            Result = "hi"
        };
        Assert.Equal("toolUse", part.Type);
        Assert.Equal("output", part.Text);
        Assert.Equal("bash", part.ToolName);
        Assert.Equal("echo hi", part.ToolArgs);
        Assert.Equal("hi", part.Result);
    }
}

public class TokenUsageTests
{
    [Fact]
    public void DefaultConstructor_SetsNulls()
    {
        var usage = new TokenUsage();
        Assert.Null(usage.Input);
        Assert.Null(usage.Output);
        Assert.Null(usage.Total);
    }

    [Fact]
    public void Properties_AreSettable()
    {
        var usage = new TokenUsage { Input = 100, Output = 50, Total = 150 };
        Assert.Equal(100, usage.Input);
        Assert.Equal(50, usage.Output);
        Assert.Equal(150, usage.Total);
    }
}

public class MessageOptionsTests
{
    [Fact]
    public void DefaultConstructor_SetsNulls()
    {
        var opts = new MessageOptions();
        Assert.Null(opts.Model);
        Assert.Null(opts.Agent);
        Assert.Null(opts.Files);
    }

    [Fact]
    public void Properties_AreSettable()
    {
        var opts = new MessageOptions
        {
            Model = "test/model",
            Agent = "helper",
            Files = new() { "file1.cs", "file2.cs" }
        };
        Assert.Equal("test/model", opts.Model);
        Assert.Equal("helper", opts.Agent);
        Assert.Equal(2, opts.Files!.Count);
    }
}

public class SessionInfoTests
{
    [Fact]
    public void DefaultConstructor_SetsDefaults()
    {
        var info = new SessionInfo();
        Assert.Equal("", info.Id);
        Assert.Null(info.Title);
        Assert.Null(info.CreatedAt);
        Assert.Null(info.UpdatedAt);
        Assert.Null(info.IsShareable);
    }

    [Fact]
    public void Properties_AreSettable()
    {
        var now = DateTime.UtcNow;
        var info = new SessionInfo
        {
            Id = "ses_123",
            Title = "Test",
            CreatedAt = now,
            UpdatedAt = now,
            IsShareable = true
        };
        Assert.Equal("ses_123", info.Id);
        Assert.Equal("Test", info.Title);
        Assert.Equal(now, info.CreatedAt);
        Assert.Equal(now, info.UpdatedAt);
        Assert.True(info.IsShareable);
    }
}

public class MessageInfoTests
{
    [Fact]
    public void Properties_AreSettable()
    {
        var now = DateTime.UtcNow;
        var msg = new MessageInfo
        {
            Id = "msg_1",
            Role = "user",
            Content = "hello",
            CreatedAt = now
        };
        Assert.Equal("msg_1", msg.Id);
        Assert.Equal("user", msg.Role);
        Assert.Equal("hello", msg.Content);
        Assert.Equal(now, msg.CreatedAt);
    }
}

public class FileDiffTests
{
    [Fact]
    public void Properties_AreSettable()
    {
        var diff = new FileDiff
        {
            Path = "src/main.cs",
            Original = "old code",
            Content = "new code",
            Type = "modified"
        };
        Assert.Equal("src/main.cs", diff.Path);
        Assert.Equal("old code", diff.Original);
        Assert.Equal("new code", diff.Content);
        Assert.Equal("modified", diff.Type);
    }
}

public class FindResultTests
{
    [Fact]
    public void Properties_AreSettable()
    {
        var result = new FindResult
        {
            Path = "file.cs",
            Lines = "line content",
            LineNumber = 42,
            Submatches = new() { new() { Match = "TODO", Start = 0, End = 4 } }
        };
        Assert.Equal("file.cs", result.Path);
        Assert.Equal(42, result.LineNumber);
        Assert.Single(result.Submatches!);
        Assert.Equal("TODO", result.Submatches[0].Match);
        Assert.Equal(0, result.Submatches[0].Start);
        Assert.Equal(4, result.Submatches[0].End);
    }
}

public class FileContentTests
{
    [Fact]
    public void Properties_AreSettable()
    {
        var fc = new FileContent { Path = "readme.md", Content = "# Hello" };
        Assert.Equal("readme.md", fc.Path);
        Assert.Equal("# Hello", fc.Content);
    }
}

public class FileNodeTests
{
    [Fact]
    public void Properties_AreSettable()
    {
        var child = new FileNode { Name = "inner.cs", Type = "file", Path = "dir/inner.cs" };
        var parent = new FileNode
        {
            Name = "dir",
            Type = "directory",
            Path = "dir",
            Children = new() { child }
        };
        Assert.Equal("dir", parent.Name);
        Assert.Equal("directory", parent.Type);
        Assert.Single(parent.Children!);
        Assert.Equal("inner.cs", parent.Children[0].Name);
    }
}

public class HealthInfoTests
{
    [Fact]
    public void Properties_AreSettable()
    {
        var health = new HealthInfo { Healthy = true, Version = "1.0.0" };
        Assert.True(health.Healthy);
        Assert.Equal("1.0.0", health.Version);
    }

    [Fact]
    public void DefaultConstructor_SetsFalse()
    {
        var health = new HealthInfo();
        Assert.False(health.Healthy);
        Assert.Null(health.Version);
    }
}

public class McpStatusTests
{
    [Fact]
    public void Properties_AreSettable()
    {
        var status = new McpStatus { Name = "filesystem", Status = "connected", Error = null };
        Assert.Equal("filesystem", status.Name);
        Assert.Equal("connected", status.Status);
        Assert.Null(status.Error);
    }
}

public class AgentInfoTests
{
    [Fact]
    public void Properties_AreSettable()
    {
        var agent = new AgentInfo { Name = "coder", Description = "Writes code", Mode = "auto" };
        Assert.Equal("coder", agent.Name);
        Assert.Equal("Writes code", agent.Description);
        Assert.Equal("auto", agent.Mode);
    }
}

public class ProviderInfoTests
{
    [Fact]
    public void Properties_AreSettable()
    {
        var provider = new ProviderInfo { Id = Providers.OpenCode, Name = "OpenCode", Connected = true };
        Assert.Equal(Providers.OpenCode, provider.Id);
        Assert.Equal("OpenCode", provider.Name);
        Assert.True(provider.Connected);
    }
}

public class ProviderListResponseTests
{
    [Fact]
    public void Properties_AreSettable()
    {
        var resp = new ProviderListResponse
        {
            All = new() { new() { Id = "p1" }, new() { Id = "p2" } },
            Connected = new() { "p1" }
        };
        Assert.Equal(2, resp.All!.Count);
        Assert.Single(resp.Connected!);
    }
}

public class ModelInfoTests
{
    [Fact]
    public void Properties_AreSettable()
    {
        var model = new ModelInfo { Id = "gpt-4", Name = "GPT-4", Provider = "openai" };
        Assert.Equal("gpt-4", model.Id);
        Assert.Equal("GPT-4", model.Name);
        Assert.Equal("openai", model.Provider);
    }
}

public class NOpenCodeConfigTests
{
    [Fact]
    public void Properties_AreSettable()
    {
        var config = new NOpenCodeConfig
        {
            Model = "provider/test-model",
            Agent = "coder",
            Title = "My Session"
        };
        Assert.Equal("provider/test-model", config.Model);
        Assert.Equal("coder", config.Agent);
        Assert.Equal("My Session", config.Title);
    }

    [Fact]
    public void DeserializeFromJson_PopulatesProperties()
    {
        var json = """{"model":"test/model","agent":"helper","title":"Test"}""";
        var config = JsonSerializer.Deserialize<NOpenCodeConfig>(json);
        Assert.NotNull(config);
        Assert.Equal("test/model", config!.Model);
        Assert.Equal("helper", config.Agent);
        Assert.Equal("Test", config.Title);
    }

    [Fact]
    public void DeserializeFromJson_UnknownFieldsIgnored()
    {
        var json = """{"model":"m","extra":"ignored"}""";
        var config = JsonSerializer.Deserialize<NOpenCodeConfig>(json);
        Assert.NotNull(config);
        Assert.Equal("m", config!.Model);
    }

    [Fact]
    public void DefaultConstructor_SetsNulls()
    {
        var config = new NOpenCodeConfig();
        Assert.Null(config.Model);
        Assert.Null(config.Agent);
        Assert.Null(config.Title);
    }
}
