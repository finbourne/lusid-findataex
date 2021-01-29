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
        /// <param name="instrumentType"></param>
        /// <param name="instrumentIds"></param>
        /// <returns></returns>
        internal static Instruments CreateInstruments(InstrumentType instrumentType, IEnumerable<string> instrumentIds)
        {
            var instruments = instrumentIds.Select(id => new PerSecurity_Dotnet.Instrument()
            {
                id = id,
                type = instrumentType,
                typeSpecified = true
            }).ToArray();
            return new Instruments()
            {
                instrument = instruments
            };
        }
    }
}