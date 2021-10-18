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
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .Build();

var services = new ServiceCollection();

try
{

    var council = config["council"] ?? string.Empty;
    var urpn = int.Parse(config["URPN"]);
    var pw = config["PW"];

    if (council is not { Length: > 0 } || pw is not { Length: >0 })
    {
        throw new Exception("Invalid settings, unable to proceed");
    }

    /*
    WebService2Soap service = new WebService2SoapClient(WebService2SoapClient.EndpointConfiguration.WebService2Soap12);

    var date = DateTime.Now;
    if (date.DayOfWeek != DayOfWeek.Sunday)
    {
        date = date.Subtract(TimeSpan.FromDays((int)date.DayOfWeek));
    }

    var formattedDate = date.ToString("dd/MM/yyyy");
    var rootNode = await service.getRoundInfoForUPRNNewAsync(council, urpn.ToString(), pw, formattedDate);
    if (rootNode is null)
    {
        Console.WriteLine("Request failed or no response");
        return;
    }

    var document = XDocument.Parse(rootNode.InnerXml);
    var row = document.Descendants("Row").FirstOrDefault();
    if (row is null)
    {
        Console.WriteLine("Missing 'Row' element");
        return;
    }

    foreach (var child in row.Descendants())
    {
        Console.WriteLine($"{child.Name}: {child.Value}");
    }
    */


}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

