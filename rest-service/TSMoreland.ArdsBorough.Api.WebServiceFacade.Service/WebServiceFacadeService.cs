
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using ArdsBorough.WebService.External;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Service;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Infrastructure;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Shared;

namespace TSMoreland.ArdsBorough.Api.WebServiceFacade.Service;

public sealed class WebServiceFacadeService : IWebServiceFacade
{
    private readonly string _apiSecret;
    private readonly UrpnRepository _urpnRepository;
    private readonly WebService2Soap _soapApi;

    public WebServiceFacadeService(string apiSecret, UrpnRepository urpnRepository, WebService2SoapFactory soapApiFactory)
    {
        if (soapApiFactory == null!)
        {
            throw new ArgumentNullException(nameof(soapApiFactory));
        }

        _apiSecret = apiSecret;
        _urpnRepository = urpnRepository ?? throw new ArgumentNullException(nameof(urpnRepository));
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
            yield return child.Value;
        }
    }
}
