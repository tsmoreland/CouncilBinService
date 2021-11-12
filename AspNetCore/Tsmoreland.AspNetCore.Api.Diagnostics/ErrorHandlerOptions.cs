using System.Collections.Generic;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public sealed class ErrorHandlerOptions
{
    public string? TraceIdentifier { get; set; } = null;

    /// <summary>
    /// Additional Headers to include if already present on error
    /// </summary>
    public List<string> HeadersToKeep { get; } = new();

    /// <summary>
    /// If <see langword="true"/> then headers present
    /// in the response before the middleware begins will be kept
    /// on error
    /// </summary>
    public bool PreserveHeaders { get; set; }
}
