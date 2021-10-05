using NUnit.Framework;
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
    public void Parse_ReturnsNone_WhenSourceIsEmpty()
    {
        RoundInfo actual = RoundInfo.ParseOrNone(string.Empty);
        Assert.That(actual, Is.EqualTo(RoundInfo.None));
    }
}