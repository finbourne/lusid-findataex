using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    /// <summary>
    ///  Instruments loaded from csv on local filesystem
    /// </summary>
    public class CsvInstrumentSource : IInstrumentSource
    {
        private readonly InstrumentType _instrumentType;
        private readonly string _filePath;
        private readonly string _delimiter;
        private readonly int _instrumentIdColIdx;
        

        protected CsvInstrumentSource(InstrumentType instrumentType, string filePath, string delimiter, int instrumentIdColIdx)
        {
            _instrumentType = instrumentType;
            _filePath = filePath;
            _delimiter = delimiter;
            _instrumentIdColIdx = instrumentIdColIdx;
        }
        
        /// <summary>
        ///  
        /// </summary>
        /// <param name="instrumentType"></param>
        /// <param name="instrumentSourceArgs">Application arguments passed in. Filepath (mandatory), delimitter (optional) and column number of the instrument id (optional)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static CsvInstrumentSource Create(InstrumentType instrumentType, IEnumerable<string> instrumentSourceArgs)
        {
            var (filePath, delimiter, instrumentIdColIdx) = ParseInstrumentSourceArgs(instrumentType, instrumentSourceArgs);
            return new CsvInstrumentSource(instrumentType, filePath, delimiter, instrumentIdColIdx);
        }

        internal static Tuple<string, string, int> ParseInstrumentSourceArgs(InstrumentType instrumentType, IEnumerable<string> instrumentSourceArgs)
        {
            var sourceArgs = instrumentSourceArgs as string[] ?? instrumentSourceArgs.ToArray();
            if (sourceArgs.Length < 1)
            {
                throw new ArgumentException(
                    $"CSV based instrument source must at least have a filepath provided to load instrument ids.");
            }
            var filePath = sourceArgs[0];
            var delimiter = sourceArgs.ElementAtOrDefault(1) ?? ",";
            Int32.TryParse(sourceArgs.ElementAtOrDefault(2) ?? "0", out var instrumentIdColIdx);
            
            Console.WriteLine($"Creating a instrument source to load instruments of type {instrumentType} " +
                              $"from local file system at {filePath}. Ids source from column index {instrumentIdColIdx} " +
                              $" with delimiter {delimiter}");
            
            return Tuple.Create(filePath, delimiter, instrumentIdColIdx);
        }

        /// <summary>
        ///  Retrieve a BBG DLWS set of instruments loaded from csv file.
        /// </summary>
        /// <returns>Set of BBG DLWS instruments</returns>
        #nullable enable
        public Instruments? Get()
        {
            IEnumerable<string> instrumentIds = LoadInstrumentsFromFile(_filePath, _delimiter, _instrumentIdColIdx);
            return CreateInstruments(instrumentIds);
        }

        protected virtual IEnumerable<string> LoadInstrumentsFromFile(string filepath, string delimiter, int instrumentIdColIdx)
        {
            return File.ReadAllLines(filepath)
                .Skip(1)
                .Select(e => e.Split(delimiter)[instrumentIdColIdx])
                .ToHashSet();
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