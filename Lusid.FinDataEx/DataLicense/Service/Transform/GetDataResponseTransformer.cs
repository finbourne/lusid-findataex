using System;
using System.Collections.Generic;
using System.Linq;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Transform
{
    public class GetDataResponseTransformer : IBbgResponseTransformer<RetrieveGetDataResponse>
    {
        public List<FinDataOutput> Transform(RetrieveGetDataResponse perSecurityResponse)
        {
            List<string> headers = perSecurityResponse.fields.ToList();
            List<Dictionary<string,string>> finDataRecords = new List<Dictionary<string, string>>(); 
            InstrumentData[] instrumentDatas = perSecurityResponse.instrumentDatas;
            foreach (var instrumentData in instrumentDatas)
            {
                // instruments with errors should be logged and ignored
                if (instrumentData.code != DLDataService.InstrumentSuccessCode)
                {
                    Console.WriteLine($"Error in GetData instrument for {instrumentData.instrument.id}. " +
                                      $"Check GetData response log above. Continuing to remaining instruments...");
                    continue;
                }
                Dictionary<string,string> instrumentRecord = new Dictionary<string, string>();
                for (var i = 0; i < perSecurityResponse.fields.Length; i++)
                {
                    instrumentRecord.Add(perSecurityResponse.fields[i], instrumentData.data[i].value);
                }
                finDataRecords.Add(instrumentRecord);
            }   
            FinDataOutput getDataOutput = new FinDataOutput(headers, finDataRecords);
            return new List<FinDataOutput>{getDataOutput};
        }
        
    }
}