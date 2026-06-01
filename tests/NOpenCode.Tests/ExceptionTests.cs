namespace NOpenCode.Tests;

public class ExceptionTests
{
    [Fact]
    public void NOpenCodeException_DefaultConstructor()
    {
        var ex = new NOpenCodeException();
        Assert.NotNull(ex.Message);
    }

    [Fact]
    public void NOpenCodeException_MessageOnly()
    {
        var ex = new NOpenCodeException("oops");
        Assert.Equal("oops", ex.Message);
    }

    [Fact]
    public void NOpenCodeException_WithInnerException()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new NOpenCodeException("outer", inner);
        Assert.Equal("outer", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void NOpenCodeServerException_WithStatusCode()
    {
        var ex = new NOpenCodeServerException("fail", 503);
        Assert.Equal("fail", ex.Message);
        Assert.Equal(503, ex.StatusCode);
    }

    [Fact]
    public void NOpenCodeServerException_WithInnerAndStatusCode()
    {
        var inner = new Exception("inner");
        var ex = new NOpenCodeServerException("fail", inner, 500);
        Assert.Same(inner, ex.InnerException);
        Assert.Equal(500, ex.StatusCode);
    }

    [Fact]
    public void NOpenCodeNotInstalledException_DefaultMessage()
    {
        var ex = new NOpenCodeNotInstalledException();
        Assert.Contains("opencode CLI is not installed", ex.Message);
    }

    [Fact]
    public void NOpenCodeNotInstalledException_CustomMessage()
    {
        var ex = new NOpenCodeNotInstalledException("custom msg");
        Assert.Equal("custom msg", ex.Message);
    }

    [Fact]
    public void NOpenCodeRequestException_WithNullResponseBody()
    {
        var ex = new NOpenCodeRequestException("/path", "err", 400);
        Assert.Equal("/path", ex.Endpoint);
        Assert.Equal(400, ex.HttpStatus);
        Assert.Null(ex.ResponseBody);
    }

    [Fact]
    public void NOpenCodeRequestException_WithResponseBody()
    {
        var ex = new NOpenCodeRequestException("/path", "err", 400, "{}");
        Assert.Equal("{}", ex.ResponseBody);
        Assert.Contains("{}", ex.Message);
    }

    [Fact]
    public void NOpenCodeRequestException_WithInnerException()
    {
        var inner = new Exception("inner");
        var ex = new NOpenCodeRequestException("/path", "err", null, null, inner);
        Assert.Same(inner, ex.InnerException);
    }
}
