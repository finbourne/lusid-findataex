﻿using System;
using System.Collections.Generic;
using System.Linq;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Transform
{
    /// <summary>
    ///  Transformer for BBG DL GetData Calls.
    ///
    /// </summary>
    public class GetDataResponseTransformer : IBbgResponseTransformer<RetrieveGetDataResponse>
    {
        /// <summary>
        /// Transform a a GetData response from BBG DLWS to FinDataOutput. Typically expect
        /// only one set of FinDataOutput to be returned.
        /// 
        /// </summary>
        /// <param name="perSecurityResponse">GetData response from BBG DLWS</param>
        /// <returns>FinDataOutput of data returned for instruments requested</returns>
        public List<FinDataOutput> Transform(RetrieveGetDataResponse perSecurityResponse)
        {
            var headers = perSecurityResponse.fields.ToList();
            var finDataRecords = new List<Dictionary<string, string>>(); 
            var instrumentDatas = perSecurityResponse.instrumentDatas;
            Dictionary<string,string> instrumentRecord = new Dictionary<string, string>();
            foreach (var instrumentData in instrumentDatas)
            {
                // instruments with errors should be logged and ignored
                if (instrumentData.code != DlDataService.InstrumentSuccessCode)
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

            var finDataOutputId = $"{perSecurityResponse.responseId}.GetData"; 
            var getDataOutput = new FinDataOutput(finDataOutputId, headers, finDataRecords);
            return new List<FinDataOutput>{getDataOutput};
        }
        
    }
}