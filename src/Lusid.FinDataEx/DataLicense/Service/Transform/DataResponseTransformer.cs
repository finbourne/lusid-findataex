using System;
using System.Collections.Generic;
using System.Linq;
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseConstants;

namespace Lusid.FinDataEx.DataLicense.Service.Transform
{
    public class GetDataResponseTransformer : IResponseTransformer
    {
        public List<Dictionary<string, string>> Transform(PerSecurityResponse perSecurityResponse)
        {
            var getDataResponse = perSecurityResponse as RetrieveGetDataResponse;

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

            return finDataRecords;
        }
    }
}