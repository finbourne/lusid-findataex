using Lusid.FinDataEx.DataLicense.Service.Call;
using Lusid.FinDataEx.DataLicense.Util;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service
{
    public interface IDataLicenseService
    {
        PerSecurityResponse Get(IDataLicenseCall<PerSecurityResponse> dataLicenseCall, Instruments dlInstruments, DataLicenseTypes.ProgramTypes programType, bool enableLiveRequests = false);
    }
}