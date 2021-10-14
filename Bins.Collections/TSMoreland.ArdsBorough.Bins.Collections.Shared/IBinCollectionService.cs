namespace TSMoreland.ArdsBorough.Bins.Collections.Shared;

public interface IBinCollectionService
{
    /// <summary>
    /// Returns the all bins collection types and date for given address
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
    IAsyncEnumerable<(BinType Type, DateOnly Date)> FindBinCollectionInfoForAddress(int houseNumber, PostCode postCode, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the bins collection types and date for given address for the current week
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
    IAsyncEnumerable<(BinType Type, DateOnly Date)> FindThisWeeksBinCollectionInfoForAddress(int houseNumber, PostCode postCode, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the bins collection types and date for given address for the next week
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
    IAsyncEnumerable<(BinType Type, DateOnly Date)> FindNextWeeksBinCollectionInfoForAddress(int houseNumber, PostCode postCode, CancellationToken cancellationToken);
}
