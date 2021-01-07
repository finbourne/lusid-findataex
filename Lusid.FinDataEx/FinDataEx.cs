﻿using System;
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
            var portfolios = dataOptions.Portfolios;
            var bbgIds = dataOptions.BbgIds;
            var instrumentIdType = dataOptions.InstrumentIdType;
            if (portfolios.Any())
            {
                // setup LusidApiFactory
                var lusidApiFactory = Sdk.Utilities.LusidApiFactoryBuilder.Build("secrets.json");
                var effectiveAt = DateTimeOffset.UtcNow;
                Console.WriteLine($"Retrieving instruments from holdings effectiveAt {effectiveAt} for portfolios {portfolios}");
                ISet<Tuple<string,string>> scopesAndPortfolios = portfolios.Select(p =>
                {
                    var scopeAndPortfolio = p.Split("|");
                    if (scopeAndPortfolio.Length != 2)
                    {
                        throw new ArgumentException($"Unexpected scope and portfolio entry for {p}. Should be " +
                                                    $"in form TestScope|UK_EQUITY");
                    }
                    return new Tuple<string,string>(scopeAndPortfolio[0], scopeAndPortfolio[1]);
                }).ToHashSet();
                
                return new LusidPortfolioInstrumentSource(lusidApiFactory, instrumentIdType, scopesAndPortfolios, effectiveAt);
            } 
            if (bbgIds.Any())
            {
                Console.WriteLine($"Constructing DL instrument requests from Figis: {String.Join(',',bbgIds)}");
                return new BasicInstrumentSource(instrumentIdType, new HashSet<string>(bbgIds));
            }
            
            // should not be possible if commandlineparser runs proper checks
            throw new ArgumentException($"No input portfolios or instruments were provided. Pleas check input " +
                                        $"options {dataOptions}");
        }
    }

    /// <summary>
    /// Base Options for all BBG DL calls
    /// 
    /// </summary>
    class DataLicenseOptions
    {
        [Option('f', "filepath", Required = true, 
            HelpText = "File path to write DLWS output. Include  \"{REQUEST_ID}\" and/or \"{TIMESTAMP}\" in the filename " +
                       " to include the DL request id and/or request timestamp respectively in the filename (e.g. " +
                       "/home/dl_results/MySubmission_{REQUEST_ID}_{TIMESTAMP}.csv")]
        public string OutputFilePath { get; set; }
        
        [Option('s', "filesystem", Required = false, Default = FileSystem.Local, 
            HelpText = "Filesystems to write DL results (Lusid or Local)")]
        public FileSystem FileSystem { get; set; }
        
        [Option('t', "instrument id type", Required = false, Default = InstrumentType.BB_GLOBAL, 
        HelpText = "Type of instrument ids being input (BB_GLOBAL (Figi), ISIN, CUSIP)")]
        public InstrumentType InstrumentIdType { get; set; }
        
        // Instrument Sources : instruments and portfolio options in different sets as only one type of input is allowed
        [Option( 'i', "instruments", Required = true, SetName = "instruments",
            HelpText = "Instruments Ids querying DL. Currently only BBG IDs (Figis) are supported.")]
        public IEnumerable<String> BbgIds { get; set; }
        
        [Option( 'p', "portfolio_and_scopes", Required = true, SetName = "portfolios",
            HelpText = "Portfolios and scopes to retrieve instrument ids from for querying DL. The instruments are returned from " +
                       "the holdings of the portfolios at execution time. Entry should be a portfolio scope pair split by " +
                       "\"|\" e.g. (TestScope|UK_EQUITY)")]
        public IEnumerable<String> Portfolios { get; set; }
        
        // Options around safety and controls
        
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
        public IEnumerable<String> DataFields { get; set; }
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