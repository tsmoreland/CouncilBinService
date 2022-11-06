//
// Copyright (c) 2022 Terry Moreland
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

using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using ArdsBorough.WebService.External;
using Microsoft.Extensions.Logging;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Infrastructure;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Shared;

namespace TSMoreland.ArdsBorough.Api.WebServiceFacade.Services;

public sealed class WebServiceFacadeService : IWebServiceFacade
{
    private readonly string _apiSecret;
    private readonly UrpnRepository _urpnRepository;
    private readonly ILogger<WebServiceFacadeService> _logger;
    private readonly WebService2Soap _soapApi;

    public WebServiceFacadeService(string apiSecret, UrpnRepository urpnRepository, WebService2SoapFactory soapApiFactory, ILogger<WebServiceFacadeService> logger)
    {
        if (soapApiFactory == null!)
        {
            throw new ArgumentNullException(nameof(soapApiFactory));
        }

        _apiSecret = apiSecret;
        _urpnRepository = urpnRepository ?? throw new ArgumentNullException(nameof(urpnRepository));
        _logger = logger;
        _soapApi = soapApiFactory.Build();
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<string> GetRoundsForDate(string postcode, int houseNumber, DateOnly date, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var (council, urpn) = await _urpnRepository.GetCouncilAndURPNFromAddressAsync(houseNumber, postcode, cancellationToken);

        XmlNode? xmlNode;
        try
        {
            xmlNode = await _soapApi.getRoundInfoForUPRNNewAsync(council, urpn, _apiSecret, date.ToString("dd/MM/yyyy"));
            if (xmlNode == null)
            {
                // throw or at the very least log an error
                yield break;
            }
        }
        catch (Exception)
        {
            throw;
        }

        /*
        foreach (var node in xmlNode.ChildNodes)
        {
            // ... to do ..
        }
        */

        var document = XDocument.Parse(xmlNode.InnerXml);
        var row = document.Descendants("Row").FirstOrDefault();
        if (row is null)
        {
            yield break;
        }

        foreach (var child in row.Descendants())
        {
            // format: Grey Bin: Mon 27 Sep then every alternate Mon 
            _logger.LogInformation("{ExternalRoundInfo}", child.Value.Replace('\r', '_').Replace('\n', '_'));
            yield return child.Value;
        }
    }
}
