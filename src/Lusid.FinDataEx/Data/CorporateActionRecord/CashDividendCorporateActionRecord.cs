using Lusid.Sdk.Model;
using System;
using System.Collections.Generic;

namespace Lusid.FinDataEx.Data.CorporateActionRecord
{
    public class CashDividendCorporateActionRecord : ICorporateActionRecord
    {
        private readonly Dictionary<string, string> _rawData;

        public Dictionary<string, string> RawData { get => _rawData; }

        public CashDividendCorporateActionRecord(Dictionary<string, string> input)
        {
            _rawData = input;
        }

        public string GetActionCode(string sourceId, string requestId) => sourceId + "-" + requestId;

        public string GetDescription() => RawData["0-Action Type"];

        public DateTimeOffset? GetAnnouncementDate() => new DateTimeOffset(DateTime.Parse(RawData["2-Announce/Declared Date"]).ToUniversalTime());

        public DateTimeOffset? GetExecutionDate() => new DateTimeOffset(DateTime.Parse(RawData["3-Effective Date"]).ToUniversalTime());

        public DateTimeOffset? GetRecordDate() => new DateTimeOffset(DateTime.Parse(RawData["11-Summary"].Replace("Record Date: ", "")).ToUniversalTime());

        public DateTimeOffset? GetPaymentDate() => new DateTimeOffset(DateTime.Parse(RawData["12-Summary"].Replace("Pay Date: ", "")).ToUniversalTime());

        public CorporateActionTransitionComponentRequest GetInputInstrument()
        {
            var units = 1;
            var cost = 0;
            var instruments = new Dictionary<string, string> { { "Instrument/default/ClientInternal", RawData["13-tad_id"] } };

            return new CorporateActionTransitionComponentRequest(instruments, units, cost);
        }

        public List<CorporateActionTransitionComponentRequest> GetOutputInstruments()
        {
            var units = decimal.Parse(RawData["8-Summary"].Replace("Gross Amount: ", ""));
            var cost = 0;
            var instruments = new Dictionary<string, string> { { "Instrument/default/Currency", RawData["9-Summary"].Replace(" Currency: ", "") } };

            return new List<CorporateActionTransitionComponentRequest> { new CorporateActionTransitionComponentRequest(instruments, units, cost) };
        }
    }
}