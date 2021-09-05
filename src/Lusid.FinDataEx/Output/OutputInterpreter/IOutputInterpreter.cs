using Lusid.Sdk.Model;
using System;
using System.Collections.Generic;

namespace Lusid.FinDataEx.Output.OutputInterpreter
{
    interface IOutputInterpreter
    {
        string GetActionCode(Dictionary<string, string> output, string requestName, int rowIndex);

        string GetDescription(Dictionary<string, string> output, string requestName, int rowIndex);

        DateTimeOffset? GetAnnouncementDate(Dictionary<string, string> output, string requestName, int rowIndex);

        DateTimeOffset? GetExecutionDate(Dictionary<string, string> output, string requestName, int rowIndex);

        DateTimeOffset? GetRecordDate(Dictionary<string, string> output, string requestName, int rowIndex);

        DateTimeOffset? GetPaymentDate(Dictionary<string, string> output, string requestName, int rowIndex);

        string GetInstrumentName(Dictionary<string, string> output, string requestName, int rowIndex);

        decimal? GetUnits(Dictionary<string, string> output, string requestName, int rowIndex);

        decimal? GetCost(Dictionary<string, string> output, string requestName, int rowIndex);

        public List<UpsertCorporateActionRequest> Interpret(DataLicenseOutput dataLicenseOutput)
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

                var lusidInputInstrumentName = GetInstrumentName(output, requestName, rowIndex);
                var inputInstrumentName = lusidInputInstrumentName;
                var inputUnits = GetUnits( output, requestName, rowIndex);
                var inputCost = GetCost(output, requestName, rowIndex);

                var lusidOutputInstrumentName = lusidInputInstrumentName;
                var outputInstrumentName = inputInstrumentName;
                var outputUnits = inputUnits;
                var outputCost = inputCost;

                var inputTransition = new CorporateActionTransitionComponentRequest(
                    new Dictionary<string, string>
                    {
                        { lusidInputInstrumentName, inputInstrumentName }
                    },
                    inputUnits,
                    inputCost);

                var outputTransition = new List<CorporateActionTransitionComponentRequest>
                {
                    new CorporateActionTransitionComponentRequest(
                        new Dictionary<string, string>
                        {
                            { lusidOutputInstrumentName, outputInstrumentName }
                        },
                        outputUnits,
                        outputCost)
                };

                var transitions = new List<CorporateActionTransitionRequest> { new CorporateActionTransitionRequest(inputTransition, outputTransition) };

                var action = new UpsertCorporateActionRequest(corporateActionCode, description, announcementDate, executionDate, recordDate, paymentDate, transitions);

                actions.Add(action);
            }

            return actions;
        }
    }
}