using System;
using System.Collections.Generic;
using Lusid.Sdk.Model;

namespace Lusid.FinDataEx.Output.OutputInterpreter
{
    public abstract class BaseOutputInterpreter : IOutputInterpreter
    {
        public abstract string GetActionCode(Dictionary<string, string> output, string requestName, int rowIndex);
        public abstract string GetDescription(Dictionary<string, string> output, string requestName, int rowIndex);
        public abstract DateTimeOffset? GetAnnouncementDate(Dictionary<string, string> output, string requestName, int rowIndex);
        public abstract DateTimeOffset? GetExecutionDate(Dictionary<string, string> output, string requestName, int rowIndex);
        public abstract DateTimeOffset? GetRecordDate(Dictionary<string, string> output, string requestName, int rowIndex);
        public abstract DateTimeOffset? GetPaymentDate(Dictionary<string, string> output, string requestName, int rowIndex);
        public abstract CorporateActionTransitionComponentRequest GetInputInstrument(Dictionary<string, string> output, string requestName, int rowIndex);
        public abstract List<CorporateActionTransitionComponentRequest> GetOutputInstruments(Dictionary<string, string> output, string requestName, int rowIndex);

        public virtual List<UpsertCorporateActionRequest> Interpret(DataLicenseOutput dataLicenseOutput)
        {
            var actions = new List<UpsertCorporateActionRequest>();

            for (var rowIndex = 0; rowIndex < dataLicenseOutput.Records.Count; rowIndex++)
            {
                var output = dataLicenseOutput.Records[rowIndex];
                var requestName = dataLicenseOutput.Id;

                var corporateActionCode = GetActionCode(output, requestName, rowIndex);
                var description = GetDescription(output, requestName, rowIndex);
                var announcementDate = GetAnnouncementDate(output, requestName, rowIndex);
                var executionDate = GetExecutionDate(output, requestName, rowIndex);
                var recordDate = GetRecordDate(output, requestName, rowIndex);
                var paymentDate = GetPaymentDate(output, requestName, rowIndex);
                var inputTransition = GetInputInstrument(output, requestName, rowIndex);
                var outputTransition = GetOutputInstruments(output, requestName, rowIndex);

                var transitions = new List<CorporateActionTransitionRequest> { new CorporateActionTransitionRequest(inputTransition, outputTransition) };

                var action = new UpsertCorporateActionRequest(corporateActionCode, description, announcementDate, executionDate, recordDate, paymentDate, transitions);

                actions.Add(action);
            }

            return actions;
        }
    }
}