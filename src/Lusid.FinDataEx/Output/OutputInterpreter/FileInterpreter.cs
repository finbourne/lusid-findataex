using Lusid.Sdk.Model;
using System;
using System.Collections.Generic;
using System.IO;
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

        public override string GetActionCode(Dictionary<string, string> output, string requestName, int rowIndex) => Path.GetFileNameWithoutExtension(requestName) + "-" + rowIndex;

        public override string GetDescription(Dictionary<string, string> output, string requestName, int rowIndex) => output["0-Action Type"];

        public override DateTimeOffset? GetAnnouncementDate(Dictionary<string, string> output, string requestName, int rowIndex) => new DateTimeOffset(DateTime.Parse(output["2-Announce/Declared Date"]).ToUniversalTime());

        public override DateTimeOffset? GetExecutionDate(Dictionary<string, string> output, string requestName, int rowIndex) => new DateTimeOffset(DateTime.Parse(output["3-Effective Date"]).ToUniversalTime());

        public override DateTimeOffset? GetRecordDate(Dictionary<string, string> output, string requestName, int rowIndex) => new DateTimeOffset(DateTime.Parse(output["11-Summary"].Replace("Record Date: ", "")).ToUniversalTime());

        public override DateTimeOffset? GetPaymentDate(Dictionary<string, string> output, string requestName, int rowIndex) => new DateTimeOffset(DateTime.Parse(output["12-Summary"].Replace("Pay Date: ", "")).ToUniversalTime());

        public override CorporateActionTransitionComponentRequest GetInputInstrument(Dictionary<string, string> output, string requestName, int rowIndex)
        {
            var units = 1;
            var cost = 0;
            var instruments = new Dictionary<string, string> { { "Instrument/default/ClientInternal", output["13-tad_id"] } };

            return new CorporateActionTransitionComponentRequest(instruments, units, cost);
        }

        public override List<CorporateActionTransitionComponentRequest> GetOutputInstruments(Dictionary<string, string> output, string requestName, int rowIndex)
        {
            var units = decimal.Parse(output["8-Summary"].Replace("Gross Amount: ", ""));
            var cost = 0;
            var instruments = new Dictionary<string, string> { { "Instrument/default/Currency", output["9-Summary"].Replace("Currency: ", "") } };

            return new List<CorporateActionTransitionComponentRequest> { new CorporateActionTransitionComponentRequest(instruments, units, cost) };
        }

        public override List<UpsertCorporateActionRequest> Interpret(DataLicenseOutput dataLicenseOutput)
        {
            var dataLicenseOutputOfActionType = dataLicenseOutput;

            var requestedActionTypes = _getOptions.CorpActionTypes.Select(actionType => ActionTypeMapping[actionType]).ToList();
            var dataRecordsOfActionType = dataLicenseOutput.Records.Where(output => requestedActionTypes.Contains(GetDescription(output, "", 0))).ToList();

            return base.Interpret(new DataLicenseOutput(dataLicenseOutput.Id, dataLicenseOutput.Header, dataRecordsOfActionType));
        }
    }
}