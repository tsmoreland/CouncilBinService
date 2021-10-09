using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TSMoreland.WebApi.Middleware;
using TSMoreland.ArdsBorough.Api.Infrastructure;

namespace TSMoreland.ArdsBorough.Api.App;

/// <summary/>
public class Startup
{
    /// <summary/>
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }
    /// <inheritdoc cref="IConfiguration"/>
    public IConfiguration Configuration { get; }
    /// <inheritdoc cref="IWebHostEnvironment"/>
    public IWebHostEnvironment Environment { get; }

    /// <summary/>
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                options.OutputFormatters.RemoveType<StringOutputFormatter>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressConsumesConstraintForFormFileParameters = true;
                options.SuppressInferBindingSourcesForParameters = true;
                options.SuppressModelStateInvalidFilter = true;
                options.SuppressMapClientErrors = true;
                options.ClientErrorMapping[StatusCodes.Status404NotFound].Link = "https://httpstatuses.com/404";
                options.InvalidModelStateResponseFactory = context =>
                {
                    var result = new BadRequestObjectResult(context.ModelState);
                    foreach (var mimeType in new[] { MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml })
                    {
                        result.ContentTypes.Add(mimeType);
                    }

                    return result;
                };

            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddHttpContextAccessor();
        services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'V");
        services.AddApiVersioning(options => options.ApiVersionReader = new UrlSegmentApiVersionReader());

        services.AddSwaggerGenWithVersioning((options, provider) =>
        {
            foreach (var version in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(version.GroupName, 
                    new OpenApiInfo
                    {
                        Title = $"ArdsBorough Api {version.GroupName}",
                        Version = version.ApiVersion.ToString()
                    });
            }
        });

        services.AddApiInfrastructure();
    }

    /// <summary/>
    public void Configure(IApplicationBuilder app)
    {
        app.UseCorrelationId();

        app.UseExceptionHandler(Environment.IsDevelopment() 
            ? "/api/error-dev" 
            : "/api/error");

        app.UseRouting();

        app.UseAuthorization();
        app.UseAuthentication();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
