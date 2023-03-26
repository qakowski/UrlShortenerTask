namespace UrlShortener.Application.Helpers;

public interface IUrlBuilderHelper
{
    string BuildUrl(string hashed);
}