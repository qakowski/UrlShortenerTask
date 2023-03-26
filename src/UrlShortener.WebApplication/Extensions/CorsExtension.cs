namespace UrlShortener.WebApplication.Extensions;

internal static class CorsExtension
{
    public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration,
        Serilog.ILogger logger)
    {
        var allowedOrigins = configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

        if (allowedOrigins == null || !allowedOrigins.Any())
        {
            logger.Warning("No allowed origins found in configuration");
            return services;
        }

        logger.Information("Adding cors: [{allowedOrigins}]", allowedOrigins);
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();

                builder.SetIsOriginAllowed(origin =>
                {
                    if (string.IsNullOrEmpty(origin) || origin.Equals("null"))
                    {
                        return true; 
                    }

                    return Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
                           uri.Host is "localhost" or "127.0.0.1";

                });
            });
        });

        return services;
    }
}