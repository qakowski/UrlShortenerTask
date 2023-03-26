using UrlShortener.Application.Services;

namespace UrlShortener.WebApplication.Extensions;

internal static class BaseUrlResolver
{
    public static IServiceCollection AddBaseUrlResolver(this IServiceCollection services, IConfiguration configuration, Serilog.ILogger logger)
    {
        var baseUrl = configuration.GetValue<string>("BaseUrl");
        if (string.IsNullOrEmpty(baseUrl))
        {
            logger.Information("Registering HttpContextBaseUrlResolver");
            services.AddHttpContextAccessor();
            services.AddSingleton<IBaseUrlResolver, HttpContextBaseUrlResolver>();
        }
        else
        {
            logger.Information("Registering ConfigurationBaseUrlResolver with base url: {baseUrl}", baseUrl);
            services.AddSingleton<IBaseUrlResolver, ConfigurationBaseUrlResolver>(_ =>
                new ConfigurationBaseUrlResolver(baseUrl));
        }

        return services;
    }
}