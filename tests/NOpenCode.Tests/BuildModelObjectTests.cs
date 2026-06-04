using System.Text.Json;
using System.Text.Json.Nodes;

namespace NOpenCode.Tests;

public class BuildModelObjectTests
{
    [Fact]
    public void ParseFullProviderModel_SplitsCorrectly()
    {
        var obj = OpenCodeSession.BuildModelObject("provider/test-model-v1");
        Assert.Equal("provider", obj["providerID"]?.ToString());
        Assert.Equal("test-model-v1", obj["modelID"]?.ToString());
    }

    [Fact]
    public void ParseModelOnly_UsesFullString()
    {
        var obj = OpenCodeSession.BuildModelObject("minimax-m3");
        Assert.Null(obj["providerID"]);
        Assert.Equal("minimax-m3", obj["modelID"]?.ToString());
    }

    [Fact]
    public void ParseLeadingSlash_UsesFullString()
    {
        var obj = OpenCodeSession.BuildModelObject("/onlymodel");
        Assert.Null(obj["providerID"]);
        Assert.Equal("/onlymodel", obj["modelID"]?.ToString());
    }

    [Fact]
    public void ParseTrailingSlash_UsesFullString()
    {
        var obj = OpenCodeSession.BuildModelObject("provider/");
        Assert.Null(obj["providerID"]);
        Assert.Equal("provider/", obj["modelID"]?.ToString());
    }

    [Fact]
    public void ParseMultipleSlashes_SplitsOnFirst()
    {
        var obj = OpenCodeSession.BuildModelObject("a/b/c");
        Assert.Equal("a", obj["providerID"]?.ToString());
        Assert.Equal("b/c", obj["modelID"]?.ToString());
    }

    [Fact]
    public void ParseEmptyString_EmptyModelId()
    {
        var obj = OpenCodeSession.BuildModelObject("");
        Assert.Null(obj["providerID"]);
        Assert.Equal("", obj["modelID"]?.ToString());
    }

    [Fact]
    public void SerializesToJson_CorrectKeys()
    {
        var obj = OpenCodeSession.BuildModelObject("provider/test-model-v1");
        var json = JsonSerializer.Serialize(obj);
        using var doc = JsonDocument.Parse(json);
        Assert.Equal("provider", doc.RootElement.GetProperty("providerID").GetString());
        Assert.Equal("test-model-v1", doc.RootElement.GetProperty("modelID").GetString());
    }
}
