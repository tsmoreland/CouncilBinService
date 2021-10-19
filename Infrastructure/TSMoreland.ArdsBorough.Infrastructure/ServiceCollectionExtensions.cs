using System;
using Microsoft.Extensions.DependencyInjection;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.DependencyInjection;
using TSMoreland.ArdsBorough.Bins.DependencyInjection;

namespace TSMoreland.ArdsBorough.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddArdsBoroughInfrastructure(this IServiceCollection services)
    {
        if (services == null!)
        {
            throw new ArgumentNullException(nameof(services));
        }


        services.AddWebServiceFacade();
        services.AddBinsCollection();

        return services;
    }
}
