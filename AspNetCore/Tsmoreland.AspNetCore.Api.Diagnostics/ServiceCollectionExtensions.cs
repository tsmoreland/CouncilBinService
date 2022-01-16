using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tsmoreland.AspNetCore.Api.Diagnostics
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddErrorHandler(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));

            services.AddSingleton<IErrorResponseProvider, ProblemDetailsErrorProvider>();
            services.AddSingleton<IConfigureOptions<ApiBehaviorOptions>, ConfigureApiBehaviorOptions>();

            return services;
        }
    }
}
