using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public interface IErrorResponseProvider
{
    IActionResult Build(ActionContext context);
    ValueTask WriteResponseIfNotSetAsync(HttpResponse response, HttpContext context, IEnumerable<(string Name, StringValues Values)> additionalHeaders, CancellationToken cancellationToken = default);
    ValueTask WriteResponseAsync(HttpResponse response, ExceptionDispatchInfo? edi, IEnumerable<(string Name, StringValues Values)> additionalHeaders, CancellationToken cancellationToken = default);
    ValueTask WriteResponseAsync(HttpResponse response, Exception? exception, IEnumerable<(string Name, StringValues Values)> additionalHeaders, CancellationToken cancellationToken = default);
    ValueTask WriteResponseAsync(HttpResponse response, IEnumerable<(string Name, StringValues Values)> additionalHeaders, int statusCode, string? title = null, string? details = null, CancellationToken cancellationToken = default);

}