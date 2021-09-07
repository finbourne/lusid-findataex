using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Lusid.FinDataEx.Input.IInputReader;

namespace Lusid.FinDataEx.Input
{
    public class LocalFilesystemInputReader : IInputReader
    {
        private readonly DataLicenseOptions _getOptions;

        public LocalFilesystemInputReader(DataLicenseOptions getOptions)
        {
            _getOptions = getOptions;
        }

        public DataLicenseOutput Read()
        {
            var data = GetFileAsStringsFromLocalFolder(_getOptions.InputPath);

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

        private string[] GetFileAsStringsFromLocalFolder(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException($"Local file '{filepath}' not found.");

            return File.ReadAllLines(filepath);
        }
    }
}