using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Infrastructure;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Shared;

namespace TSMoreland.ArdsBorough.Api.WebServiceFacade.Services;

public sealed class WebServiceFacadeFactory : IWebServiceFacadeFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;

    public WebServiceFacadeFactory(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public IWebServiceFacade Build(string apiSecret)
    {
        var urpnRepository = _serviceProvider.GetService<UrpnRepository>() ??
                             throw new Exception("customize this to service provider exception");
        var soapServiceFactory = _serviceProvider.GetService<WebService2SoapFactory>() ??
                          throw new Exception("customize this to service provider exception");

        return new WebServiceFacadeService(apiSecret, urpnRepository, soapServiceFactory, _loggerFactory.CreateLogger<WebServiceFacadeService>());
    }
}
