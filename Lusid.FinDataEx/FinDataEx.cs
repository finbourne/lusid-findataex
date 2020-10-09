using System;
using System.Collections.Generic;
using System.Linq;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Vendor;
using Lusid.FinDataEx.Output;
using static Lusid.FinDataEx.DataLicense.Util.DlTypes;

namespace Lusid.FinDataEx
{
    public class FinDataEx
    {
        public static void Main(string[] args)
        {
            // parse required arguments
            DataTypes dlDataType = Enum.Parse<DataTypes>(args[0]);
            List<string> bbgIds = GetBbgIds(args[1]);
            string outputDirectory = args[2];

            // prepare DL service and output writer
            DLDataService dlDataService = CreateDlDataService();
            IFinDataOutputWriter finDataOutputWriter = CreateFinDataOutputWriter(outputDirectory);
            
            // call DL and write results to specified output
            List<FinDataOutput> finDataOutputs =  dlDataService.Get(bbgIds, ProgramTypes.Adhoc, dlDataType);
            WriteResult writeResult =  finDataOutputWriter.Write(finDataOutputs);
            LogWriteResult(writeResult);
        }

        private static DLDataService CreateDlDataService()
        {
            PerSecurityWSFactory perSecurityWsFactory = new PerSecurityWSFactory();
            return new DLDataService(perSecurityWsFactory.CreateDefault());
        }

        private static IFinDataOutputWriter CreateFinDataOutputWriter(string outputDirectory)
        {
            return new LocalFilesystemFinDataOutputWriter(outputDirectory);
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
                Console.WriteLine(writeResult.Message);
            }
        }
    }
}