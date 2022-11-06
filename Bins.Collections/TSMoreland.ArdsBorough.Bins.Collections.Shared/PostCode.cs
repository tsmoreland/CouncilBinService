//
// Copyright (c) 2022 Terry Moreland
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

using System.Text.RegularExpressions;

namespace TSMoreland.ArdsBorough.Bins.Collections.Shared;

public readonly record struct PostCode(string Value) : IEquatable<PostCode>
{
    private static readonly Regex _postcode = new(@"([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})");

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

    public static PostCode None { get; } = new();

    private static bool Validate(string postCode)
    {
        return postCode is { Length: > 0 } && _postcode.IsMatch(postCode);
    }

    public override string ToString()
    {
        return Value;
    }
}
