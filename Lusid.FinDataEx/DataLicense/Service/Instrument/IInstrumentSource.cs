using System;
using System.Collections.Generic;
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
        ///  Creates an instrument source for a given instrument id type (e.g. ISIN, FIGI) and a set of defined
        ///  string arguments included as part of the request.
        /// </summary>
        /// <returns></returns>
        static IInstrumentSource Create(InstrumentType instrumentType, IEnumerable<string> instrumentSourceArgs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Retrieve a set of instruments using the BBG DLWS representation.
        /// </summary>
        /// <returns></returns>
        #nullable enable
        Instruments? Get();
    }
}