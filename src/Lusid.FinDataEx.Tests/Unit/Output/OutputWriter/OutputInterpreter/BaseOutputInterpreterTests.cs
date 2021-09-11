using Lusid.FinDataEx.Output.OutputInterpreter;
using Lusid.Sdk.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lusid.FinDataEx.Tests.Unit.Output.OutputInterpreter
{
    [TestFixture]
    public class BaseOutputInterpreterTests
    {
        [Test]
        public void InterpretSingleRecords()
        {
            var fakeInstrumentResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string> { { "header0", "value0" } }
            };

            var fakeInput = new DataLicenseOutput("output", new List<string>(), fakeInstrumentResponse);

            var outputList = new ConcreteBaseOutputInterpreter().Interpret(fakeInput);

            Assert.That(outputList.Count(), Is.EqualTo(1));

            var output1 = outputList.Single();

            Assert.That(output1.CorporateActionCode, Is.EqualTo("codevalue0"));
            Assert.That(output1.Description, Is.EqualTo("descriptionvalue0"));
            Assert.That(output1.AnnouncementDate, Is.EqualTo(DateTimeOffset.MaxValue));
            Assert.That(output1.ExDate, Is.EqualTo(DateTimeOffset.MaxValue));
            Assert.That(output1.RecordDate, Is.EqualTo(DateTimeOffset.MaxValue));
            Assert.That(output1.PaymentDate, Is.EqualTo(DateTimeOffset.MaxValue));

            Assert.That(output1.Transitions.Count, Is.EqualTo(1));
            var transition = output1.Transitions.Single();

            Assert.That(transition.InputTransition.InstrumentIdentifiers.ContainsKey("Instrument/default/ClientInternal"), Is.True);
            Assert.That(transition.InputTransition.InstrumentIdentifiers["Instrument/default/ClientInternal"], Is.EqualTo("instrumentId"));
            Assert.That(transition.InputTransition.UnitsFactor, Is.EqualTo(0));
            Assert.That(transition.InputTransition.CostFactor, Is.EqualTo(0));

            Assert.That(transition.OutputTransitions.Count, Is.EqualTo(1));
            var outputTransition = transition.OutputTransitions.Single();

            Assert.That(outputTransition.InstrumentIdentifiers.ContainsKey("Instrument/default/Currency"), Is.True);
            Assert.That(outputTransition.InstrumentIdentifiers["Instrument/default/Currency"], Is.EqualTo("currencyId"));
            Assert.That(outputTransition.UnitsFactor, Is.EqualTo(0));
            Assert.That(outputTransition.CostFactor, Is.EqualTo(0));
        }

        [Test]
        public void InterpretMultipleRecord()
        {
            var fakeInstrumentResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string> { { "header0", "value0" } },
                new Dictionary<string, string> { { "header1", "value1" } }
            };

            var fakeInput = new DataLicenseOutput("output", new List<string>(), fakeInstrumentResponse);

            var outputList = new ConcreteBaseOutputInterpreter().Interpret(fakeInput);

            Assert.That(outputList.Count(), Is.EqualTo(2));

            var output1 = outputList[0];

            Assert.That(output1.CorporateActionCode, Is.EqualTo("codevalue0"));
            Assert.That(output1.Description, Is.EqualTo("descriptionvalue0"));
            Assert.That(output1.AnnouncementDate, Is.EqualTo(DateTimeOffset.MaxValue));
            Assert.That(output1.ExDate, Is.EqualTo(DateTimeOffset.MaxValue));
            Assert.That(output1.RecordDate, Is.EqualTo(DateTimeOffset.MaxValue));
            Assert.That(output1.PaymentDate, Is.EqualTo(DateTimeOffset.MaxValue));

            Assert.That(output1.Transitions.Count, Is.EqualTo(1));
            var transition1 = output1.Transitions.Single();

            Assert.That(transition1.InputTransition.InstrumentIdentifiers.ContainsKey("Instrument/default/ClientInternal"), Is.True);
            Assert.That(transition1.InputTransition.InstrumentIdentifiers["Instrument/default/ClientInternal"], Is.EqualTo("instrumentId"));
            Assert.That(transition1.InputTransition.UnitsFactor, Is.EqualTo(0));
            Assert.That(transition1.InputTransition.CostFactor, Is.EqualTo(0));

            Assert.That(transition1.OutputTransitions.Count, Is.EqualTo(1));
            var outputTransition1 = transition1.OutputTransitions.Single();

            Assert.That(outputTransition1.InstrumentIdentifiers.ContainsKey("Instrument/default/Currency"), Is.True);
            Assert.That(outputTransition1.InstrumentIdentifiers["Instrument/default/Currency"], Is.EqualTo("currencyId"));
            Assert.That(outputTransition1.UnitsFactor, Is.EqualTo(0));
            Assert.That(outputTransition1.CostFactor, Is.EqualTo(0));

            var output2 = outputList[1];

            Assert.That(output2.CorporateActionCode, Is.EqualTo("codevalue1"));
            Assert.That(output2.Description, Is.EqualTo("descriptionvalue1"));
            Assert.That(output2.AnnouncementDate, Is.EqualTo(DateTimeOffset.MaxValue));
            Assert.That(output2.ExDate, Is.EqualTo(DateTimeOffset.MaxValue));
            Assert.That(output2.RecordDate, Is.EqualTo(DateTimeOffset.MaxValue));
            Assert.That(output2.PaymentDate, Is.EqualTo(DateTimeOffset.MaxValue));

            Assert.That(output2.Transitions.Count, Is.EqualTo(1));
            var transition2 = output2.Transitions.Single();

            Assert.That(transition2.InputTransition.InstrumentIdentifiers.ContainsKey("Instrument/default/ClientInternal"), Is.True);
            Assert.That(transition2.InputTransition.InstrumentIdentifiers["Instrument/default/ClientInternal"], Is.EqualTo("instrumentId"));
            Assert.That(transition2.InputTransition.UnitsFactor, Is.EqualTo(1));
            Assert.That(transition2.InputTransition.CostFactor, Is.EqualTo(1));

            Assert.That(transition2.OutputTransitions.Count, Is.EqualTo(1));
            var outputTransition2 = transition2.OutputTransitions.Single();

            Assert.That(outputTransition2.InstrumentIdentifiers.ContainsKey("Instrument/default/Currency"), Is.True);
            Assert.That(outputTransition2.InstrumentIdentifiers["Instrument/default/Currency"], Is.EqualTo("currencyId"));
            Assert.That(outputTransition2.UnitsFactor, Is.EqualTo(1));
            Assert.That(outputTransition2.CostFactor, Is.EqualTo(1));
        }

        [Test]
        public void ReturnEmptyWhenNoOutput()
        {
            var fakeInstrumentResponse = new List<Dictionary<string, string>>();

            var fakeInput = new DataLicenseOutput("output", new List<string>(), fakeInstrumentResponse);

            var output = new ConcreteBaseOutputInterpreter().Interpret(fakeInput);

            Assert.That(output, Is.Empty);
        }
    }

    public class ConcreteBaseOutputInterpreter : BaseOutputInterpreter
    {
        public override string GetActionCode(Dictionary<string, string> output, string requestName, int rowIndex)
        {
            return "code" + output["header" + rowIndex];
        }

        public override string GetDescription(Dictionary<string, string> output, string requestName, int rowIndex)
        {
            return "description" + output["header" + rowIndex];
        }

        public override DateTimeOffset? GetExecutionDate(Dictionary<string, string> output, string requestName, int rowIndex)
        {
            return DateTimeOffset.MaxValue;
        }

        public override DateTimeOffset? GetAnnouncementDate(Dictionary<string, string> output, string requestName, int rowIndex)
        {
            return DateTimeOffset.MaxValue;
        }

        public override CorporateActionTransitionComponentRequest GetInputInstrument(Dictionary<string, string> output, string requestName, int rowIndex)
        {
            var units = rowIndex;
            var cost = rowIndex;
            var instruments = new Dictionary<string, string> { { "Instrument/default/ClientInternal", "instrumentId" } };

            return new CorporateActionTransitionComponentRequest(instruments, units, cost);
        }

        public override List<CorporateActionTransitionComponentRequest> GetOutputInstruments(Dictionary<string, string> output, string requestName, int rowIndex)
        {
            var units = rowIndex;
            var cost = rowIndex;
            var instruments = new Dictionary<string, string> { { "Instrument/default/Currency", "currencyId" } };

            return new List<CorporateActionTransitionComponentRequest> { new CorporateActionTransitionComponentRequest(instruments, units, cost) };
        }

        public override DateTimeOffset? GetPaymentDate(Dictionary<string, string> output, string requestName, int rowIndex)
        {
            return DateTimeOffset.MaxValue;
        }

        public override DateTimeOffset? GetRecordDate(Dictionary<string, string> output, string requestName, int rowIndex)
        {
            return DateTimeOffset.MaxValue;
        }
    }
}