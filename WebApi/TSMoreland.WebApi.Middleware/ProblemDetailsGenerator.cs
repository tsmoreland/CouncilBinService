using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TSMoreland.WebApi.Middleware
{
    public sealed class ProblemDetailsGenerator
    {


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
}
