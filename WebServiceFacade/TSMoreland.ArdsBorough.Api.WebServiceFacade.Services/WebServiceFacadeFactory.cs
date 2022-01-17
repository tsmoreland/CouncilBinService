//
// Copyright © 2022 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

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
