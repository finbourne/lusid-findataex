using System;
using System.Collections.Generic;
using System.Linq;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    /// <summary>
    ///  Instrument source based on a provided set of instrument identifiers of a given type.
    /// </summary>
    public class InstrumentSource : IInstrumentSource
    {
        private readonly InstrumentType _instrumentType;
        private readonly ISet<string> _instrumentIds;

        private InstrumentSource(InstrumentType instrumentType, ISet<string> instrumentIds)
        {
            _instrumentType = instrumentType;
            _instrumentIds = instrumentIds;
        }

        /// <summary>
        ///  Creates an instrument source for a given instrument id type and a set of
        ///  instrument ids.
        /// </summary>
        /// <param name="instrumentType">Instrument id types (e.g. BB_GLOBAL (FIGI), ISIN, etc...)</param>
        /// <param name="instrumentSourceArgs">Set of instrument ids to create</param>
        /// <returns>An InstrumentSource instance</returns>
        public static InstrumentSource Create(InstrumentType instrumentType, IEnumerable<string> instrumentSourceArgs)
        {
            var instrumentIds = instrumentSourceArgs as string[] ?? instrumentSourceArgs.ToArray();
            Console.WriteLine($"Creating a basic instrument source using instrument id type {instrumentType} for the " +
                              $"instrument ids: {string.Join(',',instrumentIds)}");                
            return new InstrumentSource(instrumentType, new HashSet<string>(instrumentIds)); 
        }

        /// <summary>
        ///  Retrieve a BBG DLWS set of instruments based on the instrument ids and id type provided on constructing
        /// the source.
        /// </summary>
        /// <returns>Set of BBG DLWS instruments</returns>
        #nullable enable
        public Instruments? Get()
        {
            return IInstrumentSource.CreateInstruments(_instrumentType, _instrumentIds);
        }
    }
}