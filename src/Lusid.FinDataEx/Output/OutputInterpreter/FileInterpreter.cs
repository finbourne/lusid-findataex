using System;
using System.Collections.Generic;
using System.Linq;

namespace Lusid.FinDataEx.Output.OutputInterpreter
{
    public class FileInterpreter : IOutputInterpreter
    {
        public string GetActionCode(Dictionary<string, string> output, string requestName, int rowIndex) => requestName + rowIndex;

        public string GetDescription(Dictionary<string, string> output, string requestName, int rowIndex) => output["0-Action Type"];

        public DateTimeOffset? GetAnnouncementDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["2-Announce/Declared Date"]);

        public DateTimeOffset? GetExecutionDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["3-Effective Date"]);

        public DateTimeOffset? GetRecordDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["11-Summary"]);

        public DateTimeOffset? GetPaymentDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["12-Summary"]);

        public string GetInstrumentName(Dictionary<string, string> output, string requestName, int rowIndex) => output["13-tad_id"].Split(" ").First();

        public decimal? GetUnits(Dictionary<string, string> output, string requestName, int rowIndex) => 1;

        public decimal? GetCost(Dictionary<string, string> output, string requestName, int rowIndex) => decimal.Parse(output["8-Summary"].Replace("Gross Amount: ", ""));
    }
}