using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lusid.FinDataEx.Util;
using Lusid.FinDataEx.Util.FileUtils.Handler;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    public class FileInstrumentSource : IInstrumentSource
    {
        private const char InputFileEntrySeparator = '\n';

        private readonly IFileHandler _fileHandler;
        private readonly InstrumentArgs _instrumentArgs;
        private readonly string _filePath;
        private readonly string _delimiter;
        private readonly int _instrumentIdColIdx;

        protected FileInstrumentSource(IFileHandler fileHandler, InstrumentArgs instrumentArgs, string filePath, string delimiter, int instrumentIdColIdx)
        {
            _fileHandler = fileHandler;
            _instrumentArgs = instrumentArgs;
            _filePath = filePath;
            _delimiter = delimiter;
            _instrumentIdColIdx = instrumentIdColIdx;
        }

        public static FileInstrumentSource Create(IFileHandler fileHandler, InstrumentArgs instrumentArgs, IEnumerable<string> instrumentSourceArgs)
        {
            var (filePath, delimiter, instrumentIdColIdx) = ParseInstrumentSourceArgs(instrumentArgs.InstrumentType, instrumentSourceArgs);
            return new FileInstrumentSource(fileHandler, instrumentArgs, filePath, delimiter, instrumentIdColIdx);
        }

        internal static Tuple<string, string, int> ParseInstrumentSourceArgs(InstrumentType instrumentType, IEnumerable<string> instrumentSourceArgs)
        {
            // retrieve filename, delimiter and instrument column index. 
            var sourceArgs = instrumentSourceArgs as string[] ?? instrumentSourceArgs.ToArray();
            if (sourceArgs.Length < 1)
            {
                throw new ArgumentException(
                    $"CSV based instrument source must at least have a filepath provided to load instrument ids.");
            }
            var filePath = sourceArgs[0];
            var delimiter = sourceArgs.ElementAtOrDefault(1) ?? ",";
            int.TryParse(sourceArgs.ElementAtOrDefault(2) ?? "0", out var instrumentIdColIdx);
            
            // apply AutoGen patterns on filename
            filePath = AutoGenPatternUtils.ApplyDateTimePatterns(filePath);
            
            Console.WriteLine($"Creating a instrument source to load instruments of type {instrumentType} " +
                              $"from {filePath}. Ids source from column index {instrumentIdColIdx} " +
                              $" with delimiter {delimiter}");
            
            return Tuple.Create(filePath, delimiter, instrumentIdColIdx);
        }

        #nullable enable
        public Instruments? Get()
        {
            IEnumerable<string> instrumentIds = LoadInstrumentsFromFile(_filePath, _delimiter, _instrumentIdColIdx);
            return IInstrumentSource.CreateInstruments(_instrumentArgs, instrumentIds);
        }

        protected virtual IEnumerable<string> LoadInstrumentsFromFile(string filepath, string delimiter, int instrumentIdColIdx)
        {
            var validatedPath = _fileHandler.ValidatePath(filepath);
            if (string.IsNullOrWhiteSpace(validatedPath))
            {
                throw new FileNotFoundException($"Local file '{filepath}' not found.");
            }

            return _fileHandler.Read(validatedPath, InputFileEntrySeparator)
                .Skip(1)
                .Select(e => e.Split(delimiter)[instrumentIdColIdx])
                .ToHashSet();
        }
    }
}