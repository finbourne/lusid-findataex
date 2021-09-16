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

        private readonly InstrumentArgs _instrumentArgs;
        private readonly IEnumerable<string> _instrumentSourceArgs;
        private readonly IFileHandler _fileHandler;

        public FileInstrumentSource(DataLicenseOptions dataOptions, IFileHandler fileHandler)
        {
            _instrumentArgs = _instrumentArgs = InstrumentArgs.Create(dataOptions);
            _instrumentSourceArgs = dataOptions.InstrumentSourceArguments;
            _fileHandler = fileHandler;
        }

        #nullable enable
        public Instruments? Get()
        {
            var (filePath, delimiter, instrumentIdColIdx) = ParseInstrumentSourceArgs(_instrumentArgs.InstrumentType, _instrumentSourceArgs);
            IEnumerable<string> instrumentIds = LoadInstrumentsFromFile(filePath, delimiter, instrumentIdColIdx);
            return IInstrumentSource.CreateInstruments(_instrumentArgs, instrumentIds);
        }

        private static Tuple<string, string, int> ParseInstrumentSourceArgs(InstrumentType instrumentType, IEnumerable<string> instrumentSourceArgs)
        {
            var sourceArgs = instrumentSourceArgs as string[] ?? instrumentSourceArgs.ToArray();
            if (!sourceArgs.Any())
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

        private IEnumerable<string> LoadInstrumentsFromFile(string filepath, string delimiter, int instrumentIdColIdx)
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