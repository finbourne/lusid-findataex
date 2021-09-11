using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Vendor
{
    public interface IPerSecurityWsFactory
    {
        PerSecurityWS CreateDefault();
        PerSecurityWS CreateDefault(string bbgDlAddress);
        PerSecurityWS CreateDefault(string bbgDlAddress, string bbgDlCert, string bbgDlPass);
    }
}