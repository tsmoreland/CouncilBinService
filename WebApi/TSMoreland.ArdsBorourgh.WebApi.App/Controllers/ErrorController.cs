using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TSMoreland.ArdsBorough.WebApi.App.Controllers;

/// <summary>
/// based on <a href="https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-6.0">Handle errors in ASP.NET Core web APIs</a>
/// </summary>
[ApiVersion("1")]
[ApiController]
[Route("api/v{version:apiVersion}/error")]
public class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> _logger;

    /// <summary>
    /// Instantiates a new instance of the <see cref="ErrorController"/> class.
    /// </summary>
    public ErrorController(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ErrorController>();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("{id:int?}")]
    public IActionResult Error(int? id)
    {
        IExceptionHandlerPathFeature? context = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        Exception? exception = context?.Error;

        _logger.LogCritical(exception,"Unhandled exception in request {TraceIdentifier}: {Exception}",
            HttpContext.TraceIdentifier,
            exception?.GetBaseException().ToString() ?? "no exception available");

        int statusCode = id ?? StatusCodes.Status500InternalServerError;
        ProblemDetails problemDetails = new ()
        {
            Status = statusCode,
            Title = "Temporary Title",
            Detail = "Temporary Description",
            Instance = context?.Path,
        };

        return new ObjectResult(problemDetails) { StatusCode = statusCode };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="webHostEnvironment"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [HttpGet]
    [Route("api/error-dev")]
    public IActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment webHostEnvironment)
    {
        if (webHostEnvironment.EnvironmentName != "Development")
        {
            throw new InvalidOperationException(
                "This shouldn't be invoked in non-development environments.");
        }

        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (context is not null)
        {
            return Problem(
                detail: context.Error.StackTrace,
                title: context.Error.Message);
        }
        else
        {
            return Problem();
        }
    }
}
