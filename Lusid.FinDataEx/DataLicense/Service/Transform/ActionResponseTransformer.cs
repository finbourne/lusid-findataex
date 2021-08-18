using System;
using System.Collections.Generic;
using System.Linq;
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseConstants;

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
        /// <param name="getActionsResponse">GetAction response from BBG DLWS</param>
        /// <returns>FinDataOutput of data returned for instruments requested</returns>
        public DataLicenseOutput Transform(RetrieveGetActionsResponse getActionsResponse)
        {
            var corpActionOutputId = getActionsResponse.responseId;
            var actionsInstrumentDatas = getActionsResponse.instrumentDatas;
            // if no corporate actions are returned for the specific type than return an empty output.
            if (!actionsInstrumentDatas.Any())
            {
                return DataLicenseOutput.Empty(corpActionOutputId);
            }

            // corporate action headers constructed from the intersection of all corporate action fields
            // (mainly required for requests that span multiple corporate action types).
            var headers = new HashSet<string>(){TimeStarted, TimeFinished};
            // setup corp action records
            var corpActionRecords = new List<Dictionary<string, string>>();
            foreach (var instrumentData in actionsInstrumentDatas)
            {
                var corpActionRecord = new Dictionary<string, string>();
                // corp action requests for individual instruments may have failed which need to be logged
                // but should not result in entire failure of batch.
                if (instrumentData.code != DataLicenseService.InstrumentSuccessCode)
                {
                    Console.WriteLine($"Error in GetAction instrument for {instrumentData.instrument.id}. " +
                                      $"Check GetAction response log above. Continuing to remaining instruments...");
                    continue;
                }

                // Populate the data general to all corporate actions.
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
            
            //populate all records with timestamp fields from corp actions response
            var timeStarted = new DateTimeOffset(getActionsResponse.timestarted.ToUniversalTime(), TimeSpan.Zero).ToString(DefaultCultureInfo);
            var timeFinished = new DateTimeOffset(getActionsResponse.timefinished.ToUniversalTime(), TimeSpan.Zero).ToString(DefaultCultureInfo);
            corpActionRecords.ForEach(r =>
            {
                r.Add(TimeStarted, timeStarted);
                r.Add(TimeFinished, timeFinished);
            });

            // retrieve actions for all instruments or if failed then return empty output
            return corpActionRecords.Any()
                ? new DataLicenseOutput(corpActionOutputId, headers, corpActionRecords)
                : DataLicenseOutput.Empty(corpActionOutputId);
        }

    }
}