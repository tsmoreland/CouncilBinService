
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using ArdsBorough.WebService.External;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Service;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Shared;

namespace TSMoreland.ArdsBorough.Api.WebServiceFacade.Service;

public sealed class WebServiceFacadeService : IWebServiceFacade
{
    private readonly string _apiSecret;
    private readonly UrpnRepository _urpnRepository;
    private readonly WebService2Soap _soapApi;

    public WebServiceFacadeService(string apiSecret, UrpnRepository urpnRepository, WebService2Soap soapApi)
    {
        _apiSecret = apiSecret;
        _urpnRepository = urpnRepository ?? throw new ArgumentNullException(nameof(urpnRepository));
        _soapApi = soapApi ?? throw new ArgumentNullException(nameof(soapApi));
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<RoundInfo> GetRoundsForDate(string postcode, int houseNumber, DateOnly date, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var (council, urpn) = await _urpnRepository.GetCouncilAndURPNFromAddressAsync(houseNumber, postcode, cancellationToken);

        var xmlNode = await _soapApi.getRoundInfoForUPRNNewAsync(council, urpn, _apiSecret, date.ToString("dd/MM/yyyy"));
        if (xmlNode == null)
        {
            // throw or at the very least log an error
            yield break;
        }

        // sample output: Grey Bin: Mon 27 Sep then every alternate Mon 
        // pass to RoundInfo domain object (different name?) and let that handle the parsing, even if it's just a static functional method,
        // putting it in core highlights the primary test area



        yield break;
    }
}
