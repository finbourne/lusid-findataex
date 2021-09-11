using Lusid.FinDataEx.Output.OutputInterpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx.Tests.Unit.Output.OutputInterpreter
{
    [TestFixture]
    public class FileInterpreterTests
    {
        [Test]
        public void InterpretSingleRecord()
        {
            var fakeOptions = new GetActionsOptions
            {
                CorpActionTypes = new List<CorpActionType> { CorpActionType.DVD_CASH }
            };

            var fakeInstrumentResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "0-Action Type", "Cash Dividend" },
                    { "2-Announce/Declared Date", "01/01/2021 00:00" },
                    { "3-Effective Date", "01/01/2021 00:00" },
                    { "11-Summary", "Record Date: 01/01/2021 00:00" },
                    { "12-Summary", "Pay Date: 01/01/2021 00:00" },
                    { "13-tad_id", "unique id" },
                    { "9-Summary", " Currency: USD" },
                    { "8-Summary", "Gross Amount: 0.1" }
                }
            };

            var fakeInput = new DataLicenseOutput("output", new List<string>(), fakeInstrumentResponse);

            var outputList = new FileInterpreter(fakeOptions).Interpret(fakeInput);

            Assert.That(outputList.Count(), Is.EqualTo(1));

            var output1 = outputList.Single();

            Assert.That(output1.CorporateActionCode, Is.EqualTo("output-0"));
            Assert.That(output1.Description, Is.EqualTo("Cash Dividend"));
            Assert.That(output1.AnnouncementDate, Is.EqualTo(DateTimeOffset.Parse("01/01/2021 00:00")));
            Assert.That(output1.ExDate, Is.EqualTo(DateTimeOffset.Parse("01/01/2021 00:00")));
            Assert.That(output1.RecordDate, Is.EqualTo(DateTimeOffset.Parse("01/01/2021 00:00")));
            Assert.That(output1.PaymentDate, Is.EqualTo(DateTimeOffset.Parse("01/01/2021 00:00")));

            Assert.That(output1.Transitions.Count, Is.EqualTo(1));
            var transition1 = output1.Transitions.Single();

            Assert.That(transition1.InputTransition.InstrumentIdentifiers.ContainsKey("Instrument/default/ClientInternal"), Is.True);
            Assert.That(transition1.InputTransition.InstrumentIdentifiers["Instrument/default/ClientInternal"], Is.EqualTo("unique id"));
            Assert.That(transition1.InputTransition.UnitsFactor, Is.EqualTo(1));
            Assert.That(transition1.InputTransition.CostFactor, Is.EqualTo(0));

            Assert.That(transition1.OutputTransitions.Count, Is.EqualTo(1));
            var outputTransition1 = transition1.OutputTransitions.Single();

            Assert.That(outputTransition1.InstrumentIdentifiers.ContainsKey("Instrument/default/Currency"), Is.True);
            Assert.That(outputTransition1.InstrumentIdentifiers["Instrument/default/Currency"], Is.EqualTo("USD"));
            Assert.That(outputTransition1.UnitsFactor, Is.EqualTo(0.1));
            Assert.That(outputTransition1.CostFactor, Is.EqualTo(0));
        }

        [Test]
        public void InterpretSingleRecordIgnoringActionTypesNotRequested()
        {
            var fakeOptions = new GetActionsOptions
            {
                CorpActionTypes = new List<CorpActionType> { CorpActionType.DVD_CASH }
            };

            var fakeInstrumentResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "0-Action Type", "Cash Dividend" },
                    { "2-Announce/Declared Date", "01/01/2021 00:00" },
                    { "3-Effective Date", "01/01/2021 00:00" },
                    { "11-Summary", "Record Date: 01/01/2021 00:00" },
                    { "12-Summary", "Pay Date: 01/01/2021 00:00" },
                    { "13-tad_id", "unique id" },
                    { "9-Summary", " Currency: USD" },
                    { "8-Summary", "Gross Amount: 0.1" }
                },
                new Dictionary<string, string>
                {
                    { "0-Action Type", "Stock Split" },
                    { "2-Announce/Declared Date", "01/01/2021 00:00" },
                    { "3-Effective Date", "01/01/2021 00:00" },
                    { "11-Summary", "Record Date: 01/01/2021 00:00" },
                    { "12-Summary", "Pay Date: 01/01/2021 00:00" },
                    { "13-tad_id", "unique id" },
                    { "9-Summary", " Currency: USD" },
                    { "8-Summary", "Gross Amount: 0.1" }
                }
            };

            var fakeInput = new DataLicenseOutput("output", new List<string>(), fakeInstrumentResponse);

            var outputList = new FileInterpreter(fakeOptions).Interpret(fakeInput);

            Assert.That(outputList.Count(), Is.EqualTo(1));

            var output1 = outputList.Single();

            Assert.That(output1.CorporateActionCode, Is.EqualTo("output-0"));
            Assert.That(output1.Description, Is.EqualTo("Cash Dividend"));
            Assert.That(output1.AnnouncementDate, Is.EqualTo(DateTimeOffset.Parse("01/01/2021 00:00")));
            Assert.That(output1.ExDate, Is.EqualTo(DateTimeOffset.Parse("01/01/2021 00:00")));
            Assert.That(output1.RecordDate, Is.EqualTo(DateTimeOffset.Parse("01/01/2021 00:00")));
            Assert.That(output1.PaymentDate, Is.EqualTo(DateTimeOffset.Parse("01/01/2021 00:00")));

            Assert.That(output1.Transitions.Count, Is.EqualTo(1));
            var transition1 = output1.Transitions.Single();

            Assert.That(transition1.InputTransition.InstrumentIdentifiers.ContainsKey("Instrument/default/ClientInternal"), Is.True);
            Assert.That(transition1.InputTransition.InstrumentIdentifiers["Instrument/default/ClientInternal"], Is.EqualTo("unique id"));
            Assert.That(transition1.InputTransition.UnitsFactor, Is.EqualTo(1));
            Assert.That(transition1.InputTransition.CostFactor, Is.EqualTo(0));

            Assert.That(transition1.OutputTransitions.Count, Is.EqualTo(1));
            var outputTransition = transition1.OutputTransitions.Single();

            Assert.That(outputTransition.InstrumentIdentifiers.ContainsKey("Instrument/default/Currency"), Is.True);
            Assert.That(outputTransition.InstrumentIdentifiers["Instrument/default/Currency"], Is.EqualTo("USD"));
            Assert.That(outputTransition.UnitsFactor, Is.EqualTo(0.1));
            Assert.That(outputTransition.CostFactor, Is.EqualTo(0));
        }

        [Test]
        public void InterpretMultipleRecord()
        {
            var fakeOptions = new GetActionsOptions
            {
                CorpActionTypes = new List<CorpActionType> { CorpActionType.DVD_CASH }
            };

            var fakeInstrumentResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "0-Action Type", "Cash Dividend" },
                    { "2-Announce/Declared Date", "01/01/2021 00:00" },
                    { "3-Effective Date", "01/01/2021 00:00" },
                    { "11-Summary", "Record Date: 01/01/2021 00:00" },
                    { "12-Summary", "Pay Date: 01/01/2021 00:00" },
                    { "13-tad_id", "unique id1" },
                    { "9-Summary", " Currency: USD" },
                    { "8-Summary", "Gross Amount: 0.1" }
                },
                new Dictionary<string, string>
                {
                    { "0-Action Type", "Cash Dividend" },
                    { "2-Announce/Declared Date", "02/01/2021 00:00" },
                    { "3-Effective Date", "02/01/2021 00:00" },
                    { "11-Summary", "Record Date: 02/01/2021 00:00" },
                    { "12-Summary", "Pay Date: 02/01/2021 00:00" },
                    { "13-tad_id", "unique id2" },
                    { "9-Summary", " Currency: USD" },
                    { "8-Summary", "Gross Amount: 0.2" }
                }
            };

            var fakeInput = new DataLicenseOutput("output", new List<string>(), fakeInstrumentResponse);

            var outputList = new FileInterpreter(fakeOptions).Interpret(fakeInput);

            Assert.That(outputList.Count(), Is.EqualTo(2));

            var output1 = outputList[0];

            Assert.That(output1.CorporateActionCode, Is.EqualTo("output-0"));
            Assert.That(output1.Description, Is.EqualTo("Cash Dividend"));
            Assert.That(output1.AnnouncementDate, Is.EqualTo(DateTimeOffset.Parse("01/01/2021 00:00")));
            Assert.That(output1.ExDate, Is.EqualTo(DateTimeOffset.Parse("01/01/2021 00:00")));
            Assert.That(output1.RecordDate, Is.EqualTo(DateTimeOffset.Parse("01/01/2021 00:00")));
            Assert.That(output1.PaymentDate, Is.EqualTo(DateTimeOffset.Parse("01/01/2021 00:00")));

            Assert.That(output1.Transitions.Count, Is.EqualTo(1));
            var transition1 = output1.Transitions.Single();

            Assert.That(transition1.InputTransition.InstrumentIdentifiers.ContainsKey("Instrument/default/ClientInternal"), Is.True);
            Assert.That(transition1.InputTransition.InstrumentIdentifiers["Instrument/default/ClientInternal"], Is.EqualTo("unique id1"));
            Assert.That(transition1.InputTransition.UnitsFactor, Is.EqualTo(1));
            Assert.That(transition1.InputTransition.CostFactor, Is.EqualTo(0));

            Assert.That(transition1.OutputTransitions.Count, Is.EqualTo(1));
            var outputTransition1 = transition1.OutputTransitions.Single();

            Assert.That(outputTransition1.InstrumentIdentifiers.ContainsKey("Instrument/default/Currency"), Is.True);
            Assert.That(outputTransition1.InstrumentIdentifiers["Instrument/default/Currency"], Is.EqualTo("USD"));
            Assert.That(outputTransition1.UnitsFactor, Is.EqualTo(0.1));
            Assert.That(outputTransition1.CostFactor, Is.EqualTo(0));

            var output2 = outputList[1];

            Assert.That(output2.CorporateActionCode, Is.EqualTo("output-1"));
            Assert.That(output2.Description, Is.EqualTo("Cash Dividend"));
            Assert.That(output2.AnnouncementDate, Is.EqualTo(DateTimeOffset.Parse("02/01/2021 00:00")));
            Assert.That(output2.ExDate, Is.EqualTo(DateTimeOffset.Parse("02/01/2021 00:00")));
            Assert.That(output2.RecordDate, Is.EqualTo(DateTimeOffset.Parse("02/01/2021 00:00")));
            Assert.That(output2.PaymentDate, Is.EqualTo(DateTimeOffset.Parse("02/01/2021 00:00")));

            Assert.That(output2.Transitions.Count, Is.EqualTo(1));
            var transition2 = output2.Transitions.Single();

            Assert.That(transition2.InputTransition.InstrumentIdentifiers.ContainsKey("Instrument/default/ClientInternal"), Is.True);
            Assert.That(transition2.InputTransition.InstrumentIdentifiers["Instrument/default/ClientInternal"], Is.EqualTo("unique id2"));
            Assert.That(transition2.InputTransition.UnitsFactor, Is.EqualTo(1));
            Assert.That(transition2.InputTransition.CostFactor, Is.EqualTo(0));

            Assert.That(transition2.OutputTransitions.Count, Is.EqualTo(1));
            var outputTransition2 = transition2.OutputTransitions.Single();

            Assert.That(outputTransition2.InstrumentIdentifiers.ContainsKey("Instrument/default/Currency"), Is.True);
            Assert.That(outputTransition2.InstrumentIdentifiers["Instrument/default/Currency"], Is.EqualTo("USD"));
            Assert.That(outputTransition2.UnitsFactor, Is.EqualTo(0.2));
            Assert.That(outputTransition2.CostFactor, Is.EqualTo(0));
        }

        [Test]
        public void ReturnEmptyWhenNoOutput()
        {
            var fakeOptions = new GetActionsOptions
            {
                CorpActionTypes = new List<CorpActionType> { CorpActionType.DVD_CASH }
            };

            var fakeInstrumentResponse = new List<Dictionary<string, string>>();

            var fakeInput = new DataLicenseOutput("output", new List<string>(), fakeInstrumentResponse);

            var output = new FileInterpreter(fakeOptions).Interpret(fakeInput);

            Assert.That(output, Is.Empty);
        }
    }
}
