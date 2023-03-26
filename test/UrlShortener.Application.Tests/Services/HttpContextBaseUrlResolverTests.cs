using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Services;

namespace UrlShortener.Application.Tests.Services;

public class HttpContextBaseUrlResolverTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly HttpContextBaseUrlResolver _resolver;

    public HttpContextBaseUrlResolverTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var loggerMock = new Mock<ILogger<HttpContextBaseUrlResolver>>();
        _resolver = new HttpContextBaseUrlResolver(_httpContextAccessorMock.Object, loggerMock.Object);
    }

    [Fact]
    public void GetBaseUrl_WhenHttpContextIsValid_ReturnsBaseUrl()
    {
        var context = new DefaultHttpContext
        {
            Request =
            {
                Scheme = "https",
                Host = new HostString("localhost", 8080)
            }
        };
        _httpContextAccessorMock.Setup(m => m.HttpContext).Returns(context);

        var baseUrl = _resolver.GetBaseUrl();

        baseUrl.Should().Be("https://localhost:8080");
    }

    [Fact]
    public void GetBaseUrl_WhenHttpContextIsNull_ThrowsInvalidOperationException()
    {
        _httpContextAccessorMock.Setup(m => m.HttpContext).Returns((HttpContext)null);

        Action action = () => _resolver.GetBaseUrl();

        action.Should().Throw<InvalidOperationException>().WithMessage("Either HttpContext or Request is null");
    }
}