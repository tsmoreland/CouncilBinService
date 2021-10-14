using System.Text.RegularExpressions;

namespace TSMoreland.ArdsBorough.Bins.Collections.Shared;

public readonly record struct PostCode(string Value) : IEquatable<PostCode>
{
    private static readonly Regex _postcode = new (@"([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})");

    public static PostCode ConvertOrNone(string value)
    {
        return Validate(value)
            ? new PostCode(value)
            : None;
    }
    public static PostCode ConvertOrThrow(string value)
    {
        return Validate(value)
            ? new PostCode(value)
            : throw new ArgumentException("invalid postcode", nameof(value));
    }

    public bool IsValid => Validate(Value);

    public static PostCode None { get; } = new ();

    private static bool Validate(string postCode)
    {
        return postCode is { Length: >0 } && _postcode.IsMatch(postCode);
    }

    public override string ToString()
    {
        return Value;
    }
}
