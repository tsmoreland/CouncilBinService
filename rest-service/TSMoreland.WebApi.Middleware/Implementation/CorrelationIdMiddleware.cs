using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TSMoreland.WebApi.Middleware.Implementation;

public sealed  class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptionsMonitor<CorrectionIdOptions> _options;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, IOptionsMonitor<CorrectionIdOptions> options, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Invoke(HttpContext context)
    {
        _logger.LogInformation("Begin Request {CorrectionId}", context.TraceIdentifier);
        var header = _options.CurrentValue.HeaderName;
        if (header is not { Length: > 0 })
        {
            header = CorrectionIdOptions.DefaultHeaderName;
        }

        context.Response.Headers.Add("X-Correction-Id", context.TraceIdentifier);
        await _next(context);
        _logger.LogInformation("End Request {CorrectionId}", context.TraceIdentifier);
    }
}
