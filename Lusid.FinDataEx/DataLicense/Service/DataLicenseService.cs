﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lusid.FinDataEx.DataLicense.Service.Call;
using Lusid.FinDataEx.DataLicense.Service.Transform;
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx.DataLicense.Service
{
    /// <summary>
    /// Service that manages the lifecycle of all calls to BBG DLWS. Includes request
    /// construction, call execution and waiting, and transformation of response data
    /// into a standard FinDataOutput.
    /// 
    /// </summary>
    public class DataLicenseService
    {
        
        public const int DataNotAvailable = 100;
        public const int Success = 0;
        public const int RequestError = 200;
        public const string InstrumentSuccessCode = "0";
        public const int NoCorpActionCode = 300;

        /// <summary>
        ///  Executes a request to BBG DLWS for a given set of BB_UNIQUE_IDs and a given data type. On response
        /// will return a standardised output as a FinDataOutput.
        /// 
        /// </summary>
        /// <param name="dataLicenseCall">Data type call to BBG DL (e.g. GetData, GetAction)</param>
        /// <param name="dlInstruments">DLWS representation of instruments to pass to BBG DL</param>
        /// <param name="programType">Program type of the given call (e.g. Adhoc, Scheduled)</param>
        /// <returns>FinDataOutput of data returned for instruments requested</returns>
        public List<FinDataOutput> Get(IDataLicenseCall<PerSecurityResponse> dataLicenseCall, Instruments dlInstruments, ProgramTypes programType)
        {
            // validate inputs
            if(!dlInstruments.instrument.Any()) return new List<FinDataOutput>();
            VerifyBblFlags(programType, dataLicenseCall.GetDataLicenseDataType());
            
            // create relevant action and call to DLWS
            var perSecurityResponse = dataLicenseCall.Get(dlInstruments);
            
            // parse and transform dl response to standard output
            var finDataOutputs = TransformBbgResponse(perSecurityResponse, dataLicenseCall.GetDataLicenseDataType());
            return finDataOutputs;
        }

        private List<FinDataOutput> TransformBbgResponse(PerSecurityResponse perSecurityResponse, DataTypes dataType)
        {
            return dataType switch
            {
                DataTypes.GetData => new GetDataResponseTransformer().Transform(
                    (RetrieveGetDataResponse) perSecurityResponse),
                _ => throw new NotSupportedException($"{dataType} is not a currently supported BBG Data type.")
            };
        }

        private void VerifyBblFlags(ProgramTypes programType, DataTypes dataType)
        {
            if (programType != ProgramTypes.Adhoc)
            {
                throw new NotSupportedException($"Only {ProgramTypes.Adhoc} program types are currently allowed.");
            }
        }

    }
}