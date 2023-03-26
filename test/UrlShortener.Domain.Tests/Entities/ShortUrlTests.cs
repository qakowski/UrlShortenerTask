using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Tests.Entities;

public class ShortUrlTests
{
    [Fact]
    public void CreateShortUrl_ValidData_CreatesObject()
    {
        const string shortenedUrl = "http://localhost/abc";
        const string url = "http://www.example.com";
        
        var shortUrl = new ShortUrl(shortenedUrl, url);

        shortUrl.ShortenedUrl.Should().Be(shortenedUrl);
        shortUrl.Url.Should().Be(url);
    }
}