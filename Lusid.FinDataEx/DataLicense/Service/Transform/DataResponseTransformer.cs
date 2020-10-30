using System;
using System.Collections.Generic;
using System.Linq;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Transform
{
    /// <summary>
    ///  Transformer for BBG DL GetData Calls.
    ///
    /// </summary>
    public class GetDataResponseTransformer : IDataLicenseResponseTransformer<RetrieveGetDataResponse>
    {
        /// <summary>
        /// Transform a a GetData response from BBG DLWS to FinDataOutput. Typically expect
        /// only one set of FinDataOutput to be returned.
        /// 
        /// </summary>
        /// <param name="perSecurityResponse">GetData response from BBG DLWS</param>
        /// <returns>FinDataOutput of data returned for instruments requested</returns>
        public DataLicenseOutput Transform(RetrieveGetDataResponse perSecurityResponse)
        {
            var finDataOutputId = $"{perSecurityResponse.responseId}_GetData";
            // construct data headers
            var headers = new List<string>(){"timeStarted", "timeFinished"}; 
            headers.AddRange(perSecurityResponse.fields.ToList());
            
            // setup data records
            var finDataRecords = new List<Dictionary<string, string>>();
            var instrumentDatas = perSecurityResponse.instrumentDatas;
            foreach (var instrumentData in instrumentDatas)
            {
                Dictionary<string,string> instrumentRecord = new Dictionary<string, string>();
                // instruments with errors should be logged and ignored
                if (instrumentData.code != DataLicenseService.InstrumentSuccessCode)
                {
                    Console.WriteLine($"Error in GetData instrument for {instrumentData.instrument.id}. " +
                                      $"Check GetData response log above. Continuing to remaining instruments...");
                    continue;
                }

                for (var i = 0; i < perSecurityResponse.fields.Length; i++)
                {
                    instrumentRecord.Add(perSecurityResponse.fields[i], instrumentData.data[i].value);
                }
                finDataRecords.Add(instrumentRecord);
            }
            
            //populate all records with timestamp fields
            var timeStarted = new DateTimeOffset(perSecurityResponse.timestarted.ToUniversalTime(), TimeSpan.Zero).ToString();
            var timeFinished = new DateTimeOffset(perSecurityResponse.timefinished.ToUniversalTime(), TimeSpan.Zero).ToString();
            finDataRecords.ForEach(r =>
            {
                r.Add("timeStarted", timeStarted);
                r.Add("timeFinished", timeFinished);
            });

            return finDataRecords.Any()
                ? new DataLicenseOutput(finDataOutputId, headers, finDataRecords)
                : DataLicenseOutput.Empty(finDataOutputId);
        }
        
    }
}