//
// Copyright © 2022 Terry Moreland
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
