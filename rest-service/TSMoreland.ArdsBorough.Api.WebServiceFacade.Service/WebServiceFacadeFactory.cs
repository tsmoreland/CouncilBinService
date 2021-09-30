using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ArdsBorough.WebService.External;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Shared;

namespace TSMoreland.ArdsBorough.Api.WebServiceFacade.Service;

public sealed class WebServiceFacadeFactory : IWebServiceFacadeFactory
{
    private readonly IServiceProvider _serviceProvider;

    public WebServiceFacadeFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IWebServiceFacade Build(string apiSecret)
    {
        var urpnRepository = _serviceProvider.GetService<UrpnRepository>() ??
                             throw new Exception("customize this to service provider exception");
        var soapService = _serviceProvider.GetService<WebService2Soap>() ??
                          throw new Exception("customize this to service provider exception");

        return new WebServiceFacadeService(apiSecret, urpnRepository, soapService);
    }
}
