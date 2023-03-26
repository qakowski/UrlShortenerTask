using StackExchange.Redis;
using UrlShortener.Data.Redis;
using UrlShortener.Data.Repositories;
using UrlShortener.Domain.Repositories;
using ILogger = Serilog.ILogger;

namespace UrlShortener.WebApplication.Extensions;

internal static class DbAccessExtension
{
    public static IServiceCollection AddDbAccess(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        if (configuration.GetValue<bool>("UseRedis"))
        {
            logger.Information("Using redis as storage");
            var redisConfiguration = new RedisConfiguration();
            configuration.GetSection("Redis").Bind(redisConfiguration);
            
            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConfiguration.ConnectionString));
            services.AddSingleton<IStatisticsRepository, RedisStatisticsRepository>();
            services.AddSingleton<IShortUrlRepository>(sp =>
            {
                var connectionMultiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
                var statisticsRepository = sp.GetRequiredService<IStatisticsRepository>();
                var repositoryLogger = sp.GetRequiredService<ILogger<RedisShortUrlRepository>>();
                
                return new RedisShortUrlRepository(connectionMultiplexer, redisConfiguration, statisticsRepository, repositoryLogger);
            });
        }
        else
        {
            logger.Information("Using InMemoryStorage");
            services.AddSingleton<IShortUrlRepository, InMemoryStorage>();
        }

        return services;
    }
}