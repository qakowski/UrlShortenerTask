using System.ComponentModel.DataAnnotations;
using MediatR;
using UrlShortener.Application.Dto;
using UrlShortener.Application.Results;

namespace UrlShortener.Application.Requests;


public class CreateShortUrlCommand : IRequest<BaseResult<ShortUrlDto>>, IValidatableObject
{
    [Required]
    public string Url { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!IsValidUrl(Url))
            yield return new ValidationResult("Invalid URL passed");
    }

    private static bool IsValidUrl(string url)
    {
        var result = Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
            (uriResult?.Scheme == Uri.UriSchemeHttp || uriResult?.Scheme == Uri.UriSchemeHttps);

        return result;
    }
        
}