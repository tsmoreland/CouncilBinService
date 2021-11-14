﻿//
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public sealed class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptionsMonitor<ErrorHandlerOptions> _options;
    private readonly IErrorResponseProvider _errorResponseProvider;
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private readonly ILogger _logger;
    private static readonly Lazy<XmlSerializer> _xmlSerializer = new(() => new XmlSerializer(typeof(ProblemDetails)));
    private readonly List<(string Name, StringValues Values)> _headers = new();

    public ErrorHandlerMiddleware(
        RequestDelegate next, 
        IOptionsMonitor<ErrorHandlerOptions> options, 
        IErrorResponseProvider errorResponseProvider,
        ILoggerFactory loggerFactory, 
        ProblemDetailsFactory problemDetailsFactory)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _errorResponseProvider = errorResponseProvider ?? throw new ArgumentNullException(nameof(errorResponseProvider));
        _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
        _logger = loggerFactory.CreateLogger<ErrorHandlerMiddleware>();
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

        static async Task Awaited(ErrorHandlerMiddleware middleware, HttpContext context, Task task)
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
        var traceId = _options.CurrentValue.TraceIdentifier;
        var title = ProblemDetailsGenerator.GetErrorStatusDescription(statusCode);

        var problem = BuildProblem(context, statusCode: statusCode, type: type, title: title, traceId: traceId);

        return WriteProblemToResponse(context, problem);
    }

    private Task HandleException(HttpContext context, ExceptionDispatchInfo edi)
    {
        var traceId = _options.CurrentValue.TraceIdentifier;
        _logger.LogError(edi.SourceException, "Error occurred processing request");

        var statusCode = context.Response.StatusCode >= 400
            ? context.Response.StatusCode
            : 500;
        var problem = BuildProblem(context, statusCode: statusCode, traceId: traceId);

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

}