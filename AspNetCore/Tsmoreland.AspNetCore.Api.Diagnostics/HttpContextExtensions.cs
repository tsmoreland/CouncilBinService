using System;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;

namespace Tsmoreland.AspNetCore.Api.Diagnostics;

public static class HttpContextExtensions
{
    public static string GetProblemResponseTypeFromAccept(this HttpContext context)
    {
        var acceptTypes = context.Request.Headers.Accept;

        bool json = false;
        bool xml = false;

        foreach (var acceptType in acceptTypes)
        {
            if (!json && string.Equals(acceptType, MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase))
            {
                json = true;
            }
            if (!json && string.Equals(acceptType, MediaTypeNames.Application.Xml, StringComparison.OrdinalIgnoreCase))
            {
                xml = true;
            }

            if (json && xml)
            {
                break;
            }
        }

        if (xml && !json)
        {
            return "application/problem+xml";
        }
        else
        {
            return "application/problem+json";
        }
    }
}