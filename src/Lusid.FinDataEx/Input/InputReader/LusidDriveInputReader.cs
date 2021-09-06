using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Model;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Lusid.FinDataEx.Input.IInputReader;

namespace Lusid.FinDataEx.Input
{
    public class LusidDriveInputReader : IInputReader
    {
        public const string InputFileEntrySeparator = "\n";

        private readonly DataLicenseOptions _getOptions;
        private readonly IFilesApi _filesApi;
        private readonly ISearchApi _searchApi;

        public LusidDriveInputReader(DataLicenseOptions getOptions, ILusidApiFactory driveApiFactory)
        {
            _getOptions = getOptions;
            _filesApi = driveApiFactory.Api<IFilesApi>();
            _searchApi = driveApiFactory.Api<ISearchApi>();
        }

        public DataLicenseOutput Read()
        {
            var data = GetFileAsStringsFromFolderInDrive(_getOptions.InputPath);

            var headers = data.First().Split(CsvDelimiter);
            for (var col = 0; col < headers.Count(); col++)
            {
                headers[col] = col.ToString() + "-" + headers[col];
            }

            var records = new List<Dictionary<string, string>>();
            for (var row = 1; row < data.Count(); row++)
            {
                var record = new Dictionary<string, string>();
                var entry = data[row].Split(CsvDelimiter);

                for (var col = 0; col < headers.Length; col++)
                {
                    record.Add(headers[col], entry[col]);
                }

                records.Add(record);
            }

            return new DataLicenseOutput(_getOptions.InputPath, headers, records);
        }

        private string[] GetFileAsStringsFromFolderInDrive(string filepath)
        {
            var (directoryName, fileName) = LusidDriveUtils.PathToFolderAndFile(filepath);

            // Retrieve LUSID drive file id from path
            var fileId = _searchApi.Search(new SearchBody(directoryName, fileName)).Values.SingleOrDefault()?.Id;
            if (fileId == null)
                throw new FileNotFoundException($"LUSID Drive location '{directoryName}' with file '{fileName}' not found.");

            return new StreamReader(_filesApi.DownloadFile(fileId))
                .ReadToEnd()
                .Split(InputFileEntrySeparator);
        }
    }
}