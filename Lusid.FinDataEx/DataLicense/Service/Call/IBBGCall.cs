using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service
{
    public interface IBbgCall<out T> where T : PerSecurityResponse 
    {
        T Get(Instruments instruments);
    }
}