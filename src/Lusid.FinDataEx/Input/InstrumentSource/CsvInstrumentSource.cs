﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lusid.FinDataEx.Util;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    /// <summary>
    ///  Instruments loaded from csv on local filesystem
    /// </summary>
    public class CsvInstrumentSource : IInstrumentSource
    {
        private readonly InstrumentArgs _instrumentArgs;
        private readonly string _filePath;
        private readonly string _delimiter;
        private readonly int _instrumentIdColIdx;
        

        protected CsvInstrumentSource(InstrumentArgs instrumentArgs, string filePath, string delimiter, int instrumentIdColIdx)
        {
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
        /// <param name="instrumentSourceArgs">Application arguments passed in. Filepath (mandatory), delimiter (optional) and column index of the instrument id (optional)</param>
        /// <returns>A CsvInstrumentSource instance</returns>
        public static CsvInstrumentSource Create(InstrumentArgs instrumentArgs, IEnumerable<string> instrumentSourceArgs)
        {
            var (filePath, delimiter, instrumentIdColIdx) = ParseInstrumentSourceArgs(instrumentArgs.InstrumentType, instrumentSourceArgs);
            return new CsvInstrumentSource(instrumentArgs, filePath, delimiter, instrumentIdColIdx);
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

        protected virtual IEnumerable<string> LoadInstrumentsFromFile(string filepath, string delimiter, int instrumentIdColIdx)
        {
            return File.ReadAllLines(filepath)
                .Skip(1)
                .Select(e => e.Split(delimiter)[instrumentIdColIdx])
                .ToHashSet();
        }
    }
}