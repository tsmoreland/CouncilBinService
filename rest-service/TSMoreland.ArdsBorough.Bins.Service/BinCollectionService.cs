using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Configuration;
using TSMoreland.ArdsBorough.Bins.Core.Models;
using TSMoreland.ArdsBorough.Bins.Shared;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Shared;

namespace TSMoreland.ArdsBorough.Service;

public sealed class BinCollectionService : IBinCollectionService
{
    private readonly IWebServiceFacade _webServiceFacade;

    public BinCollectionService(
        IWebServiceFacadeFactory webServiceFacadeFactory,
        IConfiguration configuration
        )
    {
        var secret = configuration["test-data:secret"] ?? string.Empty;
        _webServiceFacade = webServiceFacadeFactory.Build(secret);
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<(BinType Type, DayOfWeek Day)> FindNextBinCollectionInfoForAddress(int houseNumber, PostCode postCode, CancellationToken cancellationToken)
    {
        return _webServiceFacade
            .GetRoundsForDate(postCode.Value, houseNumber, DateOnly.FromDateTime(DateTime.Now), cancellationToken)
            .Select(RoundInfo.ParseOrNone)
            .Select(info => (info.Type, info.Collection.DayOfWeek))
            .AsAsyncEnumerable();
    }
}
