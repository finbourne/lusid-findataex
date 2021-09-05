using System;
using System.Collections.Generic;

namespace Lusid.FinDataEx.Output.OutputInterpreter
{
    public class ServiceInterpreter : IOutputInterpreter
    {
        public string GetActionCode(Dictionary<string, string> output, string requestName, int rowIndex) => requestName + rowIndex;

        public string GetDescription(Dictionary<string, string> output, string requestName, int rowIndex) => output["actionId"];

        public DateTimeOffset? GetAnnouncementDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["announceDate"]);

        public DateTimeOffset? GetExecutionDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["effectiveDate"]);

        public DateTimeOffset? GetRecordDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["amendDate"]);

        public DateTimeOffset? GetPaymentDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["amendDate"]);

        public string GetInstrumentName(Dictionary<string, string> output, string requestName, int rowIndex) => output["securityId"];

        // This needs expanding and translating
        public decimal? GetUnits(Dictionary<string, string> output, string requestName, int rowIndex) => 1;

        public decimal? GetCost(Dictionary<string, string> output, string requestName, int rowIndex) => 0;
    }
}