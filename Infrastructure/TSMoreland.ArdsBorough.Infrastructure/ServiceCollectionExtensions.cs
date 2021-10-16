using System;
using Microsoft.Extensions.DependencyInjection;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.DependencyInjection;
using TSMoreland.ArdsBorough.Bins.DependencyInjection;

namespace TSMoreland.ArdsBorough.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiInfrastructure(this IServiceCollection services)
    {
        if (services == null!)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddAutoMapper(typeof(Profiles.MappingProfile).Assembly);

        services.AddWebServiceFacade();
        services.AddBinsCollection();

        return services;
    }
}
