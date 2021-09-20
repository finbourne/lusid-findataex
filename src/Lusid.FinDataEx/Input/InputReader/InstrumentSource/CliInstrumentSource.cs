using System.Collections.Generic;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    public class CliInstrumentSource : IInstrumentSource
    {
        private readonly InstrumentArgs _instrumentArgs;
        private readonly IEnumerable<string> _instrumentIds;

        public CliInstrumentSource(DataLicenseOptions dataOptions)
        {
            _instrumentArgs = InstrumentArgs.Create(dataOptions);
            _instrumentIds = dataOptions.InstrumentSourceArguments;
        }

        #nullable enable
        public Instruments? Get() => IInstrumentSource.CreateInstruments(_instrumentArgs, _instrumentIds);
    }
}