﻿//
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
using Microsoft.AspNetCore.Mvc.Filters;

namespace TSMoreland.AspNetCore.Diagnostics.Abstractions.Filters;

/// <summary>
/// Ensures that <see cref="ActionExecutingContext.ModelState"/> is valid
/// throwing a <see cref="InvalidModelStateException"/>  if it is not so
/// that it can be picked up by <see cref="ExceptionHandlerMiddleware"/>
/// </summary>
public sealed class ValidateModelStateActionFilter : IActionFilter
{
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="context"/> is <see langword="null"/>
    /// </exception>
    /// <exception cref="InvalidModelStateException">
    /// if <paramref name="context.ModelState.IsValid"/> returns
    /// <see langword="false"/>
    /// </exception>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        InvalidModelStateException.ThrowIfNotValid(context.ModelState);
    }

    /// <inheritdoc />
    public void OnActionExecuted(ActionExecutedContext context)
    {
        // no-op
    }
}