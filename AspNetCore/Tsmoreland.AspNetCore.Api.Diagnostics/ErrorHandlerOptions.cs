using System.Collections.Generic;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public sealed class ErrorHandlerOptions
{
    public string? TraceIdentifier { get; set; } = null;

    public List<string> HeadersToKeep { get; } = new();
}
