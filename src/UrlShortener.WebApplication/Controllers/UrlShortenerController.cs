using Microsoft.AspNetCore.Mvc;
using MediatR;
using UrlShortener.Application.Dto;
using UrlShortener.Application.Requests;
using UrlShortener.Application.Results;
using NotFoundResult = UrlShortener.Application.Results.NotFoundResult;

namespace UrlShortener.WebApplication.Controllers
{
    //TODO: Could add versioning
    [ApiController]
    [Route("/api/")]
    public class UrlShortenerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UrlShortenerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create a short URL based on the provided long URL.
        /// </summary>
        /// <param name="command">The command containing the long URL to be shortened.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A newly created short URL or errors if the operation failed.</returns>
        [HttpPost("shorten")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResult<ShortUrlDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResult<ShortUrlDto>))]
        public async Task<IActionResult> CreateShortUrl([FromBody] CreateShortUrlCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result.Errors.Any())
                return BadRequest(result);

            return Created(Url.Link("/", result.Value.Url) ?? string.Empty, result);
        }
        
        /// <summary>
        /// Returns a paginated list of all URLs that have been shortened.
        /// </summary>
        /// <param name="query">The query containing the pagination parameters (page number and page size).</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A paginated list of short URLs and their corresponding long URLs.</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ShortUrlDto>))]
        public async Task<IActionResult> GetUrls([FromQuery] GetUrlsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            
            return Ok(result.Value);
        }

        /// <summary>
        /// Redirects to url based on hashed value
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("/{hashed}")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
        public async Task<IActionResult> RedirectToUrl([FromRoute] RedirectQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);

            if (result is NotFoundResult || result.Value is null)
                return NotFound(result);
            
            return Redirect(result.Value);

        }
    }
}