using StackExchange.Redis;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Data.Repositories;

public class RedisStatisticsRepository : IStatisticsRepository
{
    private readonly IDatabase _database;

    public RedisStatisticsRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<double> GetScoreForKey(string sortedSetName, string key) 
        => await _database.SortedSetScoreAsync(sortedSetName, key) + 1 ?? 1;

    public async Task IncrementAccessCountAsync(string sortedSetName, string key) 
        => await _database.SortedSetIncrementAsync(sortedSetName, key, 1);

    public async Task<int> GetNumberOfEntriesAsync(string key)
    {
        var counter = await _database.StringGetAsync(key);

        return counter.HasValue ? (int)counter : 0;
    }

    public async Task IncrementCounterAsync(string counterKey) 
        => await _database.StringIncrementAsync(counterKey);
}