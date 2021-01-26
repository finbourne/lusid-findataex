using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Model;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Output;
using Lusid.FinDataEx.Util;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    public class DriveCsvInstrumentSource : CsvInstrumentSource
    {
        private readonly IFilesApi _filesApi;
        private readonly ISearchApi _searchApi;
        protected DriveCsvInstrumentSource(IFilesApi filesApi, ISearchApi searchApi, InstrumentType instrumentType, string filePath, string delimiter, int instrumentIdColIdx) : 
            base(instrumentType, filePath, delimiter, instrumentIdColIdx)
        {
            _filesApi = filesApi;
            _searchApi = searchApi;
        }
        
        public new static CsvInstrumentSource Create(InstrumentType instrumentType, IEnumerable<string> instrumentSourceArgs)
        {
            // LusidApiFactory to load instrument source file from drive.
            var lusidApiFactory = LusidApiFactoryBuilder.Build("secrets.json");
            var filesApi = lusidApiFactory.Api<IFilesApi>();
            var searchApi = lusidApiFactory.Api<ISearchApi>();
            
            var (filePath, delimiter, instrumentIdColIdx) = ParseInstrumentSourceArgs(instrumentType, instrumentSourceArgs);
            return new DriveCsvInstrumentSource(filesApi, searchApi, instrumentType, filePath, delimiter, instrumentIdColIdx); 
        }

        protected override IEnumerable<string> LoadInstrumentsFromFile(string filePath, string delimiter, int instrumentIdColIdx)
        {
            var (directoryName, fileName) = LusidDriveUtils.PathToFolderAndFile(filePath);   
            //var directoryName = Path.GetDirectoryName(filePath);
            //var fileName = Path.GetFileName(filePath);
            
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
    }
}