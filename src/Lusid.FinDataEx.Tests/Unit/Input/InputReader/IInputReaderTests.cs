using Lusid.FinDataEx.Data.CorporateActionRecord;
using Lusid.FinDataEx.Data.DataRecord;
using Lusid.FinDataEx.Input;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static Lusid.FinDataEx.Input.IInputReader;

namespace Lusid.FinDataEx.Tests.Unit.Input.InputReader
{
    [TestFixture]
    public class IInputReaderTests : IInputReader
    {
        [Test]
        public void InstrumentDataAreMapped()
        {
            var dataRecord = new Dictionary<string, string>
            {
                { "field", "value" }
            };

            var options = new GetDataOptions();

            var outputRecord = ConvertToRecord(dataRecord, options, "dummy string");

            Assert.That(outputRecord, Is.TypeOf<InstrumentDataRecord>());
        }

        [Test]
        public void CashDividendActionsAreMapped()
        {
            var dataRecord = new Dictionary<string, string>
            {
                { "field", "Cash Dividend" }
            };

            var options = new GetActionsOptions();

            var outputRecord = ConvertToRecord(dataRecord, options, "field");

            Assert.That(outputRecord, Is.TypeOf<CashDividendCorporateActionRecord>());
        }

        [Test]
        public void ThrowOnUnknownActionTypeToMap()
        {
            var dataRecord = new Dictionary<string, string>
            {
                { "field", "Stock Split" }
            };

            var options = new GetActionsOptions();

            Assert.Throws<NotImplementedException>(() => ConvertToRecord(dataRecord, options, "field"));
        }

        [Test]
        public void ThrowOnActionFieldMissing()
        {
            var dataRecord = new Dictionary<string, string>
            {
                { "different field", "Stock Split" }
            };

            var options = new GetActionsOptions();

            Assert.Throws<KeyNotFoundException>(() => ConvertToRecord(dataRecord, options, "field"));
        }

        // test column is obeyed for corp actions (fails on bad column)

        // This only exists to allow calling the protected static method
        public DataLicenseOutput Read()
        {
            throw new NotImplementedException();
        }
    }
}