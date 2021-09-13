using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TSMoreland.ArdsBorough.Api.DataTransferObjects.Converters;

/// <summary>
/// <see cref="DateOnly"/> to RFC1123 Converter
/// </summary>
public sealed class NullableDateOnlyToRfc1123Converter : JsonConverter<DateOnly?>
{
    /// <inheritdoc/>
    public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("expected string");
        }

        //ddd, dd MMM yyyy HH':'mm':'ss 'GMT'
        var value = reader.GetString() ?? string.Empty;

        if (!DateTime.TryParseExact(value, "r", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dateValue))
        {
            throw new JsonException("invalid date format");
        }

        dateValue = dateValue.ToUniversalTime();

        return new DateOnly(dateValue.Year, dateValue.Month, dateValue.Day);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToDateTime(new TimeOnly()).ToString("r"));
    }
}
