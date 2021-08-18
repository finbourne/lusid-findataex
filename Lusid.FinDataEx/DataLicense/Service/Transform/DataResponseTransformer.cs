using System;
using System.Collections.Generic;
using System.Linq;
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseConstants;

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
        /// <param name="getDataResponse">GetData response from BBG DLWS</param>
        /// <returns>FinDataOutput of data returned for instruments requested</returns>
        public DataLicenseOutput Transform(RetrieveGetDataResponse getDataResponse)
        {
            var finDataOutputId = getDataResponse.responseId;
            // construct data headers
            var headers = new List<string>(){TimeStarted, TimeFinished}; 
            headers.AddRange(getDataResponse.fields.ToList());
            
            // setup data records
            var finDataRecords = new List<Dictionary<string, string>>();
            var instrumentDatas = getDataResponse.instrumentDatas;
            foreach (var instrumentData in instrumentDatas)
            {
                var instrumentRecord = new Dictionary<string, string>();
                // errors for specific instruments should be logged only and not impact the rest of the batch
                if (instrumentData.code != DataLicenseService.InstrumentSuccessCode)
                {
                    Console.WriteLine($"Error in GetData instrument for {instrumentData.instrument.id}. " +
                                      "Check GetData response log above. Continuing to remaining instruments...");
                    continue;
                }

                // populate instrument record map for each instrument response
                for (var i = 0; i < getDataResponse.fields.Length; i++)
                {
                    instrumentRecord.Add(getDataResponse.fields[i], instrumentData.data[i].value);
                }
                finDataRecords.Add(instrumentRecord);
            }
            
            //populate all records with timestamp fields from data response
            var timeStarted = new DateTimeOffset(getDataResponse.timestarted.ToUniversalTime(), TimeSpan.Zero).ToString(DefaultCultureInfo);
            var timeFinished = new DateTimeOffset(getDataResponse.timefinished.ToUniversalTime(), TimeSpan.Zero).ToString(DefaultCultureInfo);
            finDataRecords.ForEach(r =>
            {
                r.Add(TimeStarted, timeStarted);
                r.Add(TimeFinished, timeFinished);
            });

            // retrieve data for all instruments or if failed then return empty output
            return finDataRecords.Any()
                ? new DataLicenseOutput(finDataOutputId, headers, finDataRecords)
                : DataLicenseOutput.Empty(finDataOutputId);
        }
        
    }
}