using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TSMoreland.ArdsBorough.WebApi.Infrastructure;
using Tsmoreland.AspNetCore.Api.Diagnostics;
using TSMoreland.WebApi.Middleware;
using TSMoreland.WebApi.Middleware.SwaggerFilters;

namespace TSMoreland.ArdsBorough.WebApi.App;

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
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        services.AddResponseCompression(options =>
        {
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.EnableForHttps = true;
        });
        services.Configure<BrotliCompressionProviderOptions>(options => 
        {
            options.Level = CompressionLevel.Optimal;
        });
        services.AddMvcCore(options =>
        {
            options.Filters.Add(new ValidateModelStateActionFilter());
        });
        services.AddHttpContextAccessor();
        services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'V");
        services.AddApiVersioning(options => options.ApiVersionReader = new UrlSegmentApiVersionReader());

        services.AddSwaggerGenWithVersioning((options, provider) =>
        {
            foreach (ApiVersionDescription version in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(version.GroupName, 
                    new OpenApiInfo
                    {
                        Title = $"ArdsBorough Api {version.GroupName}",
                        Version = version.ApiVersion.ToString()
                    });
            }

            options.DocInclusionPredicate((doc, descriptor) =>
            {
                return descriptor
                    .ActionDescriptor
                    .GetApiVersionModel(ApiVersionMapping.Explicit)
                    .ImplementedApiVersions
                    .Any(v => $"v{v}" == doc);
            });
            options.OperationFilter<DefaultReturnValuesOperationFilter>();
            options.OperationFilter<RemoveVersionParameterOpertationFilter>();
            options.DocumentFilter<ApplyApiVersionDocumentFilter>();

            List<string> xmlFiles = new();
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly is not null)
            {
                xmlFiles.Add($"{entryAssembly.GetName().Name}.xml");
            }
            xmlFiles.Add($"{typeof(Api.DataTransferObjects.BinType).Assembly.GetName().Name}.xml");

            var existingFiles = xmlFiles
                .Select(file => Path.Combine(AppContext.BaseDirectory, file))
                .Where(File.Exists);
            foreach (var file in existingFiles)
            {
                options.IncludeXmlComments(file);
            }
        });

        services.AddScoped<DefaultReturnValuesOperationFilter>();
        services.AddScoped<RemoveVersionParameterOpertationFilter>();
        services.AddScoped<ApplyApiVersionDocumentFilter>();

        services.AddErrorHandler();
        services.AddWebApiInfrastructure();

        // needed if/when controllers come from another library
        services
            .AddControllers()
            .AddApplicationPart(typeof(Startup).Assembly)
            .AddControllersAsServices();
    }

    /// <summary/>
    public void Configure(IApplicationBuilder app)
    {
        app.UseCorrelationId();

        // not working with versioning, may need custom middleware to accomplish this
        app.UseStatusCodePagesWithReExecute("/api/v{version:apiVersion}/error/{0}");
        app.UseExceptionHandler(error => error.UseProblemDetailsError(Environment));

        if (!Environment.IsDevelopment())
        {
            app.UseHsts();
        }


        app.UseResponseCompression();

        app.UseSwagger(options =>
        {
            // explciit set of what the default should already be
            options.RouteTemplate = "api/{documentName}/swagger.{json|yaml}";

            options.PreSerializeFilters.Add((doc, request) =>
            {
                doc.Servers ??= new List<OpenApiServer>();
                doc.Servers.Add(new OpenApiServer
                {
                    Url = $"{request.Scheme}://{request.Host.Value}/api/v{doc.Info.Version}"
                });
            });
        });
        app.UseSwaggerUI(options =>
        {
            var apiVerionDescriptorProvider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (string versionGroupName in apiVerionDescriptorProvider.ApiVersionDescriptions.Select(description => description.GroupName))
            {
                options.SwaggerEndpoint($"/api/{versionGroupName}/swagger.json", versionGroupName.ToUpperInvariant());
            }
        });


        app.UseRouting();

        app.UseAuthorization();
        app.UseAuthentication();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
