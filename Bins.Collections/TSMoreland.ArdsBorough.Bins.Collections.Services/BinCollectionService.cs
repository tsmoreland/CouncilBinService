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

using Microsoft.Extensions.Configuration;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Shared;
using TSMoreland.ArdsBorough.Bins.Collections.Core.Models;
using TSMoreland.ArdsBorough.Bins.Collections.Shared;

namespace TSMoreland.ArdsBorough.Bins.Collections.Services;

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
        int dayNumber = (int)now.DayOfWeek;

        var weekStart = now.AddDays((int)DayOfWeek.Sunday - dayNumber);
        var weekEnd = now.AddDays((int)DayOfWeek.Saturday - dayNumber);

        return FindBinCollectionInfoForAddressWithDateRange(houseNumber, postCode, weekStart, weekEnd, cancellationToken);
    }

    public IAsyncEnumerable<(BinType Type, DateOnly Date)> FindNextWeeksBinCollectionInfoForAddress(int houseNumber, PostCode postCode, CancellationToken cancellationToken)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        int dayNumber = (int)now.DayOfWeek;

        var weekStart = now.AddDays((int)DayOfWeek.Sunday - dayNumber + 7);
        var weekEnd = now.AddDays((int)DayOfWeek.Saturday - dayNumber + 7);

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
