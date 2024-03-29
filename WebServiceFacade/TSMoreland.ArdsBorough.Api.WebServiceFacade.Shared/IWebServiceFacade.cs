﻿//
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

namespace TSMoreland.ArdsBorough.Api.WebServiceFacade.Shared;

/// <summary>
/// Facade around external webservice API
/// </summary>
public interface IWebServiceFacade
{
    /// <summary>
    /// get collection round details for the current date, this will include date details for each of the collected bin type
    /// each as a different round
    /// </summary>
    /// <param name="postcode">address postcode used to get collection details for</param>
    /// <param name="houseNumber">house number which may contributed to address used to get collection details for</param>
    /// <param name="date">the date used to check</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// collection of strings representing the different bin collection details for the given date
    /// </returns>
    IAsyncEnumerable<string> GetRoundsForDate(string postcode, int houseNumber, DateOnly date, CancellationToken cancellationToken);
}
