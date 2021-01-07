using System.Collections.Generic;
using System.Linq;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    /// <summary>
    ///  Instrument source based on a provided set of instrument identifiers of a given type.
    /// </summary>
    public class BasicInstrumentSource : IInstrumentSource
    {
        private readonly InstrumentType _instrumentType;
        private readonly ISet<string> _instrumentIds;

        public BasicInstrumentSource(InstrumentType instrumentType, ISet<string> instrumentIds)
        {
            _instrumentType = instrumentType;
            _instrumentIds = instrumentIds;
        }

        /// <summary>
        ///  Retrieve a BBG DLWS set of instruments based on the instrument ids and id type provided on constructing
        /// the source.
        /// </summary>
        /// <returns>Set of BBG DLWS instruments</returns>
        #nullable enable
        public Instruments? Get()
        {
            return CreateInstruments(_instrumentIds);
        }

        private Instruments CreateInstruments(IEnumerable<string> instrumentIds)
        {
            var instruments = instrumentIds.Select(id => new PerSecurity_Dotnet.Instrument()
            {
                id = id,
                type = _instrumentType,
                typeSpecified = true
            }).ToArray();
            return new Instruments()
            {
                instrument = instruments
            };
        }
    }
}