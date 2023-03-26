using System.Reflection;
using Microsoft.OpenApi.Models;

namespace UrlShortener.WebApplication.Extensions;

internal static class SwaggerExtension
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "UrlShortener",
                Description = ".NET Core API for shortening URLs",
            });
            
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        return services;
    }

    public static IApplicationBuilder UseSwagger(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseSwagger(options =>
        {
            options.RouteTemplate = "api/swagger/{documentname}/swagger.json";
        });
        applicationBuilder.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = "api/swagger";
        });

        return applicationBuilder;
    }
}