using ArdsBorough.WebService.External;
using Microsoft.Extensions.DependencyInjection;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Infrastructure;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Shared;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Services;

namespace TSMoreland.ArdsBorough.Api.WebServiceFacade.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebServiceFacade(this IServiceCollection services)
    {
        services.AddTransient<UrpnRepository>();
        services.AddSingleton<IWebServiceFacadeFactory, WebServiceFacadeFactory>();
        services.AddSingleton<WebService2SoapFactory>();

        return services;
    }
}
