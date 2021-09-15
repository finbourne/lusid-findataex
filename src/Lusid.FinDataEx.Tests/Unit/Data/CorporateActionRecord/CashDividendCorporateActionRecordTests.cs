using Lusid.FinDataEx.Data.CorporateActionRecord;
using Lusid.Sdk.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lusid.FinDataEx.Tests.Unit.Data.CorporateActionRecord
{
    [TestFixture]
    public class CashDividendCorporateActionRecordTests
    {
        private Dictionary<string, string> validData;
        private Dictionary<string, string> missingData;
        private Dictionary<string, string> invalidData;

        [SetUp]
        public void SetUp()
        {
            validData = new Dictionary<string, string>
            {
                { "0-Action Type", "Cash Dividend" },
                { "2-Announce/Declared Date", "2000/01/01" },
                { "3-Effective Date", "2000/01/02" },
                { "8-Summary", "Gross Amount: 123" },
                { "9-Summary", " Currency: USD" },
                { "11-Summary", "2000/01/03" },
                { "12-Summary", "2000/01/04" },
                { "13-tad_id", "UNIQUE.ID.FOR.SECURITY" }
            };

            missingData = new Dictionary<string, string>
            {
                { "x0-Action Type", "Cash Dividend" },
                { "x2-Announce/Declared Date", "2000/01/01" },
                { "x3-Effective Date", "2000/01/02" },
                { "x8-Summary", "Gross Amount: 123" },
                { "x9-Summary", " Currency: USD" },
                { "x11-Summary", "2000/01/03" },
                { "x12-Summary", "2000/01/04" },
                { "x13-tad_id", "UNIQUE.ID.FOR.SECURITY" }
            };

            invalidData = new Dictionary<string, string>
            {
                { "0-Action Type", "Cash Dividend" },
                { "2-Announce/Declared Date", "notadate" },
                { "3-Effective Date", "notadate" },
                { "8-Summary", "badlyformattedamount" },
                { "9-Summary", "badlyformattedcurrency" },
                { "11-Summary", "notadate" },
                { "12-Summary", "notadate" },
                { "13-tad_id", "UNIQUE.ID.FOR.SECURITY" }
            };
        }

        [Test]
        public void TestRawDataRoundTrip()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(validData);
            Assert.That(record.RawData, Is.EqualTo(validData));
        }

        [Test]
        public void ValidCorporateActionCode()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(validData);
            Assert.That(record.GetActionCode("sourceId", "requestId"), Is.EqualTo("sourceId-requestId"));
        }

        [Test]
        public void ValidDescription()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(validData);
            Assert.That(record.GetDescription(), Is.EqualTo("Cash Dividend"));
        }

        [Test]
        public void ThrowOnMissingDescription()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(missingData);
            Assert.Throws<KeyNotFoundException>(() => record.GetDescription());
        }

        [Test]
        public void ValidAnnouncementDate()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(validData);
            Assert.That(record.GetAnnouncementDate(), Is.EqualTo(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero)));
        }

        [Test]
        public void ThrowOnMissingAnnouncementDate()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(missingData);
            Assert.Throws<KeyNotFoundException>(() => record.GetAnnouncementDate());
        }

        [Test]
        public void ThrowOnInvalidAnnouncementDate()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(invalidData);
            Assert.Throws<FormatException>(() => record.GetAnnouncementDate());
        }

        [Test]
        public void ValidExecutionDate()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(validData);
            Assert.That(record.GetExecutionDate(), Is.EqualTo(new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.Zero)));
        }

        [Test]
        public void ThrowOnMissingExecutionDate()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(missingData);
            Assert.Throws<KeyNotFoundException>(() => record.GetExecutionDate());
        }


        [Test]
        public void ThrowOnInvalidExecutionDate()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(invalidData);
            Assert.Throws<FormatException>(() => record.GetPaymentDate());
        }

        [Test]
        public void ValidRecordDate()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(validData);
            Assert.That(record.GetRecordDate(), Is.EqualTo(new DateTimeOffset(2000, 1, 3, 0, 0, 0, TimeSpan.Zero)));
        }

        [Test]
        public void ThrowOnMissingRecordDate()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(missingData);
            Assert.Throws<KeyNotFoundException>(() => record.GetRecordDate());
        }

        [Test]
        public void ThrowOnInvalidRecordDate()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(invalidData);
            Assert.Throws<FormatException>(() => record.GetRecordDate());
        }

        [Test]
        public void ValidPaymentDate()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(validData);
            Assert.That(record.GetPaymentDate(), Is.EqualTo(new DateTimeOffset(2000, 1, 4, 0, 0, 0, TimeSpan.Zero)));
        }

        [Test]
        public void ThrowOnMissingPaymentDate()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(missingData);
            Assert.Throws<KeyNotFoundException>(() => record.GetPaymentDate());
        }

        [Test]
        public void ThrowOnInvalidPaymentDate()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(invalidData);
            Assert.Throws<FormatException>(() => record.GetPaymentDate());
        }

        [Test]
        public void ValidInputInstrument()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(validData);
            var transition = new CorporateActionTransitionComponentRequest(new Dictionary<string, string> { { "Instrument/default/ClientInternal", "UNIQUE.ID.FOR.SECURITY" } }, 1, 0);
            Assert.That(record.GetInputInstrument(), Is.EqualTo(transition));
        }

        [Test]
        public void ThrowOnMissingInputInstrument()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(missingData);
            Assert.Throws<KeyNotFoundException>(() => record.GetInputInstrument());
        }

        [Test]
        public void ValidOutputInstruments()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(validData);
            var transition = new CorporateActionTransitionComponentRequest(new Dictionary<string, string> { { "Instrument/default/Currency", "USD" } }, 123, 0);
            Assert.That(record.GetOutputInstruments().Single(), Is.EqualTo(transition));
        }

        [Test]
        public void ThrowOnMissingOutputInstruments()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(missingData);
            Assert.Throws<KeyNotFoundException>(() => record.GetOutputInstruments());
        }

        [Test]
        public void ThrowOnInvalidOutputInstrument()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(invalidData);
            Assert.Throws<FormatException>(() => record.GetOutputInstruments());
        }

        [Test]
        public void ConstructRequestWithValidData()
        {
            var record = (ICorporateActionRecord) new CashDividendCorporateActionRecord(validData);
            var output = record.ConstructRequest("sourceId", "requestId");

            Assert.That(output.CorporateActionCode, Is.EqualTo("sourceId-requestId"));
            Assert.That(output.Description, Is.EqualTo("Cash Dividend"));
            Assert.That(output.AnnouncementDate, Is.EqualTo(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero)));
            Assert.That(output.ExDate, Is.EqualTo(new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.Zero)));
            Assert.That(output.RecordDate, Is.EqualTo(new DateTimeOffset(2000, 1, 3, 0, 0, 0, TimeSpan.Zero)));
            Assert.That(output.PaymentDate, Is.EqualTo(new DateTimeOffset(2000, 1, 4, 0, 0, 0, TimeSpan.Zero)));

            Assert.That(output.Transitions.Count, Is.EqualTo(1));
            var transition = output.Transitions.Single();

            Assert.That(transition.InputTransition.InstrumentIdentifiers.ContainsKey("Instrument/default/ClientInternal"), Is.True);
            Assert.That(transition.InputTransition.InstrumentIdentifiers["Instrument/default/ClientInternal"], Is.EqualTo("UNIQUE.ID.FOR.SECURITY"));
            Assert.That(transition.InputTransition.UnitsFactor, Is.EqualTo(1));
            Assert.That(transition.InputTransition.CostFactor, Is.EqualTo(0));

            Assert.That(transition.OutputTransitions.Count, Is.EqualTo(1));
            var outputTransition = transition.OutputTransitions.Single();

            Assert.That(outputTransition.InstrumentIdentifiers.ContainsKey("Instrument/default/Currency"), Is.True);
            Assert.That(outputTransition.InstrumentIdentifiers["Instrument/default/Currency"], Is.EqualTo("USD"));
            Assert.That(outputTransition.UnitsFactor, Is.EqualTo(123));
            Assert.That(outputTransition.CostFactor, Is.EqualTo(0));
        }

        [Test]
        public void ThrowOnMissingData()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(missingData);
            Assert.Throws<KeyNotFoundException>(() => record.ConstructRequest("sourceId", "requestId"));
        }

        [Test]
        public void ThrowOnInvalidData()
        {
            var record = (ICorporateActionRecord)new CashDividendCorporateActionRecord(invalidData);
            Assert.Throws<FormatException>(() => record.ConstructRequest("sourceId", "requestId"));
        }
    }
}