//
// Copyright © 2022 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.IO.Compression;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Tsmoreland.AspNetCore.Api.Diagnostics;
using TSMoreland.ArdsBorough.WebApi.Infrastructure;
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
        //app.UseStatusCodePagesWithReExecute("/api/v{version:apiVersion}/error/{0}");
        app.UseExceptionHandler(error => error.UseProblemDetailsError(Environment));
        app.UseMiddleware<EndpointNotFoundMiddleware>();

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
