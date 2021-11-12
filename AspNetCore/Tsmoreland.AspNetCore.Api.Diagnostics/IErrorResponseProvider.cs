using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public interface IErrorResponseProvider
{
    IActionResult Build(ActionContext context);
    ValueTask WriteResponse(HttpResponse response, HttpContext context);
    ValueTask WriteResponse(HttpResponse response, ExceptionDispatchInfo? edi);
    ValueTask WriteResponse(HttpResponse response, Exception? exception);
    ValueTask WriteResponse(HttpResponse response, int statusCode, string? title = null, string? details = null);

}