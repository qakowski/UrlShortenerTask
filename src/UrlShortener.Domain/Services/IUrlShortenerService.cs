namespace UrlShortener.Domain.Services;

public interface IUrlShortenerService
{
    string GenerateShortUrl(string url, int numberOfEntries);
}