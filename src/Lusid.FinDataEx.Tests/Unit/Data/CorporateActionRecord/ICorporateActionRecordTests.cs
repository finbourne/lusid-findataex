using Lusid.FinDataEx.Data.CorporateActionRecord;
using Lusid.Sdk.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lusid.FinDataEx.Tests.Unit.Data.CorporateActionRecord
{
    [TestFixture]
    public class CorporateActionRecordTests
    {
        [Test]
        public void ConstructRequestWithData()
        {
            var record= (ICorporateActionRecord) new ConcreteICorporateActionRecord();
            var output = record.ConstructRequest("sourceId", "requestId");

            Assert.That(output.CorporateActionCode, Is.EqualTo("sourceId-requestId"));
            Assert.That(output.Description, Is.EqualTo("description"));
            Assert.That(output.AnnouncementDate, Is.EqualTo(DateTimeOffset.MaxValue));
            Assert.That(output.ExDate, Is.EqualTo(DateTimeOffset.MaxValue));
            Assert.That(output.RecordDate, Is.EqualTo(DateTimeOffset.MaxValue));
            Assert.That(output.PaymentDate, Is.EqualTo(DateTimeOffset.MaxValue));

            Assert.That(output.Transitions.Count, Is.EqualTo(1));
            var transition = output.Transitions.Single();

            Assert.That(transition.InputTransition.InstrumentIdentifiers.ContainsKey("Instrument/default/ClientInternal"), Is.True);
            Assert.That(transition.InputTransition.InstrumentIdentifiers["Instrument/default/ClientInternal"], Is.EqualTo("instrumentId"));
            Assert.That(transition.InputTransition.UnitsFactor, Is.EqualTo(1));
            Assert.That(transition.InputTransition.CostFactor, Is.EqualTo(1));

            Assert.That(transition.OutputTransitions.Count, Is.EqualTo(1));
            var outputTransition = transition.OutputTransitions.Single();

            Assert.That(outputTransition.InstrumentIdentifiers.ContainsKey("Instrument/default/Currency"), Is.True);
            Assert.That(outputTransition.InstrumentIdentifiers["Instrument/default/Currency"], Is.EqualTo("currencyId"));
            Assert.That(outputTransition.UnitsFactor, Is.EqualTo(2));
            Assert.That(outputTransition.CostFactor, Is.EqualTo(2));
        }
    }

    public class ConcreteICorporateActionRecord : ICorporateActionRecord
    {
        public Dictionary<string, string> RawData => throw new NotImplementedException();

        public string GetActionCode(string sourceId, string requestId)
        {
            return sourceId + "-" + requestId;
        }

        public string GetDescription()
        {
            return "description";
        }

        public DateTimeOffset? GetExecutionDate()
        {
            return DateTimeOffset.MaxValue;
        }

        public DateTimeOffset? GetAnnouncementDate()
        {
            return DateTimeOffset.MaxValue;
        }

        public CorporateActionTransitionComponentRequest GetInputInstrument()
        {
            var units = 1;
            var cost = 1;
            var instruments = new Dictionary<string, string> { { "Instrument/default/ClientInternal", "instrumentId" } };

            return new CorporateActionTransitionComponentRequest(instruments, units, cost);
        }

        public List<CorporateActionTransitionComponentRequest> GetOutputInstruments()
        {
            var units = 2;
            var cost = 2;
            var instruments = new Dictionary<string, string> { { "Instrument/default/Currency", "currencyId" } };

            return new List<CorporateActionTransitionComponentRequest> { new CorporateActionTransitionComponentRequest(instruments, units, cost) };
        }

        public DateTimeOffset? GetPaymentDate()
        {
            return DateTimeOffset.MaxValue;
        }

        public DateTimeOffset? GetRecordDate()
        {
            return DateTimeOffset.MaxValue;
        }
    }
}