using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public static class InvalidModelStateResponseFactoryFunctions
{
    public static IActionResult HandleInvalidModelState(ActionContext context)
    {

        var result = new BadRequestObjectResult(context.ModelState);
        foreach (var mimeType in new[] { MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml })
        {
            result.ContentTypes.Add(mimeType);
        }

        return result;
    }
}