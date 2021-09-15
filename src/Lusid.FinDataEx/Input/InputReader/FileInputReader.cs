using Lusid.FinDataEx.Util.FileUtils.Handler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Lusid.FinDataEx.Input.IInputReader;

namespace Lusid.FinDataEx.Input
{
    public class FileInputReader : IInputReader
    {
        private const char InputFileEntrySeparator = '\n';
        private const string ActionTypeHeaderString = "0-Action Type";

        private readonly DataLicenseOptions _getOptions;
        private readonly IFileHandler _fileHandler;

        public FileInputReader(DataLicenseOptions getOptions, IFileHandler fileHandler)
        {
            _getOptions = getOptions;
            _fileHandler = fileHandler;
        }

        public DataLicenseOutput Read()
        {
            var data = GetFileAsStrings(_getOptions.InputPath);

            var headers = data.First().Split(CsvDelimiter);
            for (var col = 0; col < headers.Count(); col++)
            {
                headers[col] = col.ToString() + "-" + headers[col];
            }

            var rawRecords = new List<Dictionary<string, string>>();
            for (var row = 1; row < data.Count(); row++)
            {
                var rawRecord = new Dictionary<string, string>();
                var entry = data[row].Split(CsvDelimiter);

                for (var col = 0; col < headers.Length; col++)
                {
                    rawRecord.Add(headers[col], entry[col]);
                }

                rawRecords.Add(rawRecord);
            }

            var records = rawRecords.Select(r => ConvertToRecord(r, _getOptions, ActionTypeHeaderString)).ToList();

            return new DataLicenseOutput(_getOptions.InputPath, records);
        }

        private List<string> GetFileAsStrings(string filepath)
        {
            var validatedPath = _fileHandler.ValidatePath(filepath);
            if (string.IsNullOrWhiteSpace(validatedPath))
            {
                throw new FileNotFoundException($"File '{filepath}' not found.");
            }

            return _fileHandler.Read(validatedPath, InputFileEntrySeparator);
        }
    }
}