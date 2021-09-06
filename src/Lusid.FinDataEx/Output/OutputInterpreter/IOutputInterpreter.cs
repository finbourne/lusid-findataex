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

        List<UpsertCorporateActionRequest> Interpret(DataLicenseOutput dataLicenseOutput);
    }
}