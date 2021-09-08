using System;
using System.Collections.Generic;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    public class CliInstrumentSource : IInstrumentSource
    {
        private readonly InstrumentArgs _instrumentArgs;
        private readonly IEnumerable<string> _instrumentIds;

        private CliInstrumentSource(InstrumentArgs instrumentArgs, IEnumerable<string> instrumentIds)
        {
            _instrumentArgs = instrumentArgs;
            _instrumentIds = instrumentIds;
        }

        public static CliInstrumentSource Create(InstrumentArgs instrumentArgs, IEnumerable<string> instrumentSourceArgs)
        {
            Console.WriteLine($"Creating a basic instrument source using instrument id type {instrumentArgs.InstrumentType} for the " +
                              $"instrument ids: {string.Join(',', instrumentSourceArgs)}");
            return new CliInstrumentSource(instrumentArgs, instrumentSourceArgs);
        }

        #nullable enable
        public Instruments? Get() => IInstrumentSource.CreateInstruments(_instrumentArgs, _instrumentIds);
    }
}