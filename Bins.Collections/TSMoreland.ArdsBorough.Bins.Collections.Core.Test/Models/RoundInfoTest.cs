using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSMoreland.ArdsBorough.Bins.Collections.Shared;
using TSMoreland.ArdsBorough.Bins.Core.Models;

namespace TSMoreland.ArdsBorough.Bins.Core.Test.Models;

[TestFixture]
public class RoundInfoTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Parse_ReturnsNone_WhenSourceIsNull()
    {
        RoundInfo actual = RoundInfo.ParseOrNone(null!);
        Assert.That(actual, Is.EqualTo(RoundInfo.None));
    }

    [Test]
    public void Parse_ReturnsNone_WhenSourceIsEmpty()
    {
        RoundInfo actual = RoundInfo.ParseOrNone(string.Empty);
        Assert.That(actual, Is.EqualTo(RoundInfo.None));
    }

    [TestCaseSource(nameof(ValidSerializedRoundInfos))]
    public void Parse_DoesNotThrowException_WhenStringIsExpectedFormat(string data, BinType expectedBinType, DateOnly expectedDate, TimeSpan expectedFrequency)
    {
        Assert.DoesNotThrow(() => _ = RoundInfo.ParseOrNone(data));
    }

    [TestCaseSource(nameof(ValidSerializedRoundInfos))]
    public void Parse_ReturnsExpectedBinType_WhenStringIsExpectedFormat(string data, BinType expectedBinType, DateOnly expectedDate, TimeSpan expectedFrequency)
    {
        (BinType actual, _, _) = RoundInfo.ParseOrNone(data);
        Assert.That(actual, Is.EqualTo(expectedBinType));
    }

    [TestCaseSource(nameof(ValidSerializedRoundInfos))]
    public void Parse_ReturnsExpectedDate_WhenStringIsExpectedFormat(string data, BinType expectedBinType, DateOnly expectedDate, TimeSpan expectedFrequency)
    {
        (_, DateOnly actual, _) = RoundInfo.ParseOrNone(data);
        Assert.That(actual, Is.EqualTo(expectedDate));
    }

    [TestCaseSource(nameof(ValidSerializedRoundInfos))]
    public void Parse_ReturnsExpectedFrequency_WhenStringIsExpectedFormat(string data, BinType expectedBinType, DateOnly expectedDate, TimeSpan expectedFrequency)
    {
        (_, _, TimeSpan actual) = RoundInfo.ParseOrNone(data);
        Assert.That(actual, Is.EqualTo(expectedFrequency));
    }

    public static IEnumerable<TestCaseData> ValidSerializedRoundInfos()
    {
        int year = DateTime.Now.Year;
        yield return new TestCaseData("Grey Bin: Mon 11 Oct then every alternate Mon", BinType.Black, new DateOnly(year, 10, 11), TimeSpan.FromDays(14));
        yield return new TestCaseData("Blue Bin: Mon 18 Oct then every alternate Mon", BinType.Blue, new DateOnly(year, 10, 18), TimeSpan.FromDays(14));
        yield return new TestCaseData("Green /Brown Bin: Mon 18 Oct then every alternate Mon", BinType.Brown, new DateOnly(year, 10, 18), TimeSpan.FromDays(14));
        yield return new TestCaseData("Glass Collection Box: Mon 1 Nov then every fourth Mon", BinType.Glass, new DateOnly(year, 11, 1), TimeSpan.FromDays(28));
    }
}
