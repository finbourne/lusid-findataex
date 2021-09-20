using System.Collections.Generic;
using CommandLine;
using Lusid.FinDataEx.Util;
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx
{
    /// <summary>
    /// Base Options for all BBG DL calls
    /// </summary>
    public class DataLicenseOptions
    {
        /*
         * Operation
         */
        [Option('t', "operation-type",
            Required = true,
            Default = OperationType.BloombergRequest,
            HelpText = "The type of operation [ParseExisting, BloombergRequest]")]
        public OperationType OperationType { get; set; }

        /*
         * Input
         */
        [Option('i', "input-source",
            Required = true,
            Default = InputType.CLI,
            HelpText = "The input source [CLI, LUSID, Drive, Local]")]
        public InputType InputSource { get; set; }

        [Option("input-path",
            Required = false,
            HelpText = "If reading from a file, this should be the path to it")]
        public string InputPath { get; set; }

        [Option("instrument-source-args",
            SetName = "instrumentInput",
            Required = false,
            HelpText = "Arguments passed to the instrument source for retrieving instruments to query against DataLicense.")]
        public IEnumerable<string> InstrumentSourceArguments { get; set; }

        [Option("instrument_id_type",
            SetName = "instrumentInput",
            Required = false,
            Default = InstrumentType.BB_GLOBAL,
            HelpText = "Type of instrument ids being input (BB_GLOBAL (Figi), ISIN, CUSIP)")]
        public InstrumentType InstrumentIdType { get; set; }

        /*
         * Output
         */
        [Option('o', "output-target",
            Required = true,
            Default = OutputType.Local,
            HelpText = "The  output target [LUSID, Drive, Local]")]
        public OutputType OutputTarget { get; set; }

        [Option("output-path",
            Required = true,
            HelpText = "If writing to a file path, you can include \"{REQUEST_ID}\", \"{AS_AT}\", \"{AS_AT_DATE}\" in the filename " +
                       "(e.g. /home/dl_results/MySubmission_{REQUEST_ID}_{AS_AT}.csv. " +
                       "If writing to a LUSID tenant, you must provide the location as \"{SCOPE}|{CODE}\"")]
        public string OutputPath { get; set; }

        /*
         * Other Options
         */
        [Option('y', "yellowkey",
            Required = false,
            HelpText = "Yellow key required if querying by BBG by TICKER. YellowKey maps to MarketSector in DLWS.")]
        public MarketSector YellowKey { get; set; }

        /*
         *  Safety and Control Options :
         *  Given DLWS charges per call adding features to allow restrictions in number of instruments to query.
         *  Safemode allow request construction without sending to DL for testing and debugging.
         */
        [Option("enable-live-requests",
            Default = false,
            HelpText = "Set this flag to allow billable calls to be sent to BBG. " +
                       "By default, calls will be echoed to terminal and not sent to BBG to prevent accidental billing.")]
        public bool EnableLiveRequests { get; set; }

        [Option('m', "max_instruments",
            Default = 50,
            HelpText = "Set the maximum number of instruments allowed in a BBG DLWS call. Especially important in " +
                       "production environments that are billed per instrument.")]
        public int MaxInstruments { get; set; }
    }

    /// <summary>
    /// Options for GetData calls to BBG
    /// </summary>
    [Verb("getdata", HelpText = "BBG DL request to retrieve data for requested set of fields and insturments.")]
    public class GetDataOptions : DataLicenseOptions
    {
        [Option('d', "datafields",
            Required = true,
            HelpText = "BBG DL fields to retrieve. Only relevant for GetData requests.")]
        public IEnumerable<string> DataFields { get; set; }
    }

    /// <summary>
    /// Options for GetAction calls to BBG
    /// </summary>
    [Verb("getactions", HelpText = "BBG DL request to retrieve corporate actions for requested instruments.")]
    public class GetActionsOptions : DataLicenseOptions
    {
        [Option('c', "corpactions",
            Required = true,
            HelpText = "The corporate action types to retrieve (e.g. DVD_CASH, STOCK_SPLIT, etc...)")]
        public IEnumerable<CorpActionType> CorpActionTypes { get; set; }
    }
}