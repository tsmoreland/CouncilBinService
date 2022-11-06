//
// Copyright (c) 2022 Terry Moreland
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

using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public sealed class ProblemDetailsErrorProvider : IErrorResponseProvider
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private const string ProblemJsonType = "application/problem+json; charset=UTF-8";

    public ProblemDetailsErrorProvider(ProblemDetailsFactory problemDetailsFactory)
    {
        _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
    }

    /// <inheritdoc/>
    public Task WriteErrorResponse(HttpContext httpContext, Func<Task> next)
    {
        IExceptionHandlerFeature? exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
        Exception? ex = exceptionDetails?.Error;
        if (ex is null)
        {
            return next();
        }

        string? instance = httpContext.ToRequestUriOrNull()?.ToString();
        httpContext.Response.ContentType = ProblemJsonType;

        switch (ex)
        {
            case InvalidModelStateException invalidModelStateException:
                {
                    ValidationProblemDetails problem = _problemDetailsFactory
                        .CreateValidationProblemDetails(
                            httpContext,
                            invalidModelStateException.ModelState, StatusCodes.Status422UnprocessableEntity,
                            "Invalid Model State",
                            detail: "One or more fields has invalid values",
                            instance:  instance);
                    return JsonSerializer.SerializeAsync(httpContext.Response.Body, problem);
                }
            case EndpointNotFoundException endpointNotFoundException:
                {
                    httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    ProblemDetails problem = _problemDetailsFactory
                        .CreateProblemDetails(
                            httpContext,
                            statusCode: StatusCodes.Status404NotFound,
                            instance: endpointNotFoundException.Endpoint?.ToString());
                    return JsonSerializer.SerializeAsync(httpContext.Response.Body, problem);
                }
            default:
                {
                    ProblemDetails problem = _problemDetailsFactory
                        .CreateProblemDetails(
                            httpContext,
                            instance: instance);
                    return JsonSerializer.SerializeAsync(httpContext.Response.Body, problem);
                }
        }
    }
}
