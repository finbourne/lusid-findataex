using Lusid.FinDataEx.Input;
using System;
using System.Linq;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Call;
using Lusid.FinDataEx.DataLicense.Vendor;
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;
using static Lusid.FinDataEx.Input.IInputReader;
using Lusid.FinDataEx.DataLicense.Service.Transform;

namespace Lusid.FinDataEx
{
    public class DataLicenseInputReader : IInputReader
    {
        private readonly DataLicenseOptions _getOptions;
        private readonly Instruments _instruments;
        private readonly IDataLicenseService _dataLicenseService;
        private readonly IPerSecurityWsFactory _perSecurityWsFactory;
        private readonly ITransformerFactory _transformerFactory;
        private const string ActionTypeHeaderString = "Action Type";

        public DataLicenseInputReader(DataLicenseOptions getOptions, Instruments instruments, IDataLicenseService dataLicenseService, IPerSecurityWsFactory perSecurityWsFactory, ITransformerFactory transformerFactory)
        {
            _getOptions = getOptions;
            _instruments = instruments;
            _dataLicenseService = dataLicenseService;
            _perSecurityWsFactory = perSecurityWsFactory;
            _transformerFactory = transformerFactory;
        }

        public DataLicenseOutput Read()
        {
            var perSecurityWs = _perSecurityWsFactory.CreateDefault();
            var dataLicenseCall = CreateDataLicenseCall(_getOptions, perSecurityWs);

            LogRequest(_instruments, dataLicenseCall);

            var perSecurityResponse = _dataLicenseService.Get(dataLicenseCall, _instruments, ProgramTypes.Adhoc, _getOptions.EnableLiveRequests);
            var transformer = _transformerFactory.Build(dataLicenseCall.GetDataType());

            var rawRecords = transformer.Transform(perSecurityResponse);
            var records = rawRecords.Select(r => ConvertToRecord(r, _getOptions, ActionTypeHeaderString)).ToList();

            return new DataLicenseOutput(perSecurityResponse.requestId, records);
        }

        private static IDataLicenseCall<PerSecurityResponse> CreateDataLicenseCall(DataLicenseOptions getOptions, PerSecurityWS perSecurityWs) => getOptions switch
        {
            GetActionsOptions getActionsOptions => new GetActionsDataLicenseCall(perSecurityWs, getActionsOptions.CorpActionTypes.ToList()),
            GetDataOptions getDataOptions => new GetDataLicenseCall(perSecurityWs, getDataOptions.DataFields.ToArray()),
            _ => throw new NotSupportedException($"{nameof(getOptions)} is not a known request type.")
        };

        private static void LogRequest(Instruments instruments, IDataLicenseCall<PerSecurityResponse> dataLicenseCall)
        {
            var instrumentsAndTypes = string.Join(",", instruments.instrument.Select(i => $"{i.type}={i.id}"));
            Console.WriteLine($"Preparing a {dataLicenseCall.GetDataType()} call for instruments : {instrumentsAndTypes}");
        }
    }
}