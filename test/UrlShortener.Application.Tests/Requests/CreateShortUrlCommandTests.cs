using System.ComponentModel.DataAnnotations;
using UrlShortener.Application.Requests;

namespace UrlShortener.Application.Tests.Requests;

public class CreateShortUrlCommandTests
{
    [Fact]
    public void Validate_WhenValidUrlPassed_ReturnsEnumerableEmpty()
    {
        var command = new CreateShortUrlCommand { Url = "http://www.example.com" };

        var validationContext = new ValidationContext(command);
        
        var result = command.Validate(validationContext);

        result.Should().BeEquivalentTo(Enumerable.Empty<ValidationResult>());
    }
    
    [Fact]
    public void Validate_WhenInvalidUrlPassed_ReturnsEnumerableWithErrors()
    {
        var command = new CreateShortUrlCommand { Url = "invalid url" };

        var validationContext = new ValidationContext(command);
        
        var result = command.Validate(validationContext);

        result.Should().HaveCount(1);
    }
}