using System;
using System.Linq;
using Lusid.FinDataEx.DataLicense.Service.Call;
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx.DataLicense.Service
{
    /// <summary>
    /// Service that manages the lifecycle of all calls to BBG DLWS. Includes request
    /// construction, call execution and waiting, and transformation of response data
    /// into a standard FinDataOutput.
    /// </summary>
    public class DataLicenseService : IDataLicenseService
    {
        public const int DataNotAvailable = 100;
        public const int Success = 0;
        public const int RequestError = 200;
        public const string InstrumentSuccessCode = "0";

        /// <summary>
        ///  Executes a request to BBG DLWS for a given set of BB_UNIQUE_IDs and a given data type. On response
        /// will return a standardised output as a FinDataOutput.
        /// </summary>
        /// <param name="dataLicenseCall">Data type call to BBG DL (e.g. GetData, GetAction)</param>
        /// <param name="dlInstruments">DLWS representation of instruments to pass to BBG DL</param>
        /// <param name="programType">Program type of the given call (e.g. Adhoc, Scheduled)</param>
        /// <returns>FinDataOutput of data returned for instruments requested</returns>
        public PerSecurityResponse Get(IDataLicenseCall<PerSecurityResponse> dataLicenseCall, Instruments dlInstruments, ProgramTypes programType, bool enableLiveRequests = false)
        {
            if (enableLiveRequests)
            {
                // validate inputs
                if (!dlInstruments.instrument.Any())
                {
                    return new PerSecurityResponse();
                }

                VerifyBblFlags(programType);

                // create relevant action and call to DLWS
                return dataLicenseCall.Get(dlInstruments);
            }
            else
            {
                Console.WriteLine("--- SAFE MODE --- ");
                Console.WriteLine("As operating in SAFE mode no requests will be pushed to DLWS.");
                return new PerSecurityResponse();
            }

        }

        private static void VerifyBblFlags(ProgramTypes programType)
        {
            if (programType != ProgramTypes.Adhoc)
            {
                throw new NotSupportedException($"Only {ProgramTypes.Adhoc} program types are currently allowed.");
            }
        }
    }
}