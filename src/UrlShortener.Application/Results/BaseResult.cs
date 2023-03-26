namespace UrlShortener.Application.Results;

public class BaseResult<TResult>
{
    public TResult? Value { get; set; }
    public IEnumerable<string> Errors { get; set; }

    public BaseResult(TResult? value)
    {
        Value = value;
        Errors = Enumerable.Empty<string>();
    }

    public BaseResult(IEnumerable<string> errors)
    {
        Value = default;
        Errors = errors;
    }
}