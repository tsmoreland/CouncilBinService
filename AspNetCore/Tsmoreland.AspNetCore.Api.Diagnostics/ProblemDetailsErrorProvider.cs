using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public sealed class ProblemDetailsErrorProvider : IErrorResponseProvider
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ProblemDetailsErrorProvider> _logger;
    private const string ProblemJsonType = "application/problem+json";
    private const string ProblemXmlType = "application/problem+xml";

    public ProblemDetailsErrorProvider(
        IHttpContextAccessor httpContextAccessor, 
        ILoggerFactory loggerFactory)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        ArgumentNullException.ThrowIfNull(loggerFactory, nameof(loggerFactory));
        _logger = loggerFactory.CreateLogger<ProblemDetailsErrorProvider>();

        _problemDetailsFactory = null!;
    }

    /*
    public ProblemDetailsErrorProvider(
        ProblemDetailsFactory problemDetailsFactory,
        IHttpContextAccessor httpContextAccessor,
        ILoggerFactory loggerFactory)
    {
        _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        ArgumentNullException.ThrowIfNull(loggerFactory, nameof(loggerFactory));
        _logger = loggerFactory.CreateLogger<ProblemDetailsErrorProvider>();
    }
    */

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

    /// <inheritdoc/>
    public ValueTask WriteResponseIfNotSetAsync(
        HttpResponse response, 
        HttpContext context, 
        IEnumerable<(string Name, StringValues Values)> additionalHeaders, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogError("Return error {StatusCode}", context.Response.StatusCode);

        // do nothing if content-type is a problem type already
        var contentType = response.ContentType;
        if (string.Equals(contentType, "application/problem+json", StringComparison.OrdinalIgnoreCase) || 
            string.Equals(contentType, "application/problem+xml", StringComparison.OrdinalIgnoreCase))
        {
            return ValueTask.CompletedTask;
        }

        return WriteResponseAsync(response, context.GetProblemResponseTypeFromAccept(), BuildProblem(context), additionalHeaders, cancellationToken);
    }

    /// <inheritdoc/>
    public ValueTask WriteResponseAsync(
        HttpResponse response, 
        ExceptionDispatchInfo? edi, 
        IEnumerable<(string Name, StringValues Values)> additionalHeaders, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogError(edi?.SourceException, "Exception occurred, returning error {Message}",
            edi?.SourceException.Message ?? "Unknown"); 
        var context = _httpContextAccessor.HttpContext;
        return WriteResponseAsync(response, 
            context?.GetProblemResponseTypeFromAccept() ?? "application/problem+json", 
            BuildProblemFromException(context, edi?.SourceException) ?? BuildProblem(context), 
            additionalHeaders,
            cancellationToken);
    }

    /// <inheritdoc/>
    public ValueTask WriteResponseAsync(
        HttpResponse response, 
        Exception? exception, 
        IEnumerable<(string Name, StringValues Values)> additionalHeaders, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogError(exception, "Exception occurred, returning error {Message}",
            exception?.Message ?? "Unknown"); 
        var context = _httpContextAccessor.HttpContext;
        return WriteResponseAsync(response, 
            context?.GetProblemResponseTypeFromAccept() ?? ProblemJsonType, 
            BuildProblemFromException(context, exception) ?? BuildProblem(context), 
            additionalHeaders,
            cancellationToken);
    }

    /// <inheritdoc/>
    public ValueTask WriteResponseAsync(
        HttpResponse response, 
        IEnumerable<(string Name, StringValues Values)> additionalHeaders, 
        int statusCode, 
        string? title = null, 
        string? details = null, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogError("Return error {StatusCode}: {Title}", statusCode, title);
        var context = _httpContextAccessor.HttpContext;
        return WriteResponseAsync(response, 
            "application/problem+json", 
            BuildProblem(context, details,  statusCode: statusCode, title: title), 
            additionalHeaders,
            cancellationToken);
    }

    private static ValueTask WriteResponseAsync(
        HttpResponse response, 
        string contentType, 
        ProblemDetails problem, 
        IEnumerable<(string Name, StringValues Values)> additionalHeaders, 
        CancellationToken cancellationToken)
    {
        if (response.HasStarted)
        {
            throw new NotSupportedException("Unable to write content after response has started");
        }
        response.ContentType = contentType;
        response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;

        foreach (var pair in additionalHeaders)
        {
            var (name, value) = pair;
            response.Headers.Add(name, value);
        }

        return new ValueTask(response.WriteAsJsonAsync(problem, cancellationToken));
    }

    private ProblemDetails? BuildProblemFromException(HttpContext? context, Exception? exception)
    {
        if (exception == null)
        {
            return null;
        }

        var statusCode = context?.Response.StatusCode >= 400
            ? context.Response.StatusCode 
            : StatusCodes.Status500InternalServerError;
        ProblemDetails problem;
        if (context != null)
        {
            problem = _problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode,
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

    private ProblemDetails BuildProblem(
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
