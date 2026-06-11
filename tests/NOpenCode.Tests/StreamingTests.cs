using System.IO;
using System.Text;
using System.Text.Json;

namespace NOpenCode.Tests;

public class SseReaderTests
{
    [Fact]
    public async Task ReadEventAsync_ReturnsNull_WhenStreamEmpty()
    {
        using var stream = new MemoryStream();
        using var reader = new SseReader(stream);
        var evt = await reader.ReadEventAsync();
        Assert.Null(evt);
    }

    [Fact]
    public async Task ReadEventAsync_ReadsChunkEvent()
    {
        var sse = "event: chunk\ndata: {\"type\":\"text\",\"text\":\"Hello\"}\n\n";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(sse));
        using var reader = new SseReader(stream);
        var evt = await reader.ReadEventAsync();
        Assert.NotNull(evt);
        Assert.Equal("chunk", evt!.Type);
        Assert.Contains("Hello", evt.Data);
    }

    [Fact]
    public async Task ReadEventAsync_ReadsCompleteEvent()
    {
        var reply = new OpenCodeReply
        {
            Parts = new() { new() { Type = "text", Text = "done" } },
            MessageId = "msg_1"
        };
        var json = JsonSerializer.Serialize(reply);
        var sse = $"event: complete\ndata: {json}\n\n";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(sse));
        using var reader = new SseReader(stream);
        var evt = await reader.ReadEventAsync();
        Assert.NotNull(evt);
        Assert.Equal("complete", evt!.Type);
        var deserialized = JsonSerializer.Deserialize<OpenCodeReply>(evt.Data);
        Assert.NotNull(deserialized);
        Assert.Equal("done", deserialized!.GetText());
    }

    [Fact]
    public async Task ReadEventAsync_ReadsErrorEvent()
    {
        var sse = "event: error\ndata: something went wrong\n\n";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(sse));
        using var reader = new SseReader(stream);
        var evt = await reader.ReadEventAsync();
        Assert.NotNull(evt);
        Assert.Equal("error", evt!.Type);
        Assert.Equal("something went wrong", evt.Data);
    }

    [Fact]
    public async Task ReadEventAsync_SkipsCommentLines()
    {
        var sse = ":comment\nevent: chunk\ndata: {\"type\":\"text\",\"text\":\"skip\"}\n\n";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(sse));
        using var reader = new SseReader(stream);
        var evt = await reader.ReadEventAsync();
        Assert.NotNull(evt);
        Assert.Equal("chunk", evt!.Type);
    }

    [Fact]
    public async Task ReadEventAsync_DefaultsToMessageType()
    {
        var sse = "data: {\"type\":\"text\",\"text\":\"hi\"}\n\n";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(sse));
        using var reader = new SseReader(stream);
        var evt = await reader.ReadEventAsync();
        Assert.NotNull(evt);
        Assert.Equal("message", evt!.Type);
    }

    [Fact]
    public async Task ReadEventAsync_HandlesMultipleEvents()
    {
        var sse = "event: chunk\ndata: {\"type\":\"text\",\"text\":\"first\"}\n\n" +
                  "event: chunk\ndata: {\"type\":\"text\",\"text\":\"second\"}\n\n" +
                  "event: complete\ndata: {}\n\n";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(sse));
        using var reader = new SseReader(stream);
        var e1 = await reader.ReadEventAsync();
        Assert.Equal("chunk", e1!.Type);
        Assert.Contains("first", e1.Data);
        var e2 = await reader.ReadEventAsync();
        Assert.Equal("chunk", e2!.Type);
        Assert.Contains("second", e2.Data);
        var e3 = await reader.ReadEventAsync();
        Assert.Equal("complete", e3!.Type);
        Assert.Equal("{}", e3.Data);
        var e4 = await reader.ReadEventAsync();
        Assert.Null(e4);
    }
}
