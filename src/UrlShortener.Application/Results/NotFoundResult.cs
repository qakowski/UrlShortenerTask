namespace UrlShortener.Application.Results;

public class NotFoundResult : BaseResult<string>
{
    public NotFoundResult(IEnumerable<string> errors) : base(errors)
    {
    }
}