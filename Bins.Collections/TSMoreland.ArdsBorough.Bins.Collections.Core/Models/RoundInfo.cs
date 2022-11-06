//
// Copyright (c) 2022 Terry Moreland
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

using System.Globalization;
using TSMoreland.ArdsBorough.Bins.Collections.Shared;

namespace TSMoreland.ArdsBorough.Bins.Collections.Core.Models;

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

    public static RoundInfo None { get; } = new(BinType.Unknown, DateOnly.MinValue, TimeSpan.Zero);

    public static RoundInfo ParseOrNone(string source)
    {
        // Mon 11 Oct then every alternate Mon
        (bool splitSuccess, string rawBinType, string rawDate, string rawFrequency) = TrySplit(source);
        if (!splitSuccess)
        {
            return None;
        }

        if (TryParseBinType(rawBinType, out BinType binType) &&
            TryParseDate(rawDate, out DateOnly date) &&
            TryParseFrequency(rawFrequency, out TimeSpan frequency))
        {
            return new RoundInfo(binType, date, frequency);
        }

        return None;

        static (bool Succless, string RawBinType, string RawDate, string RawFrequency) TrySplit(string source)
        {
            if (source is not { Length: > 0 })
            {
                return (false, string.Empty, string.Empty, string.Empty);
            }

            int indexOfColon = source.IndexOf(": ", StringComparison.InvariantCultureIgnoreCase);
            int indexOfThenEvery = source.IndexOf("THEN EVERY ", StringComparison.InvariantCultureIgnoreCase);
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
            if (DateTime.TryParseExact(source, "ddd d MMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                date = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
                return true;
            }

            // 'ddd ' + some level of a remainder
            if (source.Length < 5)
            {
                return false;
            }

            source = source[4..];
            if (DateOnly.TryParseExact(source, "d MMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return true;
            }

            return false;
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
