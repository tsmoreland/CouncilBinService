using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TSMoreland.WebApi.Middleware;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerGenWithVersioning(this IServiceCollection services, Action<SwaggerGenOptions, IApiVersionDescriptionProvider> setupAction)
    {
        return services.AddSwaggerGenWithVersioning(Options.DefaultName, setupAction);
    }

    public static IServiceCollection AddSwaggerGenWithVersioning(this IServiceCollection services, string name, Action<SwaggerGenOptions, IApiVersionDescriptionProvider> setupAction)
    {
        if (services == null!)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddSwaggerGen();
        services.AddSingleton<IConfigureOptions<SwaggerGenOptions>>(provider => 
            new VersionedSwashbuckleConfiguration(
                name, 
                provider.GetRequiredService<IApiVersionDescriptionProvider>(), 
                setupAction)); 
        return services;
    }
}
