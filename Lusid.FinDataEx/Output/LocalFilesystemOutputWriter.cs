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
        
        public WriteResult Write(DataLicenseOutput finDataOutput)
        {
            if (finDataOutput.IsEmpty())
            {
                Console.WriteLine($"Attempting to write empty data license output : {finDataOutput}. Skipping...");
                return WriteResult.NotRun();
            }
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
                var outputPathWritten = WriteToFile(finDataOutput.Id + BbgDlOutputFileFormat, finDataRecords);
                return WriteResult.Ok(outputPathWritten);
            }
            catch (Exception e)
            {
                return WriteResult.Fail($"FAILURE : Did not write {finDataOutput.Id} to {OutputDir} due to an exception. Cause of failure: {e}");
            }
        }

        protected virtual string WriteToFile(string outputFile, IEnumerable<string> finDataRecords)
        {
            var outputPath = OutputDir + Path.DirectorySeparatorChar + outputFile;
            File.WriteAllLines(outputPath, finDataRecords);
            return outputPath;
        }

    }
}