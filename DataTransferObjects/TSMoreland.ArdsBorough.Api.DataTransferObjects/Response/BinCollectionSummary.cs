using System;

namespace TSMoreland.ArdsBorough.Api.DataTransferObjects.Response;

/// <summary>
/// Summary showing the next collection date for a bin type
/// </summary>
/// <param name="Type">Bin Collection Type</param>
/// <param name="Date">Bin Collection Date</param>
public sealed record BinCollectionSummary(BinType Type, DateTime Date)
{
    /// <summary>
    /// Bin Collection Type
    /// </summary>
    public BinType Type { get; init; } = Type;

    /// <summary>
    /// Bin Collection Date
    /// </summary>
    public DateTime Date { get; init; } = Date;

}
