using MediatR;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Requests;
using UrlShortener.Application.Results;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Handlers;

public class RedirectQueryHandler : IRequestHandler<RedirectQuery, BaseResult<string>>
{
    private ILogger<RedirectQueryHandler> _logger;
    private readonly IShortUrlRepository _shortUrlRepository;

    public RedirectQueryHandler(ILogger<RedirectQueryHandler> logger, IShortUrlRepository shortUrlRepository)
    {
        _logger = logger;
        _shortUrlRepository = shortUrlRepository;
    }

    public async Task<BaseResult<string>> Handle(RedirectQuery request, CancellationToken cancellationToken)
    {
        var result = await _shortUrlRepository.GetAsync(request.Hashed);

        return string.IsNullOrEmpty(result) ? new NotFoundResult(new[] {$"Short URL with hash '{request.Hashed}' not found."}) : new BaseResult<string>(result);
    }
}