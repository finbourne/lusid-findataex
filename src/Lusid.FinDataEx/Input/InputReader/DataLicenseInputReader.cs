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
        private readonly IDataLicenseService _dataLicenseService;
        private readonly IPerSecurityWsFactory _perSecurityWsFactory;

        public DataLicenseInputReader(DataLicenseOptions getOptions, Instruments instruments, IDataLicenseService dataLicenseService, IPerSecurityWsFactory perSecurityWsFactory)
        {
            _getOptions = getOptions;
            _instruments = instruments;
            _dataLicenseService = dataLicenseService;
            _perSecurityWsFactory = perSecurityWsFactory;
        }

        public DataLicenseOutput Read()
        {
            var perSecurityWs = _perSecurityWsFactory.CreateDefault();
            var dataLicenseCall = CreateDataLicenseCall(_getOptions, perSecurityWs);

            LogRequest(_instruments, dataLicenseCall);

            return _dataLicenseService.Get(dataLicenseCall, _instruments, ProgramTypes.Adhoc, _getOptions.EnableLiveRequests);
        }

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