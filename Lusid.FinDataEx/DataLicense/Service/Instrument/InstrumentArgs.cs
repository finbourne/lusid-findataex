using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    /// <summary>
    ///  Configuration required when constructing DLWS insturments for submission. Main
    ///  users will be implementations of instrument sources
    ///
    ///  Any new functionality offering around instrument configurations should be
    ///  placed in here. 
    /// </summary>
    public class InstrumentArgs
    {
        // mandatory instrument arguments
        public InstrumentType InstrumentType { get; }
        
        // optional instrument arguments
        public MarketSector? YellowKey { get; }

        private InstrumentArgs(InstrumentType instrumentType, MarketSector? yellowKey)
        {
            InstrumentType = instrumentType;
            YellowKey = yellowKey;
        }
        public static InstrumentArgs Create(DataLicenseOptions dataLicenseOptions)
        {
            return new InstrumentArgs(dataLicenseOptions.InstrumentIdType, dataLicenseOptions.YellowKey);
        }

        public static InstrumentArgs Create(InstrumentType instrumentType)
        {
            return new InstrumentArgs(instrumentType, null);
        }
        
    }
    
}