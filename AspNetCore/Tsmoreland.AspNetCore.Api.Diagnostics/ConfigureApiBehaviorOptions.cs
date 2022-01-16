using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public sealed class ConfigureApiBehaviorOptions : IConfigureOptions<ApiBehaviorOptions>
{
    public void Configure(ApiBehaviorOptions options)
    {
        options.SuppressConsumesConstraintForFormFileParameters = true;
        options.SuppressInferBindingSourcesForParameters = true;
        options.SuppressModelStateInvalidFilter = true;
        options.SuppressMapClientErrors = true;

        options.InvalidModelStateResponseFactory = context => throw new Exception();
    }
}
