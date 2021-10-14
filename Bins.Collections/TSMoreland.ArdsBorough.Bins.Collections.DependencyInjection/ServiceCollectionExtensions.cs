using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TSMoreland.ArdsBorough.Bins.Collections.Shared;
using TSMoreland.ArdsBorough.Bins.Services;

namespace TSMoreland.ArdsBorough.Bins.DependencyInjection;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddBinsCollection(this IServiceCollection services)
    {
        if (services == null!)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddScoped<IBinCollectionService, BinCollectionService>();

        return services;
    }
}
