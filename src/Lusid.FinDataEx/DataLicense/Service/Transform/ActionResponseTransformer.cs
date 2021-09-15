using System;
using System.Collections.Generic;
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseConstants;

namespace Lusid.FinDataEx.DataLicense.Service.Transform
{
    /// <summary>
    ///  Transformer for BBG DL GetAction Calls.
    /// </summary>
    public class GetActionResponseTransformer : IResponseTransformer
    {
        public List<Dictionary<string, string>> Transform(PerSecurityResponse perSecurityResponse)
        {
            var getActionsResponse = perSecurityResponse as RetrieveGetActionsResponse;

            var corpActionOutputId = getActionsResponse.responseId;
            var actionsInstrumentDatas = getActionsResponse.instrumentDatas;

            // Corporate action headers constructed from the intersection of all corporate action fields
            // (mainly required for requests that span multiple corporate action types).
            var headers = new HashSet<string>{ TimeStarted, TimeFinished };
            var corpActionRecords = new List<Dictionary<string, string>>();
            foreach (var instrumentData in actionsInstrumentDatas)
            {
                // Corp action requests for individual instruments may have failed which need to be logged
                // But should not result in entire failure of batch.
                if (instrumentData.code != DataLicenseService.InstrumentSuccessCode)
                {
                    Console.WriteLine($"Error in GetAction instrument for {instrumentData.instrument.id}. " +
                                      $"Check GetAction response log above. Continuing to remaining instruments...");
                    continue;
                }

                // Populate the data general to all corporate actions.
                var corpActionRecord = new Dictionary<string, string>();
                var actionStandardFields = instrumentData.standardFields;
                if (actionStandardFields == null)
                {
                    continue;
                }

                foreach (var standardFieldPropInfo in typeof(ActionStandardFields).GetProperties())
                {
                    var fieldValue = standardFieldPropInfo.GetValue(actionStandardFields, null);
                    var fieldValueEntry = fieldValue?.ToString() ?? string.Empty;
                    corpActionRecord.Add(standardFieldPropInfo.Name, fieldValueEntry);
                    headers.Add(standardFieldPropInfo.Name);
                }

                // Populate the data specific to the corporate action type
                foreach (var corpActionData in instrumentData.data)
                {
                    corpActionRecord.Add(corpActionData.field, corpActionData.value);
                    headers.Add(corpActionData.field);
                }
                corpActionRecords.Add(corpActionRecord);
            }
            
            // Populate all records with timestamp fields from corp actions response
            var timeStarted = new DateTimeOffset(getActionsResponse.timestarted.ToUniversalTime(), TimeSpan.Zero).ToString(DefaultCultureInfo);
            var timeFinished = new DateTimeOffset(getActionsResponse.timefinished.ToUniversalTime(), TimeSpan.Zero).ToString(DefaultCultureInfo);
            corpActionRecords.ForEach(r =>
            {
                r.Add(TimeStarted, timeStarted);
                r.Add(TimeFinished, timeFinished);
            });

            // Retrieve actions for all instruments or if failed then return empty output
            return corpActionRecords;
        }
    }
}