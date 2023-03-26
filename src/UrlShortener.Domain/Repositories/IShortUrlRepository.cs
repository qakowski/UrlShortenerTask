using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Repositories;

public interface IShortUrlRepository
{
    /// <summary>
    /// Adds shorten url to currently used database
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public Task<bool> AddAsync(ShortUrl url);
    
    /// <summary>
    /// Retrieves url by shorten url
    /// </summary>
    /// <param name="shortUrl"></param>
    /// <returns></returns>
    public Task<string?> GetAsync(string shortUrl);

    /// <summary>
    /// Retrieve list of urls
    /// </summary>
    /// <param name="page">Page that you want to retrieve</param>
    /// <param name="limit">Number of elements that you want to retrieve</param>
    /// <returns></returns>
    public Task<IEnumerable<ShortUrl>> GetAllAsync(int page, int limit);

    public Task<int> GetNumberOfEntriesAsync();
}