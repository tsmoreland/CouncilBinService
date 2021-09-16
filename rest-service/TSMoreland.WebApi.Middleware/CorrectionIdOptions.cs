namespace TSMoreland.WebApi.Middleware;

public sealed class CorrectionIdOptions
{
    internal static string DefaultHeaderName => "X-Correlation-Id";

    public string HeaderName { get; set; } = DefaultHeaderName;
}
