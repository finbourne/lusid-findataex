using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Call;
using Lusid.FinDataEx.DataLicense.Service.Instrument;
using Lusid.FinDataEx.DataLicense.Vendor;
using Lusid.FinDataEx.Output;
using Lusid.FinDataEx.Util;
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx
{
    public class FinDataEx
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<GetDataOptions,GetActionsOptions>(args)
                .WithParsed<GetDataOptions>(ExecuteGet)
                .WithParsed<GetActionsOptions>(ExecuteGet);
            
        }
        
        /// <summary>
        /// Execute a data license call to BBG DLWS and persists the output.
        ///
        /// Data license calls will specific data for requested instruments and
        /// output that data to csv in the selected file system output directory.
        /// 
        /// </summary>
        /// <param name="getOptions"></param>
        private static void ExecuteGet(DataLicenseOptions getOptions)
        {
            // prepare DL service and output writer
            var dlDataService = new DataLicenseService();

            // construct the writer to persist any data retrieved from Bbg
            var outputDirectory = getOptions.OutputFilePath;
            var fileSystem = getOptions.FileSystem;
            var finDataOutputWriter = CreateFinDataOutputWriter(outputDirectory, fileSystem);
            
            // construct instruments in DL format to be passed to DLWS
            var instruments = CreateInstruments(getOptions);
            
            // construct data license call
            var perSecurityWs = new PerSecurityWsFactory().CreateDefault();
            var dataLicenseCall = CreateDataLicenseCall(getOptions, perSecurityWs);

            LogRequest(instruments, dataLicenseCall);
            // call DL and write results to specified output (as long as not in safe mode)
            if (getOptions.SafeMode)
            {
                Console.WriteLine("--- SAFE MODE --- ");
                Console.WriteLine("As operating in SAFE mode no requests will be pushed to DLWS.");
            }
            else
            {
                var dataLicenseOutput = dlDataService.Get(dataLicenseCall, instruments, ProgramTypes.Adhoc);
                var writeResult = finDataOutputWriter.Write(dataLicenseOutput);
                LogWriteResult(writeResult);
            }
        }
    
        /// <summary>
        ///  Create a BBG DL call depending on the arguments passed into the application.
        /// </summary>
        /// <param name="getOptions">Options taken from the user provided arguments</param>
        /// <param name="perSecurityWs">BBG DLWS client</param>
        /// <returns></returns>
        private static IDataLicenseCall<PerSecurityResponse> CreateDataLicenseCall(DataLicenseOptions getOptions, PerSecurityWS perSecurityWs)
        {
            return getOptions switch
            {
                GetActionsOptions getActionsOptions => new GetActionsDataLicenseCall(perSecurityWs,
                    getActionsOptions.CorpActionTypes.ToList()),
                GetDataOptions getDataOptions => new GetDataLicenseCall(perSecurityWs,
                    getDataOptions.DataFields.ToArray()),
                _ => throw new ArgumentOutOfRangeException(nameof(getOptions))
            };
        }

        /// <summary>
        /// Select and construct an output writer to store the returned data from a DLWS call.
        /// 
        /// </summary>
        /// <param name="outputDirectory"></param>
        /// <param name="fileSystem"></param>
        /// <returns></returns>
        private static IOutputWriter CreateFinDataOutputWriter(string outputDirectory, FileSystem fileSystem)
        {
            return fileSystem switch
            {
                FileSystem.Lusid => new LusidDriveOutputWriter(outputDirectory,
                    LusidApiFactoryBuilder.Build("secrets.json")),
                FileSystem.Local => new LocalFilesystemOutputWriter(outputDirectory),
                _ => throw new ArgumentOutOfRangeException(nameof(fileSystem), fileSystem, null)
            };
        }
        
        /// <summary>
        ///  Log results of BBG response write.
        /// </summary>
        /// <param name="writeResult"></param>
        private static void LogWriteResult(WriteResult writeResult)
        {
            if (writeResult.Status != WriteResultStatus.Ok)
            {
                Console.Error.WriteLine("FinDataEx request completed with failures. See details below: ");
                Console.Error.WriteLine(writeResult);
            }
            else
            {
                Console.WriteLine(writeResult.FileOutputPath);
            }
        }
        
        private static void LogRequest(Instruments instruments, IDataLicenseCall<PerSecurityResponse> dataLicenseCall)
        {
            var instrumentsAndTypes = string.Join(",", instruments.instrument.Select(i => $"{i.type}={i.id}"));
            Console.WriteLine($"Preparing a {dataLicenseCall.GetDataType()} call for instruments : {instrumentsAndTypes}");
        }

        /// <summary>
        /// Select an create an instrument source to build instruments passed to BBG.
        ///
        /// Instrument sources build DLWS compatible instruments from an underlying source (e.g
        /// instruments from holdings in a portfolio, or figis passed in as args).
        ///
        /// </summary>
        /// <param name="dataOptions"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static Instruments CreateInstruments(DataLicenseOptions dataOptions)
        {
            var instruments = CreateInstrumentSource(dataOptions).Get();
            if (instruments is {} dlInstruments)
            {
                // check instruments in request does not exceed the allowed limit
                if (dlInstruments.instrument.Length > dataOptions.MaxInstruments)
                {
                    throw new ArgumentException($"Breach maximum instrument limit. Attempted to request" +
                                                $" {dlInstruments.instrument.Length} instruments but only {dataOptions.MaxInstruments} are allowed. " +
                                                $"To increase the limit override the max allowed instruments with the -m argument parameter.");
                }
                return dlInstruments;
            }
            throw new ArgumentException($"No DL instruments could be created from the instruments or " +
                                        $"portfolios provided. Check portfolios provided have existing holdings or" +
                                        $" the instruments arguments are legal . Inputs={dataOptions}");
        }

        private static IInstrumentSource CreateInstrumentSource(DataLicenseOptions dataOptions)
        {
            return dataOptions.InstrumentSource switch
            {
                nameof(InstrumentSource) =>
                    InstrumentSource.Create(dataOptions.InstrumentIdType, dataOptions.InstrumentSourceArguments),
                nameof(LusidPortfolioInstrumentSource) =>
                    LusidPortfolioInstrumentSource.Create(dataOptions.InstrumentIdType, dataOptions.InstrumentSourceArguments),
                nameof(CsvInstrumentSource) =>
                    CsvInstrumentSource.Create(dataOptions.InstrumentIdType, dataOptions.InstrumentSourceArguments),
                nameof(DriveCsvInstrumentSource) =>
                    DriveCsvInstrumentSource.Create(dataOptions.InstrumentIdType, dataOptions.InstrumentSourceArguments),
                _ => throw new ArgumentOutOfRangeException(
                    $"{dataOptions.InstrumentSource} has no supported implementation.")
            };
        }
    }

    /// <summary>
    /// Base Options for all BBG DL calls
    /// 
    /// </summary>
    class DataLicenseOptions
    {
        [Option('f', "filepath", Required = true, 
            HelpText = "File path to write DLWS output. Include  \"{REQUEST_ID}\", \"{AS_AT}\", \"{AS_AT_DATE}\" in the filename " +
                       " to include the DL request id timestamps respectively in the filename (e.g. " +
                       "/home/dl_results/MySubmission_{REQUEST_ID}_{AS_AT}.csv")]
        public string OutputFilePath { get; set; }
        
        [Option('s', "filesystem", Required = false, Default = FileSystem.Local, 
            HelpText = "Filesystems to write DL results (Lusid or Local)")]
        public FileSystem FileSystem { get; set; }
        
        [Option('t', "instrument_id_type", Required = false, Default = InstrumentType.BB_GLOBAL, 
        HelpText = "Type of instrument ids being input (BB_GLOBAL (Figi), ISIN, CUSIP)")]
        public InstrumentType InstrumentIdType { get; set; }
        
        /*
         * Start Instrument Sources :
         * Input arguments on where to source instruments to request data against.
         * Using SetName mutual exclusivity as only one instrument source is supported per request.
         */
        
        [Option( 'i', "instrument-source", Required = true, Default = "InstrumentSource",
            HelpText = "Instrument source to create the instruments to query against DataLicense. Supported types include" +
                       " : [InstrumentSource, LusidPortfolioInstrumentSource, FromDriveCsvInstrumentSource, FromLocalCsvInstrumentSource]." +
                       " Developers can add custom instrument sources as required, see FinDataEx readme for details.")]
        public string InstrumentSource { get; set; }
        
        [Option( 'a', "instrument-source-args", Required = false,
            HelpText = "Arguments passed to the instrument source for retrieving instruments to query against DataLicense.")]
        public IEnumerable<string> InstrumentSourceArguments { get; set; }

        /*
         *  Safety and Control Options :
         *  Given DLWS charges per call adding features to allow restrictions in number of instruments to query.
         *  Safemode allow request construction without sending to DL for testing and debugging.
         */
        
        [Option("safemode", Default = false, HelpText = "Running in safe mode will simply print the DL request without making the actual call to BBG.")]
        public bool SafeMode { get; set; }

        [Option('m', "max_instruments", Default = 50, HelpText = "Set the maximum number of instruments allowed in a BBG DLWS call. Especially important in" +
                                                                 " production environments that are billed per instrument.")]
        public int MaxInstruments { get; set; }
    }

    /// <summary>
    /// Options for GetData calls to BBG
    /// </summary>
    [Verb ("getdata", HelpText = "BBG DL request to retrieve data for requested set of fields and insturments.")]
    class GetDataOptions : DataLicenseOptions
    {
        [Option('d', "datafields", Required = true, HelpText = "BBG DL fields to retrieve. Only relevant for GetData requests.")]
        public IEnumerable<string> DataFields { get; set; }
    }
    
    /// <summary>
    /// Options for GetAction calls to BBG
    /// </summary>
    [Verb ("getactions", HelpText = "BBG DL request to retrieve corporate actions for requested instruments.")]
    class GetActionsOptions : DataLicenseOptions
    {
        [Option('c', "corpactions", Required = true, HelpText = "The corporate action types to retrieve (e.g. DVD_CASH, STOCK_SPLIT, etc...)")]
        public IEnumerable<CorpActionType> CorpActionTypes { get; set; }
    }
}