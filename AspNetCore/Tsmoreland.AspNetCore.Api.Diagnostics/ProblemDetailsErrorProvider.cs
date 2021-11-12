using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public sealed class ProblemDetailsErrorProvider : IErrorResponseProvider
{
    private readonly ProblemDetailsGenerator _problemDetailsGenerator;
    private readonly ILogger<ProblemDetailsErrorProvider> _logger;

    public ProblemDetailsErrorProvider(ProblemDetailsGenerator problemDetailsGenerator, ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory, nameof(loggerFactory));
        _problemDetailsGenerator = problemDetailsGenerator ?? throw new ArgumentNullException(nameof(problemDetailsGenerator));
        _logger = loggerFactory.CreateLogger<ProblemDetailsErrorProvider>();
    }

    /// <inheritdoc/>
    public IActionResult Build(ActionContext context)
    {
        return _problemDetailsGenerator.BuildForInvalidModelState(context);
    }

    /// <inheritdoc/>
    public ValueTask WriteResponse(HttpResponse response, HttpContext context)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ValueTask WriteResponse(HttpResponse response, ExceptionDispatchInfo? edi)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ValueTask WriteResponse(HttpResponse response, Exception? exception)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ValueTask WriteResponse(HttpResponse response, int statusCode, string? title = null, string? details = null)
    {
        throw new NotImplementedException();
    }



}