namespace UrlShortener.Domain.Repositories;

public interface IStatisticsRepository
{
    Task<double> GetScoreForKey(string sortedSetName, string key);
    Task IncrementAccessCountAsync(string sortedSetName, string key);
    Task<int> GetNumberOfEntriesAsync(string key);

    Task IncrementCounterAsync(string counterKey);
}