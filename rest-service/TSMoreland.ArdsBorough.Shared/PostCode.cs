using System;
using System.Text.RegularExpressions;

namespace TSMoreland.ArdsBorough.Shared;

public readonly record struct PostCode : IEquatable<PostCode>
{
    private static readonly Regex _postcode = new (@"([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})");

    public PostCode(string value)
    {
        if (!IsValid(value))
        {
            throw new ArgumentException("Invalid postcode", nameof(value));
        }

        Value = value;
    }

    public string Value { get; init; } 

    public void Decontruct(out string value)
    {
        value = Value;
    }

    private static bool IsValid(string postCode)
    {
        return postCode is { Length: >0 } && _postcode.IsMatch(postCode);
    }

    public override string ToString()
    {
        return Value;
    }
}
