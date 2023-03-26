using System;
using UrlShortener.Domain.Services;

namespace UrlShortener.Domain.Tests.Services;

public class UrlShortenerServiceTests
{
    private readonly UrlShortenerService _service;

    public UrlShortenerServiceTests()
    {
        _service = new UrlShortenerService();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GenerateShortUrl_WhenUrlIsNull_ThrowsArgumentNullException(string url)
    {
        Action action = () => _service.GenerateShortUrl(url, 0);
        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'url')");
    }

    [Theory]
    [InlineData("https://example.com", 0, 6)]
    [InlineData("https://example.com", 10000, 7)]
    [InlineData("https://example.com/some/very/long/url/path/with/multiple/segments/and/also/some/query/parameters?param1=value1&param2=value2&param3=value3#fragment", 0, 6)]
    [InlineData("https://example.com/test-url-with-!@#$%^&*()_+=[]{}|;:'\"<>,.?/special-characters", 0, 6)]
    public void GenerateShortUrl_WhenValidUrlIsProvided_ReturnsShortenedUrl(string url, int numberOfEntries, int expectedLength)
    {
        var result = _service.GenerateShortUrl(url, numberOfEntries);
        result.Should().NotBeNullOrEmpty();
        result.Length.Should().Be(expectedLength);
    }
}