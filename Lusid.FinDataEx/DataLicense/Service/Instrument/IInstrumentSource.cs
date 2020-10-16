using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    public interface IInstrumentSource
    {
        Instruments Get();
    }
}