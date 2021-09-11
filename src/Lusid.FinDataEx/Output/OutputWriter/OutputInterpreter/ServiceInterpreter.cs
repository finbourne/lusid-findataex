using Lusid.Sdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx.Output.OutputInterpreter
{
    public class ServiceInterpreter : BaseOutputInterpreter
    {
        private readonly GetActionsOptions _getOptions;

        private static readonly Dictionary<CorpActionType, string> ActionTypeMapping = new Dictionary<CorpActionType, string>
        {
            { CorpActionType.DVD_CASH, "Cash Dividend" },
            { CorpActionType.DVD_STOCK, "Stock Dividend" },
            { CorpActionType.STOCK_SPLT, "Stock Split" }
        };

        public ServiceInterpreter(GetActionsOptions getOptions)
        {
            _getOptions = getOptions;
        }

        public override string GetActionCode(Dictionary<string, string> output, string requestName, int rowIndex) => requestName + rowIndex;

        public override string GetDescription(Dictionary<string, string> output, string requestName, int rowIndex) => output["actionId"];

        public override DateTimeOffset? GetAnnouncementDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["announceDate"]);

        public override DateTimeOffset? GetExecutionDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["effectiveDate"]);

        public override DateTimeOffset? GetRecordDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["amendDate"]);

        public override DateTimeOffset? GetPaymentDate(Dictionary<string, string> output, string requestName, int rowIndex) => DateTimeOffset.Parse(output["amendDate"]);

        public override CorporateActionTransitionComponentRequest GetInputInstrument(Dictionary<string, string> output, string requestName, int rowIndex)
        {
            var units = 1;
            var cost = 0;
            var instruments = new Dictionary<string, string> { { "Instruments/default/ClientInternal", output["13-tad_id"].Split(" ").First() } };

            return new CorporateActionTransitionComponentRequest(instruments, units, cost);
        }

        public override List<CorporateActionTransitionComponentRequest> GetOutputInstruments(Dictionary<string, string> output, string requestName, int rowIndex)
        {
            var units = decimal.Parse(output["8-Summary"].Replace("Gross Amount: ", ""));
            var cost = 0;
            var instruments = new Dictionary<string, string> { { "Instruments/default/Currency", output["9-Summary"].Replace("Currency: ", "") } };

            return new List<CorporateActionTransitionComponentRequest> { new CorporateActionTransitionComponentRequest(instruments, units, cost) };
        }

        public override List<UpsertCorporateActionRequest> Interpret(DataLicenseOutput dataLicenseOutput)
        {
            throw new NotImplementedException("ServiceInterpreter is not complete at this time");
        }
    }
}