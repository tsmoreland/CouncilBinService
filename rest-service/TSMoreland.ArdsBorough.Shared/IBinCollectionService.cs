using System;
using System.Collections.Generic;

namespace TSMoreland.ArdsBorough.Shared;

public interface IBinCollectionService
{
    /// <summary>
    /// Returns the next bins collected and day they're collected on 
    /// </summary>
    /// <param name="hosueNumber"></param>
    /// <param name="postCode"></param>
    /// <returns>
    /// collection of bins collection info for the next collection
    /// </returns>
    /// <exception cref="AddressNotFoundException">
    /// if address is invalid or cannot be found
    /// </exception>
    IEnumerable<(BinType Type, DayOfWeek Day)> FindNextBinCollectionInfoForAddress(int hosueNumber, PostCode postCode);
}
