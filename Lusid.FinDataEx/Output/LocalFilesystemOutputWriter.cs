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

        protected readonly string OutputDir;

        public LocalFilesystemOutputWriter(string outputDir)
        {
            OutputDir = outputDir;
        }
        
        public WriteResult Write(IEnumerable<DataLicenseOutput> finDataOutputs)
        {
         
            var filesWritten = new List<string>();
            var responseFailResults = new List<string>();
            foreach (var finDataOutput in finDataOutputs)
            {
                // prepare fin data as strings to be written to file
                var headers = string.Join(BbgDlDelimiter, finDataOutput.Header);
                var finDataRecords = new List<string>{headers};
                finDataRecords.AddRange(
                finDataOutput.Records.Select(dR =>
                        {
                            List<string> record = new List<string>();
                            foreach (var header in finDataOutput.Header)
                            {
                                dR.TryGetValue(header, out string recordEntry);
                                record.Add(recordEntry);
                            }

                            return string.Join(BbgDlDelimiter, record);
                        }).ToList()
                );
                
                try
                {
                    string outputPathWritten = WriteToFile(finDataOutput.Id + BbgDlOutputFileFormat, finDataRecords);
                    // record the output path of files written
                    filesWritten.Add(outputPathWritten);
                }
                catch (Exception e)
                {
                    responseFailResults.Add($"FAILURE : Did not write {finDataOutput.Id} to {OutputDir} due to an exception. Cause of failure: {e}");
                }
            }
            
            // Prepare the result status to return
            var status = (responseFailResults.Any())
                ? WriteResultStatus.Fail
                : WriteResultStatus.Ok;
            var responseResults = new List<string>();
            responseResults.AddRange(responseFailResults);
            responseResults.AddRange(filesWritten);

            return new WriteResult(status, filesWritten, responseFailResults);
        }

        protected virtual string WriteToFile(string outputFile, IEnumerable<string> finDataRecords)
        {
            var outputPath = OutputDir + Path.DirectorySeparatorChar + outputFile;
            File.WriteAllLines(outputPath, finDataRecords);
            return outputPath;
        }

    }
}