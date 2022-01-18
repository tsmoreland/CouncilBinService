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

using System.ComponentModel.DataAnnotations;

namespace TSMoreland.ArdsBorough.Api.DataTransferObjects.Response;

/// <summary>
/// Details on a Bin Collection Schedule
/// </summary>
/// <param name="BinType">bin type</param>
/// <param name="NextCollection">next collection date</param>
/// <param name="DayOfWeek">collection day</param>
/// <param name="Frequency">how often bin is collected</param>
public sealed record BinCollectionDateDetails(BinType BinType, DateOnly? NextCollection, DayOfWeek? DayOfWeek, CollectionFrequency Frequency)
{
    /// <summary>
    /// Bin Type
    /// </summary>
    /// <example>blue</example>
    [Required]
    public BinType BinType { get; init; } = BinType;

    /// <summary>
    /// Next Collection Date
    /// </summary>
    /// <example>Thu, 12 May 2005 00:00:00 GMT</example>
    public DateOnly? NextCollection { get; init; } = NextCollection;

    /// <summary>
    /// Day of Week
    /// </summary>
    /// <example>monday</example>
    [Required]
    public DayOfWeek? DayOfWeek { get; init; } = DayOfWeek;

    /// <summary>
    /// Collection Frequency
    /// </summary>
    /// <example>everyTwoWeeks</example>
    [Required]
    public CollectionFrequency Frequency { get; init; } = Frequency;
}
