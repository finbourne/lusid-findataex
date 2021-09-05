﻿using System.Collections.Generic;
using CommandLine;
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
         * Input
         */
        [Option('i', "instrument-source",
            Group = "input",
            SetName = "instrumentInput",
            Required = false,
            Default = "InstrumentSource",
            HelpText = "Instrument source to create the instruments to query against DataLicense. Supported types include : " +
                       "[InstrumentSource, LusidPortfolioInstrumentSource, FromDriveCsvInstrumentSource, FromLocalCsvInstrumentSource]. " +
                       "Developers can add custom instrument sources as required, see FinDataEx readme for details.")]
        public string InstrumentSource { get; set; }

        [Option('a', "instrument-source-args",
            SetName = "instrumentInput",
            Required = false,
            HelpText = "Arguments passed to the instrument source for retrieving instruments to query against DataLicense.")]
        public IEnumerable<string> InstrumentSourceArguments { get; set; }

        [Option('t', "instrument_id_type",
            SetName = "instrumentInput",
            Required = false,
            Default = InstrumentType.BB_GLOBAL,
            HelpText = "Type of instrument ids being input (BB_GLOBAL (Figi), ISIN, CUSIP)")]
        public InstrumentType InstrumentIdType { get; set; }

        [Option('b', "bbg-source",
            SetName = "bbgInput",
            Group = "input",
            Required = false,
            HelpText = "BBG data csv file containing Data or Actions input. Paths must be for LUSID Drive")]
        public string BBGSource { get; set; }

        /*
         * Output
         */
        [Option("output-local",
            Group = "output",
            Required = false,
            HelpText = "File path to write DLWS output. Include  \"{REQUEST_ID}\", \"{AS_AT}\", \"{AS_AT_DATE}\" in the filename " +
                       " to include the DL request id timestamps respectively in the filename (e.g. " +
                       "/home/dl_results/MySubmission_{REQUEST_ID}_{AS_AT}.csv")]
        public string OutputLocal { get; set; }

        [Option("output-drive",
            Group = "output",
            Required = false,
            HelpText = "File path to write DLWS output. Include  \"{REQUEST_ID}\", \"{AS_AT}\", \"{AS_AT_DATE}\" in the filename " +
                       " to include the DL request id timestamps respectively in the filename (e.g. " +
                       "/home/dl_results/MySubmission_{REQUEST_ID}_{AS_AT}.csv")]
        public string OutputDrive { get; set; }

        [Option("output-lusid",
            Group = "output",
            Required = false,
            HelpText = "The corporate action source to upsert to. Written as \"{SCOPE}:{CODE}\"")]
        public string OutputLusid { get; set; }

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
            HelpText = "Running without the safety will allow billable calls to be sent to BBG, rather than just echoing the DL request.")]
        public bool EnableLiveRequests { get; set; }

        [Option('m', "max_instruments",
            Default = 50,
            HelpText = "Set the maximum number of instruments allowed in a BBG DLWS call. Especially important in" +
                       " production environments that are billed per instrument.")]
        public int MaxInstruments { get; set; }
    }

    /// <summary>
    /// Options for GetData calls to BBG
    /// </summary>
    [Verb("getdata", HelpText = "BBG DL request to retrieve data for requested set of fields and insturments.")]
    class GetDataOptions : DataLicenseOptions
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
    class GetActionsOptions : DataLicenseOptions
    {
        [Option('c', "corpactions",
            Required = true,
            HelpText = "The corporate action types to retrieve (e.g. DVD_CASH, STOCK_SPLIT, etc...)")]
        public IEnumerable<CorpActionType> CorpActionTypes { get; set; }
    }
}