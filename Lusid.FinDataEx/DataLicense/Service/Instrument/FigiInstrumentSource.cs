using System.Collections.Generic;
using System.Linq;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    public class FigiInstrumentSource : IInstrumentSource
    {
        private readonly ISet<string> figis;

        public FigiInstrumentSource(ISet<string> figis)
        {
            this.figis = figis;
        }

        public Instruments? Get()
        {
            return CreateInstruments(figis);
        }

        private Instruments CreateInstruments(IEnumerable<string> bbgIds)
        {
            var instruments = bbgIds.Select(id => new PerSecurity_Dotnet.Instrument()
            {
                id = id,
                type = InstrumentType.BB_GLOBAL,
                typeSpecified = true
            }).ToArray();
            return new Instruments()
            {
                instrument = instruments
            };
        }
    }
}