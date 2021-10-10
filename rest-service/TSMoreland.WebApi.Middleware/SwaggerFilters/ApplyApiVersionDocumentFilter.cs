using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TSMoreland.WebApi.Middleware.SwaggerFilters;

public sealed class ApplyApiVersionDocumentFilter : IDocumentFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {

        var pathValues = swaggerDoc
            .Paths
            .Select(path =>
                new
                {
                    Key = path.Key.Replace("/api/v{version}", $"/api/{context.DocumentName}"),
                    path.Value
                });
        var paths = new OpenApiPaths();
        foreach (var pair in pathValues)
        {
            var (key, value) = (pair.Key, pair.Value);
            paths.Add(key, value);
        }
        swaggerDoc.Paths = paths;
    }
}
