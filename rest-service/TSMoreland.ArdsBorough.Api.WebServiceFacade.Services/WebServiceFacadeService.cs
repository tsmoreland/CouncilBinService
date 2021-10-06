
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using ArdsBorough.WebService.External;
using Microsoft.Extensions.Logging;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Services;
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
