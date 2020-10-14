using System;
using System.Collections.Generic;
using System.Linq;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Vendor;
using Lusid.FinDataEx.Output;
using static Lusid.FinDataEx.DataLicense.Util.DlTypes;

namespace Lusid.FinDataEx
{
    public class FinDataEx
    {
        private static readonly string LusidFileSystem = "lusid";
        public static void Main(string[] args)
        {
            // TODO move to CommandLineParser library
            // parse required arguments
            var dlDataType = Enum.Parse<DataTypes>(args[0]);
            var bbgIds = GetBbgIds(args[1]);
            var outputDirectory = args[2];
            var fileSystem = (args.Length > 3) ? args[3] : "";

            // prepare DL service and output writer
            var dlDataService = CreateDlDataService();
            var finDataOutputWriter = CreateFinDataOutputWriter(outputDirectory, fileSystem);
            
            // call DL and write results to specified output
            var finDataOutputs =  dlDataService.Get(bbgIds, ProgramTypes.Adhoc, dlDataType);
            var writeResult =  finDataOutputWriter.Write(finDataOutputs);
            LogWriteResult(writeResult);
        }

        private static DlDataService CreateDlDataService()
        {
            var perSecurityWsFactory = new PerSecurityWsFactory();
            return new DlDataService(perSecurityWsFactory.CreateDefault());
        }

        private static IFinDataOutputWriter CreateFinDataOutputWriter(string outputDirectory, string fileSystem)
        {
            if (fileSystem.Equals(LusidFileSystem))
            {
                var lusidApiFactory = LusidApiFactoryBuilder.Build("secrets.json");
                return new LusidDriveFinDataOutputWriter(outputDirectory, lusidApiFactory);
            }
            else
            {
                return new LocalFilesystemFinDataOutputWriter(outputDirectory);
            }
        }
        
        
        private static List<string> GetBbgIds(string bbgIdArg)
        {
            return bbgIdArg.Split("|").ToList();
        }
        
        private static void LogWriteResult(WriteResult writeResult)
        {
            if (writeResult.Status != WriteResultStatus.Ok)
            {
                Console.Error.WriteLine("FinDataEx request completed with failures. See details below: ");
                Console.Error.WriteLine(writeResult);
            }
            else
            {
                Console.WriteLine(writeResult.FilesWritten);
            }
        }
    }
}