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
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx
{
    public class FinDataEx
    {
        private static readonly string LusidFileSystem = "lusid";
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<GetDataOptions>(args)
                .WithParsed(ExecuteGetData);
            
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
            var outputDirectory = getDataOptions.OutputDirectory;
            var fileSystem = getDataOptions.FileSystem;
            var dataFields = getDataOptions.DataFields;

            // prepare DL service and output writer
            var dlDataService = new DataLicenseService();
            var perSecurityWs = new PerSecurityWsFactory().CreateDefault();
            var bbgCall = new GetDataLicenseCall(perSecurityWs, dataFields.ToArray());
            var finDataOutputWriter = CreateFinDataOutputWriter(outputDirectory, fileSystem);
            
            // construct instruments in DL format to be passed to DLWS
            var instruments = CreateInstruments(getDataOptions);           
            
            // call DL and write results to specified output
            var finDataOutputs =  dlDataService.Get(bbgCall, instruments, ProgramTypes.Adhoc);
            var writeResult =  finDataOutputWriter.Write(finDataOutputs);
            LogWriteResult(writeResult);
        }

        /// <summary>
        /// Select and construct an output writer to store the returned data from a DLWS call.
        /// 
        /// </summary>
        /// <param name="outputDirectory"></param>
        /// <param name="fileSystem"></param>
        /// <returns></returns>
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
                Console.WriteLine(writeResult.FilesWritten);
            }
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
        private static Instruments CreateInstruments(BaseOptions dataOptions)
        {
            var instruments = CreateInstrumentSource(dataOptions).Get();
            if (instruments is {} dlInstruments)
            {
                return dlInstruments;
            }
            throw new ArgumentException($"No DL instruments could be created from the instruments or " +
                                        $"portfolios provided. Check portfolios provided have existing holdings or" +
                                        $" the instruments arguments are legal . Inputs={dataOptions}");
        }

        private static IInstrumentSource CreateInstrumentSource(BaseOptions dataOptions)
        {
            var portfolios = dataOptions.Portfolios;
            var bbgIds = dataOptions.BbgIds;
            var instrumentIdType = dataOptions.InstrumentIdType;
            if (portfolios.Any())
            {
                DateTimeOffset effectiveAt = DateTimeOffset.UtcNow;
                Console.WriteLine($"Retrieving instruments from holdings effectiveAt {effectiveAt} for portfolios {portfolios}");
                ISet<Tuple<string,string>> scopesAndPortfolios = portfolios.Select(p =>
                {
                    string[] scopeAndPortfolio = p.Split("|");
                    if (scopeAndPortfolio.Length != 2)
                    {
                        throw new ArgumentException($"Unexpected scope and portfolio entry for {p}. Should be " +
                                                    $"in form TestScope|UK_EQUITY");
                    }
                    return new Tuple<string,string>(scopeAndPortfolio[0], scopeAndPortfolio[1]);
                }).ToHashSet();
                
                return new LusidPortfolioInstrumentSource(instrumentIdType, scopesAndPortfolios, effectiveAt);
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
    class BaseOptions
    {
        [Option('o', "output", Required = true, HelpText = "Output directory to write DL results.")]
        public string OutputDirectory { get; set; }
        
        [Option('f', "filesystem", Required = false, Default ="Local", 
            HelpText = "Filesystems to write DL results (Lusid or Local)")]
        public string FileSystem { get; set; }
        
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