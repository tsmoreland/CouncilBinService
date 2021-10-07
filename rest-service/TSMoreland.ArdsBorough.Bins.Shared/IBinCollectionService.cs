using System;
using System.Collections.Generic;
using System.Threading;

namespace TSMoreland.ArdsBorough.Bins.Shared;

public interface IBinCollectionService
{
    /// <summary>
    /// Returns the next bins collected and day they're collected on 
    /// </summary>
    /// <param name="houseNumber">house or building number used to look up address</param>
    /// <param name="postCode">postcode used to look up address</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// asynchronous collection of bins collection info for the next collection
    /// </returns>
    /// <exception cref="AddressNotFoundException">
    /// if address is invalid or cannot be found
    /// </exception>
    IAsyncEnumerable<(BinType Type, DateOnly Date)> FindNextBinCollectionInfoForAddress(int houseNumber, PostCode postCode, CancellationToken cancellationToken);
}
