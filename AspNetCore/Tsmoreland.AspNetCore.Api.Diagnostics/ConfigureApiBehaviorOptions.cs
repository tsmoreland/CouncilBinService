using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public sealed class ConfigureApiBehaviorOptions : IConfigureOptions<ApiBehaviorOptions>
{
    private readonly IErrorResponseProvider _errorResponseProvider;

    public ConfigureApiBehaviorOptions(IErrorResponseProvider errorResponseProvider)
    {
        _errorResponseProvider = errorResponseProvider ?? throw new ArgumentNullException(nameof(errorResponseProvider));
    }

    public void Configure(ApiBehaviorOptions options)
    {
        options.SuppressConsumesConstraintForFormFileParameters = true;
        options.SuppressInferBindingSourcesForParameters = true;
        options.SuppressModelStateInvalidFilter = true;
        options.SuppressMapClientErrors = true;
        options.InvalidModelStateResponseFactory = _errorResponseProvider.Build;
    }
}