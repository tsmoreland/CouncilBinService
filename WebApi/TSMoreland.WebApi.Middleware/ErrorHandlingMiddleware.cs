//
// Copyright © 2021 Terry Moreland
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

using System.Net.Mime;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace TSMoreland.WebApi.Middleware
{
    public sealed class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly ILogger _logger;
        private static readonly Lazy<XmlSerializer> _xmlSerializer = new(() => new XmlSerializer(typeof(ProblemDetails)));
        private readonly List<(string Name, StringValues Values)> _headers = new();

        public ErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, ProblemDetailsFactory problemDetailsFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
            _logger = loggerFactory.CreateLogger<ErrorHandlingMiddleware>();
        }

        /// <summary>
        /// Executes the middleware.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
        public Task Invoke(HttpContext context)
        {
            ExceptionDispatchInfo? edi = null;
            StoreCurrentHeaders();

            try
            {
                var task = _next.Invoke(context);
                if (!task.IsCompleted)
                {
                    return Awaited(this, context, task);
                }
            }
            catch (Exception exception)
            {
                edi = ExceptionDispatchInfo.Capture(exception);
            }

            return HandleResponse(context, edi);

            static async Task Awaited(ErrorHandlingMiddleware middleware, HttpContext context, Task task)
            {
                ExceptionDispatchInfo? edi = null;
                try
                {
                    await task;
                }
                catch (Exception exception)
                {
                    edi = ExceptionDispatchInfo.Capture(exception);
                }

                await middleware.HandleResponse(context, edi);
            }

            void StoreCurrentHeaders()
            {
                foreach (var header in context.Response.Headers)
                {
                    var (name, value) = header;
                    _headers.Add((name, value));
                }
            }
        }

        private Task HandleResponse(HttpContext context, ExceptionDispatchInfo? edi)
        {
            if (edi is not null)
            {
                return HandleException(context, edi);
            }

            if (context.Response.StatusCode >= 400)
            {
                return HandleErrorStatus(context);
            }

            _headers.Clear();
            return Task.CompletedTask;
        }

        private Task HandleErrorStatus(HttpContext context)
        {
            int? statusCode = context.Response.StatusCode >= 400
                ? context.Response.StatusCode
                : null;
            if (context.Response.ContentType.StartsWith("application/problem+", StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            var type = $"https://httpstatuses.com/{statusCode}";
            var correctionId = context.Response.Headers[CorrelationIdOptions.DefaultHeaderName].ToString();
            var title = GetErrorStatusDescription(statusCode);

            var problem = BuildProblem(context, statusCode: statusCode, type: type, title: title, traceId: correctionId);

            return WriteProblemToResponse(context, problem);
        }
        private Task HandleException(HttpContext context, ExceptionDispatchInfo edi)
        {
            var correctionId = context.Response.Headers[CorrelationIdOptions.DefaultHeaderName].ToString();
            _logger.LogError(edi.SourceException, "Error occurred processing request");

            var statusCode = context.Response.StatusCode >= 400
                ? context.Response.StatusCode
                : 500;
            var problem = BuildProblem(context, statusCode: statusCode, traceId: correctionId);

            return WriteProblemToResponse(context, problem);
        }

        private async Task WriteProblemToResponse(HttpContext context, ProblemDetails problem)
        {
            var acceptTypes = context.Request.Headers.Accept;

            bool json = acceptTypes.Any(s =>
                string.Equals(s, MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase));
            bool xml = acceptTypes.Any(s =>
                string.Equals(s, MediaTypeNames.Application.Xml, StringComparison.OrdinalIgnoreCase));

            context.Response.Clear();
            context.Response.StatusCode = problem.Status ?? 500;

            foreach (var pair in _headers)
            {
                var (name, value) = pair;
                context.Response.Headers.Add(name, value);
            }

            _headers.Clear();

            if (json || !xml)
            {
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem, typeof(ProblemDetails), CancellationToken.None);
            }
            else if (xml)
            {
                context.Response.ContentType = "application/problem+xml";

                var encoding = new UTF8Encoding(false);
                await using var writer = new StreamWriter(context.Response.Body, encoding);
                _xmlSerializer.Value.Serialize(writer, problem);
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
}
