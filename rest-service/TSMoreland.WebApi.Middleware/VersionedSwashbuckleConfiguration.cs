using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TSMoreland.WebApi.Middleware;

public sealed class VersionedSwashbuckleConfiguration : ConfigureNamedOptions<SwaggerGenOptions, IApiVersionDescriptionProvider> 
{
    public VersionedSwashbuckleConfiguration(
        string name, 
        IApiVersionDescriptionProvider apiVersionDescriptionProvider, 
        Action<SwaggerGenOptions, IApiVersionDescriptionProvider> setupAction)
        : base(name, apiVersionDescriptionProvider, setupAction)
    {
    }
}
