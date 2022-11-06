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

using Microsoft.Extensions.Configuration;

namespace TSMoreland.ArdsBorough.Api.WebServiceFacade.Services;

public class UrpnRepository
{
    private readonly IConfiguration _configuration;

    public UrpnRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<(string Council, string URPN)> GetCouncilAndURPNFromAddressAsync(int houseNumber, string postcode, CancellationToken cancellationToken)
    {
        if (ValidateParametersOrThrow() is { } exception)
        {
            return Task.FromException<(string Council, string URPN)>(exception);
        }

        var council = _configuration["test-data:council"] ?? string.Empty;
        var urpn = _configuration["test-data:urpn"] ?? string.Empty;

        return Task.FromResult<(string Council, string URPN)>((council, urpn));

        Exception? ValidateParametersOrThrow()
        {
            if (houseNumber <= 0)
            {
                return new ArgumentOutOfRangeException(nameof(houseNumber));
            }

            if (postcode is not { Length: > 0 })
            {
                return new ArgumentException("invalid postcode", nameof(postcode));
            }

            return null;
        }
    }
}
