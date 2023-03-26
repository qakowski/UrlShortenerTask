using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Data.Repositories;

public class InMemoryStorage : IShortUrlRepository
{
    private readonly IDictionary<string, string> _database;

    public InMemoryStorage()
    {
        _database = new Dictionary<string, string>();
    }

    public Task<bool> AddAsync(ShortUrl url)
    {
        _database.TryAdd(url.ShortenedUrl, url.Url);
        return Task.FromResult(true);
    }

    public Task<string?> GetAsync(string shortUrl)
    {
        _database.TryGetValue(shortUrl, out var result);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<ShortUrl>> GetAllAsync(int page, int limit)
    {
        var results = _database.Select(x => new ShortUrl(x.Key, x.Value));
        if (limit == 0 && page == 0)
        {
            return Task.FromResult(results);
        }

        return Task.FromResult(results.Skip(page * limit).Take(limit));
    }

    public Task<int> GetNumberOfEntriesAsync()
    {
        return Task.FromResult(_database.Count);
    }
}