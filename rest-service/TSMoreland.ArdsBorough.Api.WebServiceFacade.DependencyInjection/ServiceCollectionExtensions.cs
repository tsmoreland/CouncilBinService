using ArdsBorough.WebService.External;
using Microsoft.Extensions.DependencyInjection;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Shared;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Service;

namespace TSMoreland.ArdsBorough.Api.WebServiceFacade.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebServiceFacade(this IServiceCollection services)
    {
        services.AddTransient<IWebServiceFacade, WebServiceFacadeService>();
        services.AddTransient<WebService2Soap, WebService2SoapClient>();
        services.AddTransient<UrpnRepository>();
        services.AddSingleton<IWebServiceFacadeFactory, WebServiceFacadeFactory>();

        return services;
    }
}
