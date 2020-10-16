using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Call;
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
            Parser.Default.ParseArguments<GetDataOptions>(args)
                .WithParsed<GetDataOptions>(ExecuteGetData);
            
        }

        /// <summary>
        /// Execute a GetData call to BBG DL and persist output
        ///
        /// GetData calls will retrieve instrument data for the requested data fields and
        /// output a csv to the selected file system output directory.
        /// 
        /// </summary>
        /// <param name="getDataOptions"></param>
        private static void ExecuteGetData(GetDataOptions getDataOptions)
        {
            var bbgIds = getDataOptions.BbgIds;
            var outputDirectory = getDataOptions.OutputDirectory;
            var fileSystem = getDataOptions.FileSystem;
            var dataFields = getDataOptions.DataFields;
            
            // prepare DL service and output writer
            var dlDataService = new DlDataService();
            var perSecurityWs = new PerSecurityWsFactory().CreateDefault();
            var bbgCall = new GetDataBbgCall(perSecurityWs, dataFields.ToArray());
            var finDataOutputWriter = CreateFinDataOutputWriter(outputDirectory, fileSystem);
            
            // call DL and write results to specified output
            var finDataOutputs =  dlDataService.Get(bbgCall, bbgIds, ProgramTypes.Adhoc);
            var writeResult =  finDataOutputWriter.Write(finDataOutputs);
            LogWriteResult(writeResult);
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

    /// <summary>
    /// Base Options for all BBG DL calls
    /// 
    /// </summary>
    class BaseOptions
    {
        [Option('i', "instruments", Required = true,
            HelpText = "Instruments Ids querying DL for. Currently only BBG IDs (Figis) are supported.")]
        public IEnumerable<String> BbgIds { get; set; }
        
        [Option('o', "output", Required = true, HelpText = "Output directory to write DL results.")]
        public string OutputDirectory { get; set; }
        
        [Option('f', "filesystem", Required = false, Default ="Local", 
            HelpText = "Filesystems to write DL results (Lusid or Local")]
        public string FileSystem { get; set; }
        
        /*[Option("datatype", Required = false, HelpText = "BBG DL datatype. Supported types GetData or GetActions.")]
        public string DataType { get; set; }
        
        [Option("datafields", Required = false, HelpText = "BBG DL fields to retrieve. Only relevant for GetData requests.")]
        public IEnumerable<String> DataFields { get; set; }*/
        
    }

    /// <summary>
    /// Options for GetData calls to BBG
    /// </summary>
    [Verb ("getdata", HelpText = "BBG DL request to retrieve data for requested set of fields and insturments.")]
    class GetDataOptions : BaseOptions
    {
        [Option('d', "datafields", Required = true, HelpText = "BBG DL fields to retrieve. Only relevant for GetData requests.")]
        public IEnumerable<String> DataFields { get; set; }
    }
}