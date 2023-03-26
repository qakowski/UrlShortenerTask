using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using UrlShortener.Data.Redis;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Data.Repositories;

public class RedisShortUrlRepository : IShortUrlRepository
{
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly IDatabase _database;
    private readonly ILogger<RedisShortUrlRepository> _logger;
    private const string CounterKey = "url_count";
    private const string SortedSetName = "shortUrls";
    private readonly TimeSpan _timeToLive;

    public RedisShortUrlRepository(IConnectionMultiplexer connectionMultiplexer, 
        RedisConfiguration configuration, 
        IStatisticsRepository statisticsRepository, 
        ILogger<RedisShortUrlRepository> logger)
    {
        _statisticsRepository = statisticsRepository;
        _timeToLive = TimeSpan.FromMinutes(configuration.TimeToLive);
        _database = connectionMultiplexer.GetDatabase();
        _logger = logger;
        
        var subscriber = connectionMultiplexer.GetSubscriber();

        subscriber.Subscribe(RedisChannels.KeyExpiredChannel, (channel, key) =>
        {
            logger.LogInformation("Key expired: {key}", key.ToString());
            RemoveExpiredKey(key.ToString());
        });
    }
    
    public async Task<bool> AddAsync(ShortUrl url)
    {
        var shortenedUrl = url.ShortenedUrl;
        
        var addKeyValueTask =  _database.StringSetAsync(shortenedUrl, url.Url, _timeToLive);

        var addToSortedSetTask = _database.SortedSetAddAsync(SortedSetName, shortenedUrl, await _statisticsRepository.GetScoreForKey(SortedSetName, shortenedUrl));

        await Task.WhenAll(addKeyValueTask, addToSortedSetTask);

        var addKeyValueResult = addKeyValueTask.Result;

        if (addKeyValueResult)
            await _statisticsRepository.IncrementCounterAsync(CounterKey);

        return addKeyValueResult;
    }
    
    public async Task<string?> GetAsync(string shortUrl)
    {
        var result = await _database.StringGetAsync(shortUrl);
        
        if (result.HasValue) 
            await Task.WhenAll(_statisticsRepository.IncrementAccessCountAsync(SortedSetName, shortUrl), Refresh(shortUrl));

        return result;
    }

    public async Task<IEnumerable<ShortUrl>> GetAllAsync(int page, int limit)
    {
        if (page < 0) page = 0;
        if (limit < 0) limit = 0;

        if (page == 0 && limit == 0)
            return await RetrieveAllAsync();

        return await RetrieveWithPaginationAsync(page, limit);
    }

    private async Task<IEnumerable<ShortUrl>> RetrieveAllAsync()
    {
        var entries = await _database.SortedSetRangeByRankWithScoresAsync(SortedSetName, order: Order.Descending);
        var results = new List<ShortUrl>();
        foreach (var entry in entries)
        {
            var key = entry.Element.ToString();
            var value = await _database.StringGetAsync(key);
            results.Add(new ShortUrl(key, value));
        }

        return results;
    }

    private async Task<IEnumerable<ShortUrl>> RetrieveWithPaginationAsync(int page, int limit)
    {
        var start = (page - 1) * limit;
        var end = start + limit - 1;

        var sortedSetEntries = await _database.SortedSetRangeByRankWithScoresAsync(SortedSetName, start, end, Order.Descending);

        var results = new List<ShortUrl>();
        foreach (var entry in sortedSetEntries)
        {
            var key = entry.Element.ToString();
            var value = await _database.StringGetAsync(key);
            results.Add(new ShortUrl(key, value));
        }

        return results;
    }
    
    public async Task<int> GetNumberOfEntriesAsync() 
        => await _statisticsRepository.GetNumberOfEntriesAsync(CounterKey);

    private async Task Refresh(string url) 
        => await _database.KeyExpireAsync(url, _timeToLive);
    
    private void RemoveExpiredKey(string key) 
        => _database.SortedSetRemoveAsync(SortedSetName, key).ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                _logger.LogError(t.Exception, "Could not remove key: {key} from sorted set", key);
            }
            
            _logger.LogInformation("Finished removal of key: {key}", key);
        });
}