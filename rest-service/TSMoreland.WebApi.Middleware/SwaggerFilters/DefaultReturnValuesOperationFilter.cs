using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TSMoreland.WebApi.Middleware.SwaggerFilters
{
    public sealed class DefaultReturnValuesOperationFilter : IOperationFilter
    {
        /// <inheritdoc/>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // TODO: add 500, with problem details
            // TODO: check to see if we can identify id query/post parameters and if present add 400
        }
    }
}
