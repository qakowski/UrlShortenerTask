using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UrlShortener.Application.Services;

public class HttpContextBaseUrlResolver : IBaseUrlResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<HttpContextBaseUrlResolver> _logger;
    private readonly Lazy<string> _baseUrl;

    public HttpContextBaseUrlResolver(IHttpContextAccessor httpContextAccessor, ILogger<HttpContextBaseUrlResolver> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _baseUrl = new Lazy<string>(GetBaseUrlFromHttpContext);
    }
    
    public string GetBaseUrl()
    {
        return _baseUrl.Value;
    }
    
    private string GetBaseUrlFromHttpContext()
    {
        if(_httpContextAccessor.HttpContext?.Request is null)
        {
            _logger.LogCritical("HttpContext is {Context} Request is {Request}", _httpContextAccessor.HttpContext, 
                _httpContextAccessor.HttpContext?.Request);
            throw new InvalidOperationException($"Either HttpContext or Request is null");
        }
        
        var request = _httpContextAccessor.HttpContext.Request;
        var host = request.Host.Host;
        var scheme = request.Scheme;
        var port = request.Host.Port;

        var sb = new StringBuilder();
        sb.Append($"{scheme}://{host}");

        if (port.HasValue && port != 80 && port != 443)
        {
            sb.Append($":{port}");
        }

        return sb.ToString();
    }


}