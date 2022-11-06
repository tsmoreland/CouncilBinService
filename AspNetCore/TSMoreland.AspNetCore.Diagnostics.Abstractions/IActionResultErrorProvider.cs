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

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TSMoreland.AspNetCore.Diagnostics.Abstractions;

/// <summary>
/// Provides translation of errors to <see cref="IActionResult"/> instance
/// </summary>
/// <remarks>
/// intended to be called from an error controller used by <see cref="ExceptionHandlerMiddleware"/>
/// such as <code>app.UseExceptionHandler("/error")</code>
/// </remarks>
public interface IActionResultErrorProvider
{
    /// <summary>
    /// Builds an instance of <see cref="IActionResult"/> using details provided in <paramref name="controller"/>
    /// </summary>
    /// <param name="controller">
    /// reference to error controller which in turn can provide <see cref="HttpContext"/>
    /// and <see cref="IExceptionHandlerFeature"/>
    /// </param>
    /// <returns>
    /// <see cref="IActionResult"/> containing the error response to be returned
    /// </returns>
    IActionResult ConstructErrorResponse(ControllerBase controller);

    /// <summary>
    /// Builds an instance of <see cref="IActionResult"/> using details provided in <paramref name="controller"/>
    /// </summary>
    /// <param name="controller">
    /// reference to error controller which in turn can provide <see cref="HttpContext"/>
    /// and <see cref="IExceptionHandlerFeature"/>
    /// </param>
    /// <returns>
    /// An asynchronous task which upon completion contains an instance of
    /// <see cref="IActionResult"/> containing the error response 
    /// </returns>
    ValueTask<IActionResult> ConstructErrorResponseAsync(ControllerBase controller);
}
