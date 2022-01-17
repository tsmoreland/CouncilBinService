//
// Copyright © 2022 Terry Moreland
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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public static class ProblemDetailsExceptionHandler
{
    public static void UseProblemDetailsError(this IApplicationBuilder app, IHostEnvironment environment)
    {
        IErrorResponseProvider errorResponseProvider = app.ApplicationServices.GetRequiredService<IErrorResponseProvider>();

        ProblemDetailsFactory problemDetailsFactory = app.ApplicationServices.GetRequiredService<ProblemDetailsFactory>();

        app.Use((context, next) => WriteResponse(context, next, errorResponseProvider, problemDetailsFactory, includeDetails: environment.IsDevelopment()));
    }

    private static Task WriteResponse(HttpContext httpContext, Func<Task> next, IErrorResponseProvider errorResponseProvider, ProblemDetailsFactory problemDetailsFactory, bool includeDetails)
    {
        IExceptionHandlerFeature? exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
        Exception? ex = exceptionDetails?.Error;

        if (ex is null)
        {
            return next();
        }

        httpContext.Response.ContentType = "application/problem+json; charset=UTF-8";
        if (ex is InvalidModelStateException invalidModelStateException)
        {
            ValidationProblemDetails problem = problemDetailsFactory.CreateValidationProblemDetails(httpContext,
                invalidModelStateException.ModelState, StatusCodes.Status422UnprocessableEntity, "Invalid Model State", detail: "One or more fields has invalid values");
            return JsonSerializer.SerializeAsync(httpContext.Response.Body, problem);
        }
        else
        {
            ProblemDetails problem = problemDetailsFactory.CreateProblemDetails(httpContext);
            return JsonSerializer.SerializeAsync(httpContext.Response.Body, problem);
        }

    }
}
