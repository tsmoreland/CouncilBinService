using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Tsmoreland.AspNetCore.Api.Diagnostics
{
    public sealed class ConfigureErrorHandlerOptions : IConfigureOptions<ErrorHandlerOptions>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConfigureErrorHandlerOptions(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }


        public void Configure(ErrorHandlerOptions options)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is not null)
            {
                options.TraceIdentifier = context.TraceIdentifier;
            }

        }
    }
}
