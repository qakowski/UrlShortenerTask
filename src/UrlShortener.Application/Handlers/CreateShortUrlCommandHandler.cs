using MediatR;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Dto;
using UrlShortener.Application.Helpers;
using UrlShortener.Application.Requests;
using UrlShortener.Application.Results;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.Services;

namespace UrlShortener.Application.Handlers;

public class CreateShortUrlCommandHandler : IRequestHandler<CreateShortUrlCommand, BaseResult<ShortUrlDto>>
{
    private readonly ILogger<CreateShortUrlCommandHandler> _logger;
    private readonly IShortUrlRepository _shortUrlRepository;
    private readonly IUrlShortenerService _urlShortenerService;
    private readonly IUrlBuilderHelper _urlBuilderHelper;

    public CreateShortUrlCommandHandler(ILogger<CreateShortUrlCommandHandler> logger, 
        IShortUrlRepository shortUrlRepository, 
        IUrlShortenerService urlShortenerService, 
        IUrlBuilderHelper urlBuilderHelper)
    {
        _logger = logger;
        _shortUrlRepository = shortUrlRepository;
        _urlShortenerService = urlShortenerService;
        _urlBuilderHelper = urlBuilderHelper;
    }

    public async Task<BaseResult<ShortUrlDto>> Handle(CreateShortUrlCommand request,
        CancellationToken cancellationToken)
    {
        var url = request.Url;
        var currentEntries = await _shortUrlRepository.GetNumberOfEntriesAsync();
        var hashed = _urlShortenerService.GenerateShortUrl(url, currentEntries);

        _logger.LogInformation("Adding shortened url: {url} - {hashedValue}", url, hashed);

        var result = await _shortUrlRepository.AddAsync(new ShortUrl(hashed, url));

        if (result)
            return new BaseResult<ShortUrlDto>(new ShortUrlDto(_urlBuilderHelper.BuildUrl(hashed)));

        _logger.LogError("Problem occured while trying to create shorten url for: {url} - {hashedValue}", request.Url,
            hashed);
        
        return new BaseResult<ShortUrlDto>(new[]
            { $"Problem occured while trying to create shorten url for: {request.Url}" });
    }

}