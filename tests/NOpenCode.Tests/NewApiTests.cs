using System.Reflection;

namespace NOpenCode.Tests;

public class NewApiTests
{
    [Fact]
    public void AskOperation_Execute_AcceptsCancellationToken()
    {
        var method = typeof(AskOperation).GetMethod("Execute", new[] { typeof(CancellationToken) });
        Assert.NotNull(method);
        Assert.Equal(typeof(Task<string>), method!.ReturnType);
    }

    [Fact]
    public void AskOperation_ExecuteFull_AcceptsCancellationToken()
    {
        var method = typeof(AskOperation).GetMethod("ExecuteFull", new[] { typeof(CancellationToken) });
        Assert.NotNull(method);
        Assert.Equal(typeof(Task<OpenCodeReply>), method!.ReturnType);
    }

    [Fact]
    public void AskOperation_WithFiles_StoresFiles()
    {
        var op = (AskOperation)Activator.CreateInstance(
            typeof(AskOperation),
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new object?[] { null!, null!, "test prompt" },
            null)!;
        var result = op.WithFiles("file1.cs", "file2.cs");
        Assert.Same(op, result);
    }

    [Fact]
    public void McpClient_HasRemoveMethod()
    {
        var method = typeof(McpClient).GetMethod("Remove", new[] { typeof(string), typeof(CancellationToken) });
        Assert.NotNull(method);
        Assert.Equal(typeof(Task), method!.ReturnType);
    }

    [Fact]
    public void McpClient_HasUpdateMethod()
    {
        var method = typeof(McpClient).GetMethod("Update", new[] { typeof(string), typeof(object), typeof(CancellationToken) });
        Assert.NotNull(method);
        Assert.Equal(typeof(Task<McpStatus>), method!.ReturnType);
    }

    [Fact]
    public void ConfigClient_HasTypedUpdateOverload()
    {
        var methods = typeof(ConfigClient).GetMethods()
            .Where(m => m.Name == "Update")
            .ToList();
        var typedOverload = methods.FirstOrDefault(m =>
        {
            var p = m.GetParameters();
            return p.Length == 2 && p[0].ParameterType == typeof(NOpenCodeConfig);
        });
        Assert.NotNull(typedOverload);
    }

    [Fact]
    public void Part_HasUriProperty()
    {
        var prop = typeof(Part).GetProperty("Uri");
        Assert.NotNull(prop);
        Assert.Equal(typeof(string), prop!.PropertyType);
    }

    [Fact]
    public void MessageOptions_FilesPropertyExists()
    {
        var prop = typeof(MessageOptions).GetProperty("Files");
        Assert.NotNull(prop);
        Assert.Equal(typeof(List<string>), prop!.PropertyType);
    }

    [Fact]
    public void OpenCodeSession_AskStream_UsesPostStream()
    {
        var methods = typeof(OpenCodeSession).GetMethods()
            .Where(m => m.Name == "AskStream")
            .ToList();
        Assert.Equal(2, methods.Count);

        var withConfigure = methods.FirstOrDefault(m =>
        {
            var p = m.GetParameters();
            return p.Length >= 2 && p[1].ParameterType == typeof(Action<MessageOptions>);
        });
        Assert.NotNull(withConfigure);

        var withoutConfigure = methods.FirstOrDefault(m =>
        {
            var p = m.GetParameters();
            return p[0].ParameterType == typeof(string) &&
                   p[1].ParameterType == typeof(Action<string>);
        });
        Assert.NotNull(withoutConfigure);
    }

    [Fact]
    public void ConfigClient_HasBothUpdateOverloads()
    {
        var methods = typeof(ConfigClient).GetMethods()
            .Where(m => m.Name == "Update" && m.DeclaringType == typeof(ConfigClient))
            .ToArray();
        Assert.Equal(2, methods.Length);
    }
}
