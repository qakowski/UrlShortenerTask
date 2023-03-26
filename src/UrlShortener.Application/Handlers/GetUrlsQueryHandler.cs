using MediatR;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Dto;
using UrlShortener.Application.Helpers;
using UrlShortener.Application.Requests;
using UrlShortener.Application.Results;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Handlers;

public class GetUrlsQueryHandler : IRequestHandler<GetUrlsQuery, BaseResult<IEnumerable<UrlsDto>>>
{
    private readonly ILogger<GetUrlsQueryHandler> _logger;
    private readonly IShortUrlRepository _shortUrlRepository;
    private readonly IUrlBuilderHelper _urlBuilderHelper;

    public GetUrlsQueryHandler(ILogger<GetUrlsQueryHandler> logger, IShortUrlRepository shortUrlRepository, IUrlBuilderHelper urlBuilderHelper)
    {
        _logger = logger;
        _shortUrlRepository = shortUrlRepository;
        _urlBuilderHelper = urlBuilderHelper;
    }

    public async Task<BaseResult<IEnumerable<UrlsDto>>> Handle(GetUrlsQuery request, CancellationToken cancellationToken)
    {
        var result = await _shortUrlRepository.GetAllAsync(request.Page ?? 0, request.Limit ?? 0);
        return new BaseResult<IEnumerable<UrlsDto>>(result.Select(x => new UrlsDto(x.Url, _urlBuilderHelper.BuildUrl(x.ShortenedUrl))));
    }
}