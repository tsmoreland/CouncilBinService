using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TSMoreland.WebApi.Middleware.Implementation
{
    public sealed class ErrorHandlerMiddleware
    {
        private readonly ExceptionHandlerMiddleware _middleware;

        public ErrorHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<ExceptionHandlerOptions> options, DiagnosticListener diagnosticListener)
        {
            _middleware = new ExceptionHandlerMiddleware(next, loggerFactory, options, diagnosticListener);
        }

        public Task Invoke(HttpContext context)
        {
            return _middleware.Invoke(context);
        }
    }
}
