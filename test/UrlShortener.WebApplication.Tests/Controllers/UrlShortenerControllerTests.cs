using System.Net;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Dto;
using UrlShortener.Application.Requests;
using UrlShortener.Application.Results;
using UrlShortener.WebApplication.Controllers;
using NotFoundResult = UrlShortener.Application.Results.NotFoundResult;

namespace UrlShortener.WebApplication.Tests.Controllers;

public class UrlShortenerControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;

    public UrlShortenerControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
    }

    [Fact]
    public async Task CreateShortUrl_WhenShortUrlCreated_ReturnsCreatedResult()
    {
        var expectedValue = new BaseResult<ShortUrlDto>(new ShortUrlDto("https://localhost/1234"));

        var command = new CreateShortUrlCommand { Url = "www.example.com/" };

        var urlMock = new Mock<IUrlHelper>();

        urlMock.Setup(helper => helper.Link(It.IsAny<string>(), It.IsAny<string>())).Returns("https://localhost/1234");
        
        _mediatorMock.Setup(mediator => mediator.Send(It.IsAny<CreateShortUrlCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedValue);
        
        var controller = new UrlShortenerController(_mediatorMock.Object)
        {
            Url = urlMock.Object
        };

        var result = await controller.CreateShortUrl(command, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<CreatedResult>();
            result.As<ObjectResult>().StatusCode.Should().Be((int)HttpStatusCode.Created);
            result.As<CreatedResult>().Value.Should().Be(expectedValue);
        }
    }

    [Fact]
    public async Task GetUrls_ReturnsResults()
    {
        var expectedResult = new[]
        {
            new UrlsDto("http://www.example.com/123", "http://localhost/123"),
            new UrlsDto("http://www.example.com/123/4567/890/123/456/789", "http://localhost/321"),
        };
        
        var query = new GetUrlsQuery();

        _mediatorMock.Setup(mediator => mediator.Send(It.IsAny<GetUrlsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BaseResult<IEnumerable<UrlsDto>>(new[]
            {
                new UrlsDto("http://www.example.com/123", "http://localhost/123"),
                new UrlsDto("http://www.example.com/123/4567/890/123/456/789", "http://localhost/321"),
            }));

        var controller = new UrlShortenerController(_mediatorMock.Object);

        var result = await controller.GetUrls(query, CancellationToken.None);
        
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            result.As<ObjectResult>().StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.As<OkObjectResult>().Value.Should().BeEquivalentTo(expectedResult);
        }
    }

    [Fact]
    public async Task RedirectToUrl_WhenUrlDoesNotExists_ReturnsNotFound()
    {
        const string hashed = "not existing";
        
        var query = new RedirectQuery { Hashed = hashed };
        
        var expectedResult = new NotFoundResult(new[] { $"Short URL with hash '{hashed}' not found." });
        
        _mediatorMock.Setup(mediator => mediator.Send(It.IsAny<RedirectQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var controller = new UrlShortenerController(_mediatorMock.Object);

        var result = await controller.RedirectToUrl(query, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            result.As<ObjectResult>().StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.As<NotFoundObjectResult>().Value.Should().BeEquivalentTo(expectedResult);
        }
    }
    
    [Fact]
    public async Task RedirectToUrl_WhenUrlExists_ReturnsRedirect()
    {
        const string hashed = "existing";
        const string redirectUrl = "https://www.example.com";
        var query = new RedirectQuery { Hashed = hashed };
        
        _mediatorMock.Setup(mediator => mediator.Send(It.IsAny<RedirectQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BaseResult<string>(redirectUrl));
        
        var controller = new UrlShortenerController(_mediatorMock.Object);

        var result = await controller.RedirectToUrl(query, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().BeOfType<RedirectResult>();
            result.As<RedirectResult>().Url.Should().Be(redirectUrl);
        }
    }
}