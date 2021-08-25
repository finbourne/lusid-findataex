using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Model;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Util;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    public class DriveCsvInstrumentSource : IInstrumentSource
    {
        private readonly IFilesApi _filesApi;
        private readonly ISearchApi _searchApi;

        private readonly InstrumentArgs _instrumentArgs;
        private readonly string _filePath;
        private readonly string _delimiter;
        private readonly int _instrumentIdColIdx;

        private DriveCsvInstrumentSource(IFilesApi filesApi, ISearchApi searchApi, InstrumentArgs instrumentArgs, string filePath, string delimiter, int instrumentIdColIdx)
        {
            _filesApi = filesApi;
            _searchApi = searchApi;

            _instrumentArgs = instrumentArgs;
            _filePath = filePath;
            _delimiter = delimiter;
            _instrumentIdColIdx = instrumentIdColIdx;
        }

        /// <summary>
        ///  Creates an instrument source for a given instrument id type and a set of
        ///  instrument ids.
        /// </summary>
        /// <param name="instrumentArgs">Configuration for the instrument request to DLWS (InsturmentIdType (e.g. Ticker), YellowKey (e.g. Curncy), etc...)</param>
        /// <param name="instrumentSourceArgs">Application arguments passed in. LUSID filepath (mandatory), delimiter (optional) and column number of the instrument id (optional)</param>
        /// <returns>A DriveCsvInstrumentSource instance</returns>
        public static DriveCsvInstrumentSource Create(ILusidApiFactory factory, InstrumentArgs instrumentArgs, IEnumerable<string> instrumentSourceArgs)
        {
            var filesApi = factory.Api<IFilesApi>();
            var searchApi = factory.Api<ISearchApi>();
            
            var (filePath, delimiter, instrumentIdColIdx) = ParseInstrumentSourceArgs(instrumentArgs.InstrumentType, instrumentSourceArgs);
            return new DriveCsvInstrumentSource(filesApi, searchApi, instrumentArgs, filePath, delimiter, instrumentIdColIdx); 
        }

        private IEnumerable<string> LoadInstrumentsFromFile(string filePath, string delimiter, int instrumentIdColIdx)
        {
            var (directoryName, fileName) = LusidDriveUtils.PathToFolderAndFile(filePath);
            
            // Retrieve LUSID drive file id from path
            var fileId = _searchApi.Search(new SearchBody(directoryName, fileName)).Values.SingleOrDefault()?.Id;
            if (fileId == null)
                throw new FileNotFoundException($"{filePath} not found.");
            
            // Download file and feed into a string via stream
            IList<string> csvEntries = new List<string>();
            using (var csvDriveReader = new StreamReader(_filesApi.DownloadFile(fileId)))
            {
                string entry;
                while((entry = csvDriveReader.ReadLine()) != null)
                {
                    csvEntries.Add(entry);
                }
            };

            // precondition is instrument source file have headers so should be ignored
            // split entries by delimiter and return the id from provided col index.
            return csvEntries.Skip(1)
                .Select(e => e.Split(delimiter)[instrumentIdColIdx])
                .ToHashSet();
        }

        /// <summary>
        /// Retrieve the instrument source filepath, delimiter used and the column index of the instrument Id. Additionally apply
        /// any AutoGen patterns to the file name (e.g. to insert the AsOf date into the filename). 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        ///  Retrieve a BBG DLWS set of instruments loaded from csv file.
        /// </summary>
        /// <returns>Set of BBG DLWS instruments</returns>
#nullable enable
        public Instruments? Get()
        {
            IEnumerable<string> instrumentIds = LoadInstrumentsFromFile(_filePath, _delimiter, _instrumentIdColIdx);
            return IInstrumentSource.CreateInstruments(_instrumentArgs, instrumentIds);
        }
    }
}