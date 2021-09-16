using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using TSMoreland.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers(options =>
    {
        options.OutputFormatters.RemoveType<StringOutputFormatter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });


var app = builder.Build();

app.UseCorrelationId();
app.UseExceptionHandler();
app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapRazorPages();

app.Run();
