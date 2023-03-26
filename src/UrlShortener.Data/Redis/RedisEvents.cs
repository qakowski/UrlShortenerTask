namespace UrlShortener.Data.Redis;

public static class RedisChannels
{
    public static string KeyExpiredChannel => "__keyevent@0__:expired";
}