using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tsmoreland.AspNetCore.Api.Diagnostics
{
    public static class ApplicationBuilderExtensions 
    {
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder app, Action<ErrorHandlerOptions>? setupAction = null)
        {
            ArgumentNullException.ThrowIfNull(app, nameof(app));

            ErrorHandlerOptions options;
            using (var scope = app.ApplicationServices.CreateScope())
            {
                options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<ErrorHandlerOptions>>().Value;
                setupAction?.Invoke(options);
            }

            return app.UseMiddleware<ErrorHandlerMiddleware>(options);

        }
    }
}
