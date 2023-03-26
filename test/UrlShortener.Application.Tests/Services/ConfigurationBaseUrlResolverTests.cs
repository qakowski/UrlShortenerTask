using UrlShortener.Application.Services;

namespace UrlShortener.Application.Tests.Services;

public class ConfigurationBaseUrlResolverTests
{
    [Fact]
    public void GetBaseUrl_ReturnsBaseUrl()
    {
        const string expectedValue = "base-url";
        
        var configurationBaseUrlResolver = new ConfigurationBaseUrlResolver("base-url");

        var result = configurationBaseUrlResolver.GetBaseUrl();

        result.Should().Be(expectedValue);
    }
}