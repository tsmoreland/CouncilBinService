using Microsoft.AspNetCore.Builder;
using TSMoreland.WebApi.Middleware.Implementation;

namespace TSMoreland.WebApi.Middleware;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the correction id middleware to the application request pipeline
    /// </summary>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }

}
