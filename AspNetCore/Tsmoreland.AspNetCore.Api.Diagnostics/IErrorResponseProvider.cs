using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public interface IErrorResponseProvider
{
    IActionResult Build(ActionContext context);
    ValueTask WriteResponseAsync(HttpResponse response, HttpContext context, CancellationToken cancellationToken = default);
    ValueTask WriteResponseAsync(HttpResponse response,  ExceptionDispatchInfo? edi, CancellationToken cancellationToken = default);
    ValueTask WriteResponseAsync(HttpResponse response, Exception? exception, CancellationToken cancellationToken = default);
    ValueTask WriteResponseAsync(HttpResponse response, int statusCode, string? title = null, string? details = null, CancellationToken cancellationToken = default);

}