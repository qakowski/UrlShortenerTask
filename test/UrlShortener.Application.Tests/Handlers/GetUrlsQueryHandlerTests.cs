using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using UrlShortener.Application.Dto;
using UrlShortener.Application.Handlers;
using UrlShortener.Application.Helpers;
using UrlShortener.Application.Requests;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Tests.Handlers;

public class GetUrlsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsUrlsDtoList()
    {
        var repositoryMock = new Mock<IShortUrlRepository>();
        repositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<ShortUrl>
        {
            new("123", "http://www.example.com"),
            new("456", "http://www.example.org")
        });

        var loggerMock = new Mock<ILogger<GetUrlsQueryHandler>>();

        var urlBuilderHelperMock = new Mock<IUrlBuilderHelper>();
        urlBuilderHelperMock.Setup(urlBuilder => urlBuilder.BuildUrl(It.IsAny<string>()))
            .Returns((string input) => $"http://localhost:8080/{input}");

        var handler = new GetUrlsQueryHandler(loggerMock.Object, repositoryMock.Object, urlBuilderHelperMock.Object);

        var request = new GetUrlsQuery();

        var response = await handler.Handle(request, CancellationToken.None);

        using (new AssertionScope())
        {
            response.Value.Should().NotBeNull();
            response.Value.Should().HaveCount(2);
            response.Value.Should().BeEquivalentTo(new List<UrlsDto>
            {
                new("http://www.example.com", "http://localhost:8080/123"),
                new("http://www.example.org", "http://localhost:8080/456")
            });
        }
    }
}