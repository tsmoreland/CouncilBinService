using System;
using System.Text.Json;
using NUnit.Framework;
using TSMoreland.ArdsBorough.Api.DataTransferObjects.Response;

namespace TSMoreland.ArdsBorough.Api.DataTransferObjects.Test.Response
{

    [TestFixture]
    public sealed class BinCollectionDateDetailsTest
    {

        [Test]
        public void BinCollection_SerializesToExpectedJson_WhenNextCollectionIsNotNull()
        {
            var @object = new BinCollectionDateDetails(BinType.Black, new DateOnly(2005, 05, 12), DayOfWeek.Monday,
                CollectionFrequency.EveryTwoWeeks);

            const string expected =
                @"{""binType"":""black"",""nextCollection"":""Thu, 12 May 2005 00:00:00 GMT"",""dayOfWeek"":""monday"",""frequency"":""everyTwoWeeks""}";

            var actual = JsonSerializer.Serialize(@object, SerializerOptions.Options);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void BinCollection_SerializesToExpectedJson_WhenNextCollectionIsNull()
        {
            var @object = new BinCollectionDateDetails(BinType.Black, null, DayOfWeek.Monday,
                CollectionFrequency.EveryTwoWeeks);

            const string expected =
                @"{""binType"":""black"",""dayOfWeek"":""monday"",""frequency"":""everyTwoWeeks""}";

            var actual = JsonSerializer.Serialize(@object, SerializerOptions.Options);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void BinCollection_DeserializesToExpectedModel_WhenNextCollectionIncluded()
        {
            const string serialized =
                @"{""binType"":""black"",""nextCollection"":""Thu, 12 May 2005 00:00:00 GMT"",""dayOfWeek"":""monday"",""frequency"":""everyTwoWeeks""}";
            var expected = new BinCollectionDateDetails(BinType.Black, new DateOnly(2005, 05, 12), DayOfWeek.Monday,
                CollectionFrequency.EveryTwoWeeks);
            var actual = JsonSerializer.Deserialize<BinCollectionDateDetails>(serialized, SerializerOptions.Options);

            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public void BinCollection_DeserializesToExpectedModel_WhenNextCollectionNotIncluded()
        {
            const string serialized =
                @"{""binType"":""black"",""dayOfWeek"":""monday"",""frequency"":""everyTwoWeeks""}";
            var expected = new BinCollectionDateDetails(BinType.Black, null, DayOfWeek.Monday,
                CollectionFrequency.EveryTwoWeeks);
            var actual = JsonSerializer.Deserialize<BinCollectionDateDetails>(serialized, SerializerOptions.Options);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
