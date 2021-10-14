using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using Microsoft.Extensions.Configuration;
using TSMoreland.ArdsBorough.Bins.Core.Models;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Shared;
using TSMoreland.ArdsBorough.Bins.Collections.Shared;

namespace TSMoreland.ArdsBorough.Bins.Services;

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
    public IAsyncEnumerable<(BinType Type, DateOnly Date)> FindBinCollectionInfoForAddress(int houseNumber, PostCode postCode, CancellationToken cancellationToken)
    {
        return _webServiceFacade
            .GetRoundsForDate(postCode.Value, houseNumber, DateOnly.FromDateTime(DateTime.Now), cancellationToken)
            .Select(RoundInfo.ParseOrNone)
            .Where(info => info != RoundInfo.None)
            .OrderBy(info => info.Collection)
            .Select(info => (info.Type, info.Collection))
            .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<(BinType Type, DateOnly Date)> FindThisWeeksBinCollectionInfoForAddress(int houseNumber, PostCode postCode,
        CancellationToken cancellationToken)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        int dayNumber = (int) now.DayOfWeek;

        var weekStart = now.AddDays((int) DayOfWeek.Sunday - dayNumber);
        var weekEnd = now.AddDays((int) DayOfWeek.Saturday - dayNumber);

        return FindBinCollectionInfoForAddressWithDateRange(houseNumber, postCode, weekStart, weekEnd, cancellationToken);
    }

    public IAsyncEnumerable<(BinType Type, DateOnly Date)> FindNextWeeksBinCollectionInfoForAddress(int houseNumber, PostCode postCode, CancellationToken cancellationToken)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        int dayNumber = (int) now.DayOfWeek;

        var weekStart = now.AddDays((int) DayOfWeek.Sunday - dayNumber + 7);
        var weekEnd = now.AddDays((int) DayOfWeek.Saturday - dayNumber + 7);

        return FindBinCollectionInfoForAddressWithDateRange(houseNumber, postCode, weekStart, weekEnd, cancellationToken);
    }

    private IAsyncEnumerable<(BinType Type, DateOnly Date)> FindBinCollectionInfoForAddressWithDateRange(int houseNumber, PostCode postCode, DateOnly weekStart, DateOnly weekEnd,
        CancellationToken cancellationToken)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);

        return _webServiceFacade
            .GetRoundsForDate(postCode.Value, houseNumber, DateOnly.FromDateTime(DateTime.Now), cancellationToken)
            .Select(RoundInfo.ParseOrNone)
            .Where(info => info != RoundInfo.None)
            .Where(info => info.Collection >= weekStart && info.Collection <= weekEnd)
            .OrderBy(info => info.Collection)
            .Select(info => (info.Type, info.Collection))
            .AsAsyncEnumerable();
    }
}
