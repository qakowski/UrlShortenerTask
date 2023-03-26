using UrlShortener.Application.Helpers;
using UrlShortener.Application.Services;

namespace UrlShortener.Application.Tests.Helpers;

public class UrlBuilderHelperTests
{
    [Fact]
    public void BuildUrl_WhenHashedValueIsPassed_ReturnsUrl()
    {
        const string hash = "123";
        const string baseUrl = "localhost";
        
        var mockBaseUrlResolver = new Mock<IBaseUrlResolver>();

        mockBaseUrlResolver.Setup(x => x.GetBaseUrl()).Returns("localhost");

        var urlBuilderHelper = new UrlBuilderHelper(mockBaseUrlResolver.Object);

        var result = urlBuilderHelper.BuildUrl(hash);

        result.Should().Be($"{baseUrl}/{hash}");
    }
}