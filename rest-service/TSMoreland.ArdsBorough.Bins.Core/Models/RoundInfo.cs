using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSMoreland.ArdsBorough.Bins.Shared;

namespace TSMoreland.ArdsBorough.Bins.Core.Models;

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
        // Mon 11 Oct then every alternate Mon
        var (splitSuccess, rawBinType, rawDate, rawFrequency) = TrySplit(source);
        if (!splitSuccess)
        {
            return None;
        }

        if (TryParseBinType(rawBinType, out var binType) &&
            TryParseDate(rawDate, out var date) &&
            TryParseFrequency(rawFrequency, out var frequency))
        {
            return new RoundInfo(binType, date, frequency);
        }

        return None;

        static (bool Succless, string RawBinType, string RawDate, string RawFrequency) TrySplit(string source)
        {
            if (source is not { Length: >0 })
            {
                return (false, string.Empty, string.Empty, string.Empty);
            }

            var indexOfColon = source.IndexOf(": ", StringComparison.InvariantCultureIgnoreCase);
            var indexOfThenEvery = source.IndexOf("THEN EVERY ", StringComparison.InvariantCultureIgnoreCase);
            if (indexOfColon == -1 || indexOfThenEvery == -1)
            {
                return (false, string.Empty, string.Empty, string.Empty);
            }

            return (
                true,
                source[..indexOfColon],
                source[(indexOfColon + 1)..indexOfThenEvery],
                source[(indexOfThenEvery + "THEN EVERY ".Length)..]
            );

        }
        static bool TryParseBinType(string source, out BinType binType)
        {
            binType = BinType.Unknown;

            bool success;
            (success, binType) = source.Trim().ToUpperInvariant() switch 
            {
                "GREY BIN" => (true, BinType.Black),
                "BLUE BIN" => (true, BinType.Blue),
                "GREEN /BROWN BIN" => (true, BinType.Brown),
                "GLASS COLLECTION BOX" => (true, BinType.Glass),
                _ => (false, BinType.Unknown),
            };

            return success;
        }
        static bool TryParseDate(string source, out DateOnly date)
        {
            date = DateOnly.MinValue;

            if (source.Contains("TODAY", StringComparison.InvariantCultureIgnoreCase))
            {
                date = DateOnly.FromDateTime(DateTime.UtcNow);
                return true;
            }

            if (source.Contains("TOMORROW", StringComparison.CurrentCultureIgnoreCase))
            {
                date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
                return true;
            }

            source = source.Trim();
            return DateOnly.TryParseExact(source, "ddd d MMM", out date);
        }
        static bool TryParseFrequency(string source, out TimeSpan frequency)
        {
            frequency = TimeSpan.Zero;
            if (source.Contains("ALTERNATE", StringComparison.InvariantCultureIgnoreCase))
            {
                frequency = TimeSpan.FromDays(14);
                return true;
            }

            if (!source.StartsWith("FOURTH", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            frequency = TimeSpan.FromDays(28);
            return true;
        }

    }
}
