using UrlShortener.Application.Services;

namespace UrlShortener.Application.Helpers;

public class UrlBuilderHelper : IUrlBuilderHelper
{
    private readonly IBaseUrlResolver _baseUrlResolver;
    
    public UrlBuilderHelper(IBaseUrlResolver baseUrlResolver)
    {
        _baseUrlResolver = baseUrlResolver;
    }

    public string BuildUrl(string hashed) 
        => $"{_baseUrlResolver.GetBaseUrl()}/{hashed}";
}
