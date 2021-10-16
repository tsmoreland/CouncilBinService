using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TSMoreland.WebApi.Middleware.SwaggerFilters;

public sealed class RemoveVersionParameterOpertationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var parameter = operation.Parameters.SingleOrDefault(parameter => parameter.Name == "version");
        if (parameter is null)
        {
            return;
        }

        operation.Parameters.Remove(parameter);
    }
}
