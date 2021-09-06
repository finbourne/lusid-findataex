using Lusid.FinDataEx.Input;
using System;
using System.Linq;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Call;
using Lusid.FinDataEx.DataLicense.Vendor;
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx
{
    public class DataLicenseInputReader : IInputReader
    {
        private readonly DataLicenseOptions _getOptions;
        private readonly Instruments _instruments;

        public DataLicenseInputReader(DataLicenseOptions getOptions, Instruments instruments)
        {
            _getOptions = getOptions;
            _instruments = instruments;
        }

        public DataLicenseOutput Read()
        {
            // construct data license call
            var perSecurityWs = new PerSecurityWsFactory().CreateDefault();
            var dataLicenseCall = CreateDataLicenseCall(_getOptions, perSecurityWs);

            LogRequest(_instruments, dataLicenseCall);

            // call DL and write results to specified output (as long as not in safe mode)
            var dlDataService = new DataLicenseService();
            return dlDataService.Get(dataLicenseCall, _instruments, ProgramTypes.Adhoc, _getOptions.EnableLiveRequests);
        }

        /// <summary>
        ///  Create a BBG DL call depending on the arguments passed into the application.
        /// </summary>
        /// <param name="getOptions">Options taken from the user provided arguments</param>
        /// <param name="perSecurityWs">BBG DLWS client</param>
        /// <returns></returns>
        private static IDataLicenseCall<PerSecurityResponse> CreateDataLicenseCall(DataLicenseOptions getOptions, PerSecurityWS perSecurityWs)
        {
            return getOptions switch
            {
                GetActionsOptions getActionsOptions => new GetActionsDataLicenseCall(perSecurityWs,
                    getActionsOptions.CorpActionTypes.ToList()),
                GetDataOptions getDataOptions => new GetDataLicenseCall(perSecurityWs,
                    getDataOptions.DataFields.ToArray()),
                _ => throw new ArgumentOutOfRangeException(nameof(getOptions))
            };
        }

        private static void LogRequest(Instruments instruments, IDataLicenseCall<PerSecurityResponse> dataLicenseCall)
        {
            var instrumentsAndTypes = string.Join(",", instruments.instrument.Select(i => $"{i.type}={i.id}"));
            Console.WriteLine($"Preparing a {dataLicenseCall.GetDataType()} call for instruments : {instrumentsAndTypes}");
        }
    }
}