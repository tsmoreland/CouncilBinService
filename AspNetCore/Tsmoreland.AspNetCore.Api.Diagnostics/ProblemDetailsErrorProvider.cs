//
// Copyright © 2022 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public sealed class ProblemDetailsErrorProvider : IErrorResponseProvider
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private readonly ILogger<ProblemDetailsErrorProvider> _logger;
    private const string ProblemJsonType = "application/problem+json";
    private const string ProblemXmlType = "application/problem+xml";

    public ProblemDetailsErrorProvider(
        ProblemDetailsFactory problemDetailsFactory,
        ILoggerFactory loggerFactory)
    {
        _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
        ArgumentNullException.ThrowIfNull(loggerFactory, nameof(loggerFactory));
        _logger = loggerFactory.CreateLogger<ProblemDetailsErrorProvider>();
    }

    /// <inheritdoc/>
    public IActionResult Build(ActionContext context)
    {
        var problem = _problemDetailsFactory.CreateValidationProblemDetails(
            context.HttpContext,
            context.ModelState,
            StatusCodes.Status400BadRequest,
            GetErrorStatusDescription(StatusCodes.Status400BadRequest),
            $"https://httpstatuses.com/{StatusCodes.Status400BadRequest}");
        problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

        var result = new BadRequestObjectResult(problem);
        result.ContentTypes.Clear();
        result.ContentTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue(ProblemJsonType));
        result.ContentTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue(ProblemXmlType));
        return result;
    }


    public ProblemDetails? BuildProblemFromException(int? statusCode, HttpContext? context, Exception? exception)
    {
        if (exception == null)
        {
            return null;
        }

        int status = statusCode ?? StatusCodes.Status500InternalServerError;

        ProblemDetails problem;
        if (context != null)
        {
            problem = _problemDetailsFactory.CreateProblemDetails(
                context,
                status,
                GetErrorStatusDescription(statusCode),
                $"https://httpstatuses.com/{statusCode}");
            problem.Extensions["traceId"] = context.TraceIdentifier;
        }
        else
        {
            problem = new ProblemDetails
            {
                Status = statusCode,
                Title = GetErrorStatusDescription(statusCode),
                Type = $"https://httpstatuses.com/{statusCode}",
            };
        }

        return problem;
    }

    public ProblemDetails BuildProblem(
        HttpContext? context,
        string? detail = null,
        string? instance = null,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? traceId = null)
    {
        statusCode ??= context?.Response.StatusCode >= 400
            ? context.Response.StatusCode
            : StatusCodes.Status500InternalServerError;

        ProblemDetails problem;
        if (context != null)
        {
            problem = _problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode,
                title ?? GetErrorStatusDescription(statusCode),
                type ?? $"https://httpstatuses.com/{statusCode}");
            problem.Extensions["traceId"] = traceId ?? context.TraceIdentifier;
        }
        else
        {
            problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title ?? GetErrorStatusDescription(statusCode),
                Type = type ?? $"https://httpstatuses.com/{statusCode}",
            };
            if (traceId != null)
            {
                problem.Extensions["traceId"] = traceId;
            }
        }
        problem.Detail = detail;
        problem.Instance = instance;

        return problem;
    }

    private static string GetErrorStatusDescription(int? statusCode)
    {
        var value = statusCode ?? 0;

        return value switch
        {
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status401Unauthorized => "Not authorized",
            StatusCodes.Status402PaymentRequired => "Payment required",
            StatusCodes.Status403Forbidden => "Forbidden",
            StatusCodes.Status404NotFound => "Not Found",
            StatusCodes.Status405MethodNotAllowed => "Method not allowed",
            StatusCodes.Status406NotAcceptable => "Not acceptable",
            StatusCodes.Status407ProxyAuthenticationRequired => "Proxy authentication required",
            StatusCodes.Status408RequestTimeout => "operation timed out",
            StatusCodes.Status409Conflict => "confict",
            StatusCodes.Status410Gone => "Resource not available",
            StatusCodes.Status411LengthRequired => "Missing Required Length",
            StatusCodes.Status412PreconditionFailed => "One or more preconditions failed",
            StatusCodes.Status413PayloadTooLarge => "Pay lod too large",
            StatusCodes.Status414RequestUriTooLong => "Uri too long or",
            StatusCodes.Status415UnsupportedMediaType => "Unsupported Media Type",
            StatusCodes.Status416RangeNotSatisfiable => "Provided range cannot be fulfilled",
            StatusCodes.Status417ExpectationFailed => "Cannot fulfill request-header requirements",
            StatusCodes.Status418ImATeapot => "I'm a teapot",
            StatusCodes.Status419AuthenticationTimeout => "Authentication timed out",
            StatusCodes.Status421MisdirectedRequest => "Invalid misdirect",
            StatusCodes.Status422UnprocessableEntity => "Unable to Process",
            StatusCodes.Status423Locked => "Resource is locked",
            425 => "Request called too early",
            StatusCodes.Status426UpgradeRequired => "Protocol must be upgraded",
            StatusCodes.Status428PreconditionRequired => "Precondition required",
            StatusCodes.Status429TooManyRequests => "Too many requests",
            StatusCodes.Status431RequestHeaderFieldsTooLarge => "Request Headers too large",
            StatusCodes.Status451UnavailableForLegalReasons => "Legally unavailable",
            StatusCodes.Status500InternalServerError => "Internal Error",
            StatusCodes.Status501NotImplemented => "Not Implemented",
            StatusCodes.Status502BadGateway => "Bad Gateway",
            StatusCodes.Status503ServiceUnavailable => "Services unavailable",
            StatusCodes.Status504GatewayTimeout => "Gateway timeout",
            StatusCodes.Status505HttpVersionNotsupported => "HTTP Version not supported",
            StatusCodes.Status506VariantAlsoNegotiates => "Variant also Negotiates (possible circular reference)",
            StatusCodes.Status507InsufficientStorage => "Insufficient Storage",
            StatusCodes.Status508LoopDetected => "Loop Detected",
            StatusCodes.Status510NotExtended => "Not Extended",
            StatusCodes.Status511NetworkAuthenticationRequired => "Network Authentication required.",
            _ => "Unknown Error",
        };
    }

}
