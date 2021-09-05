using Lusid.Sdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx.Output.OutputInterpreter
{
    public class FileInterpreter : BaseOutputInterpreter
    {
        private readonly GetActionsOptions _getOptions;

        private static readonly Dictionary<CorpActionType, string> ActionTypeMapping = new Dictionary<CorpActionType, string>
        {
            { CorpActionType.DVD_CASH, "Cash Dividend" },
            { CorpActionType.DVD_STOCK, "Stock Dividend" },
            { CorpActionType.STOCK_SPLT, "Stock Split" }
        };

        public FileInterpreter(GetActionsOptions getOptions)
        {
            _getOptions = getOptions;
        }

        public override string GetActionCode(Dictionary<string, string> output, string requestName, int rowIndex) => requestName + rowIndex;

        public override string GetDescription(Dictionary<string, string> output, string requestName, int rowIndex) => output["0-Action Type"];

        public override DateTimeOffset? GetAnnouncementDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["2-Announce/Declared Date"]);

        public override DateTimeOffset? GetExecutionDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["3-Effective Date"]);

        public override DateTimeOffset? GetRecordDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["11-Summary"]);

        public override DateTimeOffset? GetPaymentDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["12-Summary"]);

        public override string GetInstrumentName(Dictionary<string, string> output, string requestName, int rowIndex) => output["13-tad_id"].Split(" ").First();

        public override decimal? GetUnits(Dictionary<string, string> output, string requestName, int rowIndex) => 1;

        public override decimal? GetCost(Dictionary<string, string> output, string requestName, int rowIndex) => decimal.Parse(output["8-Summary"].Replace("Gross Amount: ", ""));

        public override List<UpsertCorporateActionRequest> Interpret(DataLicenseOutput dataLicenseOutput)
        {
            var dataLicenseOutputOfActionType = dataLicenseOutput;

            var requestedActionTypes = _getOptions.CorpActionTypes.Select(actionType => ActionTypeMapping[actionType]).ToList();
            var dataRecordsOfActionType = dataLicenseOutput.Records.Where(output => requestedActionTypes.Contains(GetDescription(output, "", 0))).ToList();

            return base.Interpret(new DataLicenseOutput(dataLicenseOutput.Id, dataLicenseOutput.Header, dataRecordsOfActionType));
        }
    }
}