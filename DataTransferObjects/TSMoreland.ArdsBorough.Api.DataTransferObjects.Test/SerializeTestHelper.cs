using System;
using System.Text.Json;
using NUnit.Framework;

namespace TSMoreland.ArdsBorough.Api.DataTransferObjects.Test;

internal static class SerializeTestHelper
{
    public static void SerializesObjectEqaulsExpected(object @object, string expected)
    {
        var actual = JsonSerializer.Serialize(@object, SerializerOptions.Options);
        Assert.That(actual, Is.EqualTo(expected));
    }

    public static void DeserializeReturnsExpectedObject<T>(string serialized, Func<T?, bool> equalityTest)
    {
        var actual = JsonSerializer.Deserialize<T>(serialized, SerializerOptions.Options);

        Assert.That(equalityTest(actual), Is.True);
    }
}
