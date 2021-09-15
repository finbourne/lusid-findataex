using Lusid.FinDataEx.Data.DataRecord;
using NUnit.Framework;
using System.Collections.Generic;

namespace Lusid.FinDataEx.Tests.Unit.Data.DataRecord
{
    [TestFixture]
    public class DataRecordTests
    {
        private Dictionary<string, string> validData;

        [SetUp]
        public void SetUp()
        {
            validData = new Dictionary<string, string>
            {
                { "field1", "value1" },
                { "field2", "value2" },
                { "field3", "value3" },
            };
        }

        [Test]
        public void TestRawDataRoundTrip()
        {
            var record = (IDataRecord)new InstrumentDataRecord(validData);
            Assert.That(record.RawData, Is.EqualTo(validData));
        }
    }
}
