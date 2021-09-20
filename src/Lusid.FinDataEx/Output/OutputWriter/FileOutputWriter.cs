using System;
using System.Collections.Generic;
using System.Linq;
using Lusid.FinDataEx.Util;
using Lusid.FinDataEx.Util.FileUtils.Handler;
using static Lusid.FinDataEx.Output.IOutputWriter;

namespace Lusid.FinDataEx.Output
{
    public class FileOutputWriter : IOutputWriter
    {
        private const char OutputFileEntrySeparator = '\n';

        private readonly string _outputFilePath;
        private readonly IFileHandler _fileHandler;

        public FileOutputWriter(DataLicenseOptions getOptions, IFileHandler fileHandler)
        {
            _outputFilePath = getOptions.OutputPath;
            _fileHandler = fileHandler;
        }

        public WriteResult Write(DataLicenseOutput dataLicenseOutput)
        {
            if (dataLicenseOutput.IsEmpty())
            {
                Console.WriteLine($"Attempting to write empty data license output : {dataLicenseOutput}. Skipping...");
                return WriteResult.Ok(string.Empty);
            }

            var finDataRecords = new List<string>();

            if (dataLicenseOutput.DataRecords.Any())
            {
                var headers = dataLicenseOutput.DataRecords.First().Headers;
                finDataRecords.Add(string.Join(BbgDlDelimiter, headers));

                finDataRecords.AddRange(
                    dataLicenseOutput.DataRecords.Select(r =>
                    {
                        var record = new List<string>();
                        foreach (var header in headers)
                        {
                            var recordEntry = r.RawData[header];
                            record.Add(recordEntry);
                        }

                        return string.Join(BbgDlDelimiter, record);
                    }).ToList()
                );
            }

            if (dataLicenseOutput.CorporateActionRecords.Any())
            {
                if (finDataRecords.Any())
                {
                    finDataRecords.Add(string.Empty);
                }

                var headers = dataLicenseOutput.CorporateActionRecords.First().Headers;
                finDataRecords.Add(string.Join(BbgDlDelimiter, headers));

                finDataRecords.AddRange(
                    dataLicenseOutput.CorporateActionRecords.Select(r =>
                    {
                        var record = new List<string>();
                        foreach (var header in headers)
                        {
                            var recordEntry = r.RawData[header];
                            record.Add(recordEntry);
                        }

                        return string.Join(BbgDlDelimiter, record);
                    }).ToList()
                );
            }

            try
            {
                var modifiedFilepath = CreateFilepathWithAutoGenPatterns(dataLicenseOutput.Id);
                var outputPathWritten = WriteToFile(modifiedFilepath, finDataRecords);
                return WriteResult.Ok(outputPathWritten);
            }
            catch (Exception e)
            {
                return WriteResult.Fail($"FAILURE : Did not write {dataLicenseOutput.Id} to {_outputFilePath} due to an exception. Cause of failure: {e}");
            }
        }

        private string CreateFilepathWithAutoGenPatterns(string dataLicenseOutputId)
        {
            return AutoGenPatternUtils.ApplyAllPatterns(_outputFilePath, dataLicenseOutputId);
        }

        private string WriteToFile(string modifiedFilepath, List<string> finDataRecords)
        {
            Console.WriteLine($"Attempting to write to filename={modifiedFilepath}");

            modifiedFilepath = _fileHandler.Write(modifiedFilepath, finDataRecords, OutputFileEntrySeparator);

            Console.WriteLine($"Completed write to filename={modifiedFilepath}");

            return modifiedFilepath;
        }
    }
}