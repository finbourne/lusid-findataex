using System;
using System.IO;
using System.Linq;
using CommandLine;
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
        private const int SuExitCode = 0;
        private const int FaBadArgExitCode = 1;
        private const int FaProcessingExitCode = 1;

        private const string secretsJsonFilename = "secrets.json";
        private static readonly Sdk.Utilities.ILusidApiFactory LusidApiFactory = Sdk.Utilities.LusidApiFactoryBuilder.Build(secretsJsonFilename);
        private static readonly Drive.Sdk.Utilities.ILusidApiFactory DriveApiFactory = Drive.Sdk.Utilities.LusidApiFactoryBuilder.Build(secretsJsonFilename);

        public static int Main(string[] args)
        {
            try
            {
                var parserResult = Parser.Default.ParseArguments<GetDataOptions, GetActionsOptions>(args)
                    .WithParsed<GetDataOptions>(ExecuteGet)
                    .WithParsed<GetActionsOptions>(ExecuteGet);
                if (parserResult.Tag == ParserResultType.NotParsed)
                {
                    Console.WriteLine(
                        "FinDataExt program arguments could not be parsed. Check above logs for details. Exiting FinDataEx.");
                    return FaBadArgExitCode;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"FinDataEx request processing failed. Exiting FinDataEx. Exception details : {e}");
                return FaProcessingExitCode;
            }
            Console.WriteLine($"FinDataEx run to successful completion with exit code {SuExitCode}");
            return SuExitCode;
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
            if (instruments.instrument.Length == 0)
            {
                Console.WriteLine("No instruments were constructed from your selected source and arguments. No DLWS call " +
                                  "will be executed");
                return;
            }
            
            // construct data license call
            var perSecurityWs = new PerSecurityWsFactory().CreateDefault();
            var dataLicenseCall = CreateDataLicenseCall(getOptions, perSecurityWs);

            LogRequest(instruments, dataLicenseCall);
            // call DL and write results to specified output (as long as not in safe mode)
            if (getOptions.Unsafe)
            {
                var dataLicenseOutput = dlDataService.Get(dataLicenseCall, instruments, ProgramTypes.Adhoc);
                var writeResult = finDataOutputWriter.Write(dataLicenseOutput);
                ProcessWriteResult(writeResult);
            }
            else
            {
                Console.WriteLine("--- SAFE MODE --- ");
                Console.WriteLine("As operating in SAFE mode no requests will be pushed to DLWS.");
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
                FileSystem.Lusid => new LusidDriveOutputWriter(outputDirectory, DriveApiFactory),
                FileSystem.Local => new LocalFilesystemOutputWriter(outputDirectory),
                _ => throw new ArgumentOutOfRangeException(nameof(fileSystem), fileSystem, null)
            };
        }
        
        /// <summary>
        ///  Log results of BBG response write.
        /// </summary>
        /// <param name="writeResult"></param>
        private static void ProcessWriteResult(WriteResult writeResult)
        {
            if (writeResult.Status != WriteResultStatus.Ok)
            {
                Console.Error.WriteLine("FinDataEx request completed with failures...");
                throw new Exception(writeResult.ToString());
            }
            Console.WriteLine($"FinDataEx request completed and output written to {writeResult.FileOutputPath}");
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
            var instrumentSource = CreateInstrumentSource(dataOptions);
            var instruments = instrumentSource.Get();
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
            var instrumentArgs = InstrumentArgs.Create(dataOptions);
            return dataOptions.InstrumentSource switch
            {
                nameof(InstrumentSource) =>
                    InstrumentSource.Create(instrumentArgs, dataOptions.InstrumentSourceArguments),
                nameof(LusidPortfolioInstrumentSource) =>
                    LusidPortfolioInstrumentSource.Create(LusidApiFactory, instrumentArgs, dataOptions.InstrumentSourceArguments),
                nameof(CsvInstrumentSource) =>
                    CsvInstrumentSource.Create(instrumentArgs, dataOptions.InstrumentSourceArguments),
                nameof(DriveCsvInstrumentSource) =>
                    DriveCsvInstrumentSource.Create(DriveApiFactory, instrumentArgs, dataOptions.InstrumentSourceArguments),
                _ => throw new ArgumentOutOfRangeException(
                    $"{dataOptions.InstrumentSource} has no supported implementation.")
            };
        }
    }
}