using Lusid.FinDataEx.Input;
using System;
using System.Linq;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Call;
using Lusid.FinDataEx.DataLicense.Service.Instrument;
using Lusid.FinDataEx.DataLicense.Vendor;
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx
{
    public class DataLicenseReader : IInputReader
    {
        private readonly DataLicenseOptions _getOptions;
        private readonly Sdk.Utilities.ILusidApiFactory _lusidApiFactory;
        private readonly Drive.Sdk.Utilities.ILusidApiFactory _driveApiFactory;

        public DataLicenseReader(DataLicenseOptions getOptions, Sdk.Utilities.ILusidApiFactory lusidApiFactory, Drive.Sdk.Utilities.ILusidApiFactory driveApiFactory)
        {
            _getOptions = getOptions;
            _lusidApiFactory = lusidApiFactory;
            _driveApiFactory = driveApiFactory;
        }

        public DataLicenseOutput Read()
        {
            // construct instruments in DL format to be passed to DLWS
            var instruments = CreateInstruments(_getOptions);
            if (!instruments.instrument.Any())
            {
                Console.WriteLine("No instruments were constructed from your selected source and arguments. No DLWS call " +
                                  "will be executed");
                return DataLicenseOutput.Empty();
            }

            // construct data license call
            var perSecurityWs = new PerSecurityWsFactory().CreateDefault();
            var dataLicenseCall = CreateDataLicenseCall(_getOptions, perSecurityWs);

            LogRequest(instruments, dataLicenseCall);

            // call DL and write results to specified output (as long as not in safe mode)
            var dlDataService = new DataLicenseService();
            return dlDataService.Get(dataLicenseCall, instruments, ProgramTypes.Adhoc, _getOptions.EnableLiveRequests);
        }

        /// <summary>
        /// Select an create an instrument source to build instruments passed to BBG.
        ///
        /// Instrument sources build DLWS compatible instruments from an underlying source (e.g
        /// instruments from holdings in a portfolio, or figis passed in as args).
        /// </summary>
        /// <param name="dataOptions"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private Instruments CreateInstruments(DataLicenseOptions dataOptions)
        {
            var instrumentSource = CreateInstrumentSource(dataOptions);
            var instruments = instrumentSource.Get();
            if (instruments is { } dlInstruments)
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

        private IInstrumentSource CreateInstrumentSource(DataLicenseOptions dataOptions)
        {
            var instrumentArgs = InstrumentArgs.Create(dataOptions);
            return dataOptions.InstrumentSource switch
            {
                nameof(InstrumentSource) =>
                    InstrumentSource.Create(instrumentArgs, dataOptions.InstrumentSourceArguments),
                nameof(LusidPortfolioInstrumentSource) =>
                    LusidPortfolioInstrumentSource.Create(_lusidApiFactory, instrumentArgs, dataOptions.InstrumentSourceArguments),
                nameof(CsvInstrumentSource) =>
                    CsvInstrumentSource.Create(instrumentArgs, dataOptions.InstrumentSourceArguments),
                nameof(DriveCsvInstrumentSource) =>
                    DriveCsvInstrumentSource.Create(_driveApiFactory, instrumentArgs, dataOptions.InstrumentSourceArguments),
                _ => throw new ArgumentOutOfRangeException(
                    $"{dataOptions.InstrumentSource} has no supported implementation.")
            };
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

        private static void LogRequest(Instruments instruments, IDataLicenseCall<PerSecurityResponse> dataLicenseCall)
        {
            var instrumentsAndTypes = string.Join(",", instruments.instrument.Select(i => $"{i.type}={i.id}"));
            Console.WriteLine($"Preparing a {dataLicenseCall.GetDataType()} call for instruments : {instrumentsAndTypes}");
        }
    }
}