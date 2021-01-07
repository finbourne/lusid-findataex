using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Lusid.FinDataEx.Output.IOutputWriter;

namespace Lusid.FinDataEx.Output
{
    /// <summary>
    ///  Writes FinDataOutput to local file system.
    /// </summary>
    public class LocalFilesystemOutputWriter : IOutputWriter
    {
        protected readonly string OutputFilePath;

        public LocalFilesystemOutputWriter(string outputFilePath)
        {
            OutputFilePath = outputFilePath;
        }
        
        public WriteResult Write(DataLicenseOutput dataLicenseOutput)
        {
            if (dataLicenseOutput.IsEmpty())
            {
                Console.WriteLine($"Attempting to write empty data license output : {dataLicenseOutput}. Skipping...");
                return WriteResult.NotRun();
            }
            // prepare fin data as strings to be written to file
            var headers = string.Join(BbgDlDelimiter, dataLicenseOutput.Header);
            var finDataRecords = new List<string>{headers};
            finDataRecords.AddRange(
            dataLicenseOutput.Records.Select(dR =>
                    {
                        var record = new List<string>();
                        foreach (var header in dataLicenseOutput.Header)
                        {
                            dR.TryGetValue(header, out string recordEntry);
                            record.Add(recordEntry);
                        }

                        return string.Join(BbgDlDelimiter, record);
                    }).ToList()
            );
            
            try
            {
                var modifiedFilepath = CreateFilepathWithAutoGenPatterns(dataLicenseOutput.Id);
                var outputPathWritten = WriteToFile(modifiedFilepath, finDataRecords);
                return WriteResult.Ok(outputPathWritten);
            }
            catch (Exception e)
            {
                return WriteResult.Fail($"FAILURE : Did not write {dataLicenseOutput.Id} to {OutputFilePath} due to an exception. Cause of failure: {e}");
            }
        }

        protected virtual string WriteToFile(string modifiedFilepath, IEnumerable<string> finDataRecords)
        {
            File.WriteAllLines(modifiedFilepath, finDataRecords);
            return modifiedFilepath;
        }

        private string CreateFilepathWithAutoGenPatterns(string dataLicenseOutputId)
        {
            // check for and apply patterns to output filename
            var modifiedOutputFilePath = OutputFilePath.Replace(TimestampPattern,
                DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss"));
            return modifiedOutputFilePath.Replace(RequestIdPattern, dataLicenseOutputId);
        }

    }
}