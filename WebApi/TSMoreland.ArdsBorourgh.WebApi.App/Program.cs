using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TSMoreland.ArdsBorough.WebApi.App;

var appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
if (appPath is { Length: >0 })
{
    Directory.SetCurrentDirectory(appPath);
}

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .AddUserSecrets(typeof(Program).Assembly, optional: true)
    .Build();


Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
        webBuilder.UseKestrel();
    })
    .ConfigureLogging(loggingBuilder =>
        loggingBuilder
            .AddConfiguration(config)
            .AddConsole()
            .AddDebug())
    .Build()
    .Run();
