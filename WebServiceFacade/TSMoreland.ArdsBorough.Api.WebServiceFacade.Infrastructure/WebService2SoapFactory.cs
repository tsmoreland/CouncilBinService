using ArdsBorough.WebService.External;

namespace TSMoreland.ArdsBorough.Api.WebServiceFacade.Infrastructure;

public class WebService2SoapFactory
{
    public WebService2Soap Build()
    {
        return new WebService2SoapClient(WebService2SoapClient.EndpointConfiguration.WebService2Soap12);
    }
}