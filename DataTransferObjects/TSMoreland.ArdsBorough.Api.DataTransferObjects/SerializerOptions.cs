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
using System.Text.Json;
using System.Text.Json.Serialization;
using TSMoreland.ArdsBorough.Api.DataTransferObjects.Converters;

namespace TSMoreland.ArdsBorough.Api.DataTransferObjects;

/// <summary>
/// Serialization options used by the server provided to allow client APIs to match
/// </summary>
public static class SerializerOptions
{
    private static readonly Lazy<JsonSerializerOptions> _options = new(BuildJsonSerializerOptions);
    /// <summary>
    /// Options used by the server 
    /// </summary>
    public static JsonSerializerOptions Options => _options.Value;

    /// <summary>
    /// Constructs default serializer settings, used to construct <see cref="Options"/>
    /// </summary>
    public static JsonSerializerOptions BuildJsonSerializerOptions()
    {
        return new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull

        }.AddDataTransferObjectConverters();
    }

    /// <summary>
    /// updates <paramref name="options"/> converters used by <see cref="Options"/>
    /// </summary>
    public static JsonSerializerOptions AddDataTransferObjectConverters(this JsonSerializerOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (!options.Converters.OfType<JsonStringEnumConverter>().Any())
        {
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            options.Converters.Add(new NullableDateOnlyToRfc1123Converter());
        }

        return options;
    }
}
