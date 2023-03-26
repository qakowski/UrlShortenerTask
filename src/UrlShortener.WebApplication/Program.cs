using System.Reflection;
using Serilog;
using Serilog.Events;
using UrlShortener.Application.Helpers;
using UrlShortener.Application.Results;
using UrlShortener.Domain.Services;
using UrlShortener.WebApplication.Extensions;

var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

Log.Logger = logger;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

builder.Logging.AddSerilog(Log.Logger);

var configuration = builder.Configuration;
var services = builder.Services;

services.AddControllersWithViews();

services.AddBaseUrlResolver(configuration, logger)
    .AddSwagger()
    .AddDbAccess(configuration, logger)
    .AddCors(configuration, logger);

services.AddScoped<IUrlBuilderHelper, UrlBuilderHelper>();
services.AddTransient<IUrlShortenerService, UrlShortenerService>();
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(BaseResult<>))!));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    logger.Information("Starting Swagger");
    app.UseSwagger();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();