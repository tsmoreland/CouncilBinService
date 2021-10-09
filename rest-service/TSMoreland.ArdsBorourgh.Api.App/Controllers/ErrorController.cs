using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace TSMoreland.ArdsBorough.Api.App.Controllers;

/// <summary>
/// based on <a href="https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-6.0">Handle errors in ASP.NET Core web APIs</a>
/// </summary>
[ApiVersion("1")]
[ApiController]
public class ErrorController : ControllerBase
{
    [Route("api/error")]
    public IActionResult Error()
    {
        return Problem();
    }

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
