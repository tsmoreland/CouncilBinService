using System;
using System.Collections.Generic;
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

        static (bool Succcess, string RawBinType, string RawDate, string RawFrequency) TrySplit(string source)
        {
            var indexOfBinColon = source.IndexOf("BIN: ", StringComparison.InvariantCultureIgnoreCase);
            var indexOfThenEvery = source.IndexOf("THEN EVERY", StringComparison.InvariantCultureIgnoreCase);
            if (indexOfBinColon == -1 || indexOfThenEvery == -1)
            {
                return (false, string.Empty, string.Empty, string.Empty);
            }

            return (
                true,
                source[..indexOfBinColon],
                source[(indexOfBinColon + "BIN: ".Length)..indexOfThenEvery],
                source[(indexOfThenEvery + "THEN EVERY".Length)..]
            );

        }
        static bool TryParseBinType(string source, out BinType binType)
        {
            binType = BinType.Unknown;
            return false;
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

            if (!DateOnly.TryParseExact(source, "ddd dd MMM", out date))
            {
                return false;
            }

            date = new DateOnly(DateTime.Now.Year, date.Month, date.Day);
            return true;

        }
        static bool TryParseFrequency(string source, out TimeSpan frequency)
        {
            frequency = TimeSpan.Zero;
            if (source.Contains("ALTERNATE", StringComparison.InvariantCultureIgnoreCase))
            {
                frequency = TimeSpan.FromDays(14);
                return true;
            }

            if (source.Contains("EVERY FOUR", StringComparison.CurrentCultureIgnoreCase))
            {
                frequency = TimeSpan.FromDays(28);
                return true;
            }

            return false;
        }

    }
}
