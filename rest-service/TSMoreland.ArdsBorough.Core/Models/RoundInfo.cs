using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSMoreland.ArdsBorough.Shared;

namespace TSMoreland.ArdsBorough.Core.Models;

public sealed record RoundInfo(BinType Type, DateOnly Collection, TimeSpan Frequency)
{

    /// <summary>
    /// Bin Type 
    /// </summary>
    public BinType Type { get; init; } = Type;

    /// <summary>
    /// Next Collection Date
    /// </summary>
    public DateOnly Collection { get; init; } = Collection;

    /// <summary>
    /// How often the bin is collected
    /// </summary>
    public TimeSpan Fequency { get; init; } = Frequency;

    public static RoundInfo None { get; } = new (BinType.Unknown, DateOnly.MinValue, TimeSpan.Zero);

    public static RoundInfo ParseOrNone(string source)
    {
        return None;
    }
}
