//
// Copyright © 2021 Terry Moreland
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

using System;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TSMoreland.ArdsBorough.Bins.Collections.Shared;
using TSMoreland.ArdsBorough.Infrastructure;

var config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .Build();

try
{
    var services = new ServiceCollection();
    services.AddSingleton<IConfiguration>(config);
    services.AddArdsBoroughInfrastructure();
    services.AddLogging(builder =>
    {
        builder.AddConfiguration(config.GetSection("Logging"));
        if (!OperatingSystem.IsBrowser())
        {
            builder.AddConsole();
        }
        builder.AddDebug();
    });

    var provider = services.BuildServiceProvider();

    var service = provider.GetRequiredService<IBinCollectionService>();

    var enumerable = service.FindBinCollectionInfoForAddress(1, new PostCode("SW1A1AA"), CancellationToken.None);
    await foreach (var (type, date) in enumerable)
    {
        Console.WriteLine($"{type} on {date}");
    }


}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

