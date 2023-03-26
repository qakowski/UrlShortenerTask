namespace UrlShortener.Data.Redis;

public class RedisConfiguration
{
    /// <summary>
    /// Time to live of redis entry expressed in minutes
    /// </summary>
    public int TimeToLive { get; set; }
    
    /// <summary>
    /// Connection string to Redis instance
    /// </summary>
    public string ConnectionString { get; set; }
}