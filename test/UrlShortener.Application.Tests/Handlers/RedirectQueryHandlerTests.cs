using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Handlers;
using UrlShortener.Application.Requests;
using UrlShortener.Application.Results;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Tests.Handlers;

public class RedirectQueryHandlerTests
{
    private readonly Mock<ILogger<RedirectQueryHandler>> _loggerMock;
    private readonly Mock<IShortUrlRepository> _shortUrlRepositoryMock;

    public RedirectQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<RedirectQueryHandler>>();
        _shortUrlRepositoryMock = new Mock<IShortUrlRepository>();
    }

    [Fact]
    public async Task Handle_WhenShortUrlDoesNotExist_ReturnsNotFoundResult()
    {
        const string hashed = "abc123";
        
        var request = new RedirectQuery{Hashed = hashed};
        
        _shortUrlRepositoryMock.Setup(repo => repo.GetAsync(hashed)).ReturnsAsync((string)null);

        var handler = new RedirectQueryHandler(_loggerMock.Object, _shortUrlRepositoryMock.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundResult>();
            result.Errors.First().Should().Be($"Short URL with hash '{hashed}' not found.");
        }
    }

    [Fact]
    public async Task Handle_WhenShortUrlExists_ReturnsBaseResultWithOriginalUrl()
    {
        const string hashed = "def456";
        const string originalUrl = "https://example.com";

        var request = new RedirectQuery { Hashed = hashed };

        _shortUrlRepositoryMock.Setup(repo => repo.GetAsync(hashed)).ReturnsAsync(originalUrl);

        var handler = new RedirectQueryHandler(_loggerMock.Object, _shortUrlRepositoryMock.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<BaseResult<string>>();
            result?.Value.Should().Be(originalUrl);
        }
    }
}