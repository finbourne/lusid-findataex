using System.Collections.Generic;
using System.Linq;
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
        
        /// <summary>
        ///  Default implementation to construct DL instruments from a set of instrument ids and a given instrument type.
        /// </summary>
        /// <param name="instrumentArgs">Configuration for the instrument request to DLWS (InsturmentIdType (e.g. Ticker), YellowKey (e.g. Curncy), etc...)</param>
        /// <param name="instrumentIds">Set of instrument ids to create and configure for submission to DLWS</param>
        /// <returns></returns>
        internal static Instruments CreateInstruments(InstrumentArgs instrumentArgs, IEnumerable<string> instrumentIds)
        {
            var instruments = instrumentIds.ToHashSet().Select(id =>
            {
                // add mandatory instrument args
                var instrument = new PerSecurity_Dotnet.Instrument()
                {
                    id = id,
                    type = instrumentArgs.InstrumentType,
                    typeSpecified = true
                };

                // add optional instrument args
                if (instrumentArgs.YellowKey.HasValue)
                {
                    instrument.yellowkeySpecified = true;
                    instrument.yellowkey = instrumentArgs.YellowKey.Value;
                }
                return instrument;
            }).ToArray();
            
            return new Instruments()
            {
                instrument = instruments
            };
        }
    }
}