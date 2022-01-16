using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public interface IErrorResponseProvider
{
    IActionResult Build(ActionContext context);

    ProblemDetails? BuildProblemFromException(int? statusCode, HttpContext? context, Exception? exception);

    ProblemDetails BuildProblem(
        HttpContext? context,
        string? detail = null,
        string? instance = null,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? traceId = null);
}
