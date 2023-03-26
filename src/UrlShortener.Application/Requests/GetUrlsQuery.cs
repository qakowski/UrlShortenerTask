using MediatR;
using UrlShortener.Application.Dto;
using UrlShortener.Application.Results;

namespace UrlShortener.Application.Requests;

public class GetUrlsQuery : IRequest<BaseResult<IEnumerable<UrlsDto>>>
{
    public int? Limit { get; set; }
    public int? Page { get; set; }
}