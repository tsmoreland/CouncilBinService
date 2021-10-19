using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TSMoreland.ArdsBorough.Infrastructure;

namespace TSMoreland.ArdsBorough.WebApi.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiInfrastructure(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Profiles.MappingProfile).Assembly);
        services.AddArdsBoroughInfrastructure();
        return services;
    }

}
