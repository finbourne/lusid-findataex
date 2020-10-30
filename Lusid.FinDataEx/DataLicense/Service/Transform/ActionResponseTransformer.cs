using System;
using System.Collections.Generic;
using System.Linq;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Transform
{
    /// <summary>
    ///  Transformer for BBG DL GetAction Calls.
    ///
    /// </summary>
    public class GetActionResponseTransformer : IDataLicenseResponseTransformer<RetrieveGetActionsResponse>
    {
        /// <summary>
        /// Transform a a GetAction response from BBG DLWS to a standardised output.
        /// 
        /// </summary>
        /// <param name="perSecurityResponse">GetAction response from BBG DLWS</param>
        /// <returns>FinDataOutput of data returned for instruments requested</returns>
        public DataLicenseOutput Transform(RetrieveGetActionsResponse perSecurityResponse)
        {
            var corpActionOutputId = $"{perSecurityResponse.responseId}_GetActions";
            var actionsInstrumentDatas = perSecurityResponse.instrumentDatas;
            // if no corporate actions are returned for the specific type than return an empty output.
            if (!actionsInstrumentDatas.Any())
            {
                return DataLicenseOutput.Empty(corpActionOutputId);
            }

            // headers from intersection of all fields (mainly required for requests that span multiple corporate
            // action types)
            var headers = new HashSet<string>(){"timeStarted", "timeFinished"};
            var corpActionRecords = new List<Dictionary<string, string>>();
            foreach (var instrumentData in actionsInstrumentDatas)
            {
                Dictionary<string, string> corpActionRecord = new Dictionary<string, string>();
                // corp action requests for individual instruments may have failed which need to be logged
                // but should not result in entire failure of batch.
                if (instrumentData.code != DataLicenseService.InstrumentSuccessCode)
                {
                    Console.WriteLine($"Error in GetAction instrument for {instrumentData.instrument.id}. " +
                                      $"Check GetAction response log above. Continuing to remaining instruments...");
                    continue;
                }

                // Populate the data general to all corporate actions
                var actionStandardFields = instrumentData.standardFields;
                foreach (var standardFieldPropInfo in typeof(ActionStandardFields).GetProperties())
                {
                    var fieldValue = standardFieldPropInfo.GetValue(actionStandardFields, null);
                    var fieldValueEntry = (fieldValue == null) ? "" : fieldValue.ToString();
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
            
            //populate all records with timestamp fields
            var timeStarted = new DateTimeOffset(perSecurityResponse.timestarted.ToUniversalTime(), TimeSpan.Zero).ToString();
            var timeFinished = new DateTimeOffset(perSecurityResponse.timefinished.ToUniversalTime(), TimeSpan.Zero).ToString();
            corpActionRecords.ForEach(r =>
            {
                r.Add("timeStarted", timeStarted);
                r.Add("timeFinished", timeFinished);
            });

            // if failed to retrieve actions for all instruments then return empty output
            return corpActionRecords.Any()
                ? new DataLicenseOutput(corpActionOutputId, headers, corpActionRecords)
                : DataLicenseOutput.Empty(corpActionOutputId);
        }

    }
}