using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace TSMoreland.ArdsBorough.Api.WebServiceFacade.Service
{
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

                if (postcode is not { Length: >0 })
                {
                    return new ArgumentException("invalid postcode", nameof(postcode));
                }

                return null;
            }
        }
    }
}
