using System;
using System.Net.Mime;
using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public sealed class ProblemDetailsGenerator
{
    private readonly string? _traceIdHeaderName;
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public ProblemDetailsGenerator(string? traceIdHeaderName, ProblemDetailsFactory problemDetailsFactory)
    {
        _traceIdHeaderName = traceIdHeaderName;
        _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
    }

    public IActionResult BuildForInvalidModelState(ActionContext actionContext)
    {
        var context = actionContext.HttpContext;
        int? statusCode = context.Response.StatusCode >= 400
            ? context.Response.StatusCode
            : null;

        var problem = Build(context);
        var result = new ObjectResult(problem) { StatusCode = statusCode };

        var contentType = GetResponseType(context);
        result.ContentTypes.Clear();
        result.ContentTypes.Add(new MediaTypeHeaderValue(contentType)); 

        return result;
    }

    public ProblemDetails Build(HttpContext context)
    {
        if (context.Response.StatusCode < 400)
        {
            throw new ArgumentException("Cannot build problems for successful status");
        }

        int statusCode = context.Response.StatusCode;
        var type = $"https://httpstatuses.com/{statusCode}";
        var title = GetErrorStatusDescription(statusCode);

        return BuildProblem(context, statusCode: statusCode, type: type, title: title, traceId: GetTraceIdOrDefault(context));
    }

    public ProblemDetails Build(HttpContext context, ExceptionDispatchInfo edi)
    {
        var statusCode = context.Response.StatusCode >= 400
            ? context.Response.StatusCode
            : 500;

        return BuildProblem(context, statusCode: statusCode, traceId: GetTraceIdOrDefault(context));
    }

    private string? GetTraceIdOrDefault(HttpContext context)
    {
        return _traceIdHeaderName is { Length: > 0 } 
            ? context.Response.Headers[_traceIdHeaderName].ToString()
            : null;
    }

    private static string GetResponseType(HttpContext context)
    {
        var requestJson = false;
        var requestXml = false;

        foreach (var header in context.Request.Headers.Accept)
        {
            if (string.Equals(header, MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase))
            {
                requestJson = true;
                break;
            }
            if (string.Equals(header, MediaTypeNames.Application.Xml, StringComparison.OrdinalIgnoreCase))
            {
                requestXml = true;
            }
        }

        if (requestXml && !requestJson)
        {
            return "application/problem+xml"; 
        }
        else
        {
            return "application/problem+json"; 
        }
    }

    private ProblemDetails BuildProblem(
        HttpContext context,
        string? detail = null,
        string? instance = null,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? traceId = null)
    {
        var problem = _problemDetailsFactory.CreateProblemDetails(
            context,
            statusCode: statusCode ?? 500,
            title: title,
            type: type,
            detail: detail,
            instance: instance);
        if (traceId is { Length: > 0 })
        {
            problem.Extensions["traceId"] = traceId;
        }

        return problem;
    }
    internal static string GetErrorStatusDescription(int? statusCode)
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
