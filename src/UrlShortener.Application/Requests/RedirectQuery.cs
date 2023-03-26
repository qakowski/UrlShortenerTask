using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Results;

namespace UrlShortener.Application.Requests;

public class RedirectQuery : IRequest<BaseResult<string>>
{
    [Required]
    [FromRoute(Name = "hashed")]
    public string Hashed { get; set; }
}