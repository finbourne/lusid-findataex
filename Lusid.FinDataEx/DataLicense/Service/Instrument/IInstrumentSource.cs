using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    public interface IInstrumentSource
    {
        #nullable enable
        Instruments? Get();
    }
}