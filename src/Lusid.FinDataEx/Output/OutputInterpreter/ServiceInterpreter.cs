using System;
using System.Collections.Generic;

namespace Lusid.FinDataEx.Output.OutputInterpreter
{
    public class ServiceInterpreter : BaseOutputInterpreter
    {
        public override string GetActionCode(Dictionary<string, string> output, string requestName, int rowIndex) => requestName + rowIndex;

        public override string GetDescription(Dictionary<string, string> output, string requestName, int rowIndex) => output["actionId"];

        public override DateTimeOffset? GetAnnouncementDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["announceDate"]);

        public override DateTimeOffset? GetExecutionDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["effectiveDate"]);

        public override DateTimeOffset? GetRecordDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["amendDate"]);

        public override DateTimeOffset? GetPaymentDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["amendDate"]);

        public override string GetInstrumentName(Dictionary<string, string> output, string requestName, int rowIndex) => output["securityId"];

        // This needs expanding and translating
        public override decimal? GetUnits(Dictionary<string, string> output, string requestName, int rowIndex) => 1;

        public override decimal? GetCost(Dictionary<string, string> output, string requestName, int rowIndex) => 0;
    }
}