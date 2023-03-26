namespace UrlShortener.Application.Services;

public class ConfigurationBaseUrlResolver : IBaseUrlResolver
{
    private readonly string _baseUrl;

    public ConfigurationBaseUrlResolver(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public string GetBaseUrl()
        => _baseUrl;
}