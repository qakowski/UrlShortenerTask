namespace UrlShortener.Application.Dto;

public class ShortUrlDto
{
    public string Url { get; set; }

    public ShortUrlDto(string url)
    {
        Url = url;
    }
}