using Lusid.FinDataEx.Output.OutputInterpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx.Tests.Unit.Output.OutputInterpreter
{
    [TestFixture]
    public class ServiceInterpreterTests
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

            Assert.Throws<NotImplementedException>(() => new ServiceInterpreter(fakeOptions).Interpret(fakeInput));
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

            Assert.Throws<NotImplementedException>(() => new ServiceInterpreter(fakeOptions).Interpret(fakeInput));
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

            Assert.Throws<NotImplementedException>(() => new ServiceInterpreter(fakeOptions).Interpret(fakeInput));
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

            Assert.Throws<NotImplementedException>(() => new ServiceInterpreter(fakeOptions).Interpret(fakeInput));
        }
    }
}