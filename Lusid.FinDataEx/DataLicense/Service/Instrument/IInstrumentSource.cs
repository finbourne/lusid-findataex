using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    /// <summary>
    /// A source of BBG DL instruments typically used for querying
    /// BBG DL with.
    /// 
    /// </summary>
    public interface IInstrumentSource
    {
        /// <summary>
        ///  Retrieve a set of instruments using the BBG DLWS representation.
        /// </summary>
        /// <returns></returns>
        #nullable enable
        Instruments? Get();
    }
}