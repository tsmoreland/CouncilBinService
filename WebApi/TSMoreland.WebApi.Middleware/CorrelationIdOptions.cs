namespace TSMoreland.WebApi.Middleware;

public sealed class CorrelationIdOptions
{
    internal static string DefaultHeaderName => "X-Correlation-Id";

    public string HeaderName { get; set; } = DefaultHeaderName;
}
