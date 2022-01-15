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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public sealed class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptionsMonitor<ErrorHandlerOptions> _options;
    private readonly IErrorResponseProvider _errorResponseProvider;
    private readonly List<(string Name, StringValues Values)> _headers = new();

    public ErrorHandlerMiddleware(
        RequestDelegate next, 
        IOptionsMonitor<ErrorHandlerOptions> options, 
        IErrorResponseProvider errorResponseProvider)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _errorResponseProvider = errorResponseProvider ?? throw new ArgumentNullException(nameof(errorResponseProvider));
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
        return context.Response.StatusCode < 400 
            ? Task.CompletedTask 
            : _errorResponseProvider
                .WriteResponseIfNotSetAsync(context.Response, context, _headers.ToArray())
                .AsTask();
    }

    private Task HandleException(HttpContext context, ExceptionDispatchInfo edi)
    {
        return _errorResponseProvider.WriteResponseAsync(context.Response, edi, _headers.ToArray()).AsTask();
    }
}
