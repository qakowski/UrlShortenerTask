using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using UrlShortener.Application.Handlers;
using UrlShortener.Application.Helpers;
using UrlShortener.Application.Requests;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.Services;

namespace UrlShortener.Application.Tests.Handlers;

public class CreateShortUrlCommandHandlerTests
{
    private readonly Mock<ILogger<CreateShortUrlCommandHandler>> _loggerMock;
    private readonly Mock<IShortUrlRepository> _shortUrlRepositoryMock;
    private readonly Mock<IUrlShortenerService> _urlShortenerServiceMock;
    private readonly Mock<IUrlBuilderHelper> _urlBuilderHelperMock;

    private readonly CreateShortUrlCommandHandler _commandHandler;

    public CreateShortUrlCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CreateShortUrlCommandHandler>>();
        _shortUrlRepositoryMock = new Mock<IShortUrlRepository>();
        _urlShortenerServiceMock = new Mock<IUrlShortenerService>();
        _urlBuilderHelperMock = new Mock<IUrlBuilderHelper>();

        _commandHandler = new CreateShortUrlCommandHandler(
            _loggerMock.Object,
            _shortUrlRepositoryMock.Object,
            _urlShortenerServiceMock.Object,
            _urlBuilderHelperMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenRepositoryAddSucceeds_ReturnsSuccessResult()
    {
        const string url = "http://example.com";
        const string hashed = "abcd1234";
        const string expectedShortUrl = $"http://localhost/{hashed}";

        var command = new CreateShortUrlCommand { Url = url};

        _shortUrlRepositoryMock.Setup(repo => repo.GetNumberOfEntriesAsync())
            .ReturnsAsync(0);

        _shortUrlRepositoryMock.Setup(repo => repo.AddAsync(It.Is<ShortUrl>(su => su.ShortenedUrl == hashed && su.Url == url)))
            .ReturnsAsync(true);
        
        _urlShortenerServiceMock.Setup(service => service.GenerateShortUrl(url, 0))
            .Returns(hashed);

        _urlBuilderHelperMock.Setup(helper => helper.BuildUrl(hashed))
            .Returns(expectedShortUrl);
        
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Errors.Should().BeEmpty();
            result.Value.Should().NotBeNull();
            result.Value!.Url.Should().Be(expectedShortUrl);
            
            _shortUrlRepositoryMock.Verify(repo => repo.GetNumberOfEntriesAsync(), Times.Once);
            
            _shortUrlRepositoryMock.Verify(
                repo => repo.AddAsync(It.Is<ShortUrl>(su => su.ShortenedUrl == hashed && su.Url == url)),
                Times.Once);
            
            _urlShortenerServiceMock.Verify(service => service.GenerateShortUrl(url, 0), Times.Once);
            
            _urlBuilderHelperMock.Verify(helper => helper.BuildUrl(hashed), Times.Once);
        }
    }

    [Fact]
    public async Task Handle_WhenAddingUrlToRepositoryFails_ReturnsErrorResult()
    {
        const string url = "https://www.example.com";
        const string hashed = "ABCD";
        const int currentEntries = 0;
        
        var shortUrl = new ShortUrl(hashed, url);
        var command = new CreateShortUrlCommand{Url = url};
        
        _shortUrlRepositoryMock.Setup(repo => repo.GetNumberOfEntriesAsync())
            .ReturnsAsync(currentEntries);

        _urlShortenerServiceMock.Setup(service => service.GenerateShortUrl(url, currentEntries))
            .Returns(hashed);
        
        _shortUrlRepositoryMock.Setup(repo => repo.AddAsync(shortUrl))
            .ReturnsAsync(false);

        var result = await _commandHandler.Handle(command, CancellationToken.None);
        using (new AssertionScope())
        {
            result.Errors.Should().NotBeNullOrEmpty();
            result.Errors.Should().Contain($"Problem occured while trying to create shorten url for: {url}");
            result.Value.Should().BeNull();
            
            _shortUrlRepositoryMock.Verify(repo => repo.GetNumberOfEntriesAsync(), Times.Once);

            _shortUrlRepositoryMock.Verify(
                repo => repo.AddAsync(It.Is<ShortUrl>(su => su.ShortenedUrl == hashed && su.Url == url)),
                Times.Once);
            
            _urlShortenerServiceMock.Verify(service => service.GenerateShortUrl(url, currentEntries), Times.Once);

            _urlBuilderHelperMock.Verify(helper => helper.BuildUrl(hashed), Times.Never);
        }
    }
}