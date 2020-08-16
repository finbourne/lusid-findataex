using System.Collections.Generic;
using System.IO;
using Lusid.FinDataEx.Core;

namespace Lusid.FinDataEx.Vendor.Bbg.Ftp
{
    public class BbgDlFtpExtractor : FdeBaseExtractor<BbgFtpRequest, BbgFtpResponse>
    {
        
        public const string RespTagNone = "NONE";
        public const string RespTagStartOfFile = "START-OF-FILE"; 
        public const string RespTagStartOfFields = "START-OF-FIELDS"; 
        public const string RespTagStartOfData = "START-OF-DATA"; 
        public const string RespTagEndOf = "END-OF-";

        private const int RespIndexTicker = 0;
        private const int RespIdColumns = 3;
        private const char RespDataDelimiter = '|';


        public BbgDlFtpExtractor(IVendorClient<BbgFtpRequest, BbgFtpResponse> vendorClient) : base(vendorClient)
        {
        }

        protected override BbgFtpRequest ToVendorRequest(FdeRequest request)
        {
            return new BbgFtpRequest(
                request.ConnectorConfig.url.Value,
                request.ConnectorConfig.user.Value,
                request.ConnectorConfig.password.Value,
                request.RequestBody.sourceData.Value
                );
        }

        protected override FdeResponse ToFdeResponse(BbgFtpResponse response)
        {
            // setup our response lists to populate and return
            List<string> finDataHeaders = new List<string>();
            finDataHeaders.Add("Ticker");
            List<List<string>> finData = new List<List<string>>();
            finData.Add(finDataHeaders);
            
            // iterate over vendor responses ensuring we apply right logic for different stages of the file
            string processingTag = RespTagNone;
            string[] entries = File.ReadAllLines(response.ResponseFileUrl);
            foreach (var entry in entries)
            {
                processingTag = UpdateProcessingTag(processingTag, entry);
                // all tag lines should be skipped
                if (entry == processingTag)
                {
                    continue;
                }
                switch (processingTag)
                {
                    // start tag of vendor response should be ignored as contains no required data
                    case (RespTagStartOfFile):
                        break;
                    // field tag data contains the headers of our extracted data
                    case (RespTagStartOfFields):
                        finDataHeaders.Add(entry);
                        break;
                    // data tag contains the extracted data formatted as per vendor standards and the fields requested
                    case (RespTagStartOfData):
                        finData.Add(ProcessDataEntry(entry));
                        break;
                }
            }
            
            FdeResponse fdeResponse = new FdeResponse(finData);
            return fdeResponse;
        }

        private string UpdateProcessingTag(string processingTag, string entry)
        {
            // if start of a new set of tags update to relevant tag and return
            switch (entry)
            {
                case (RespTagStartOfFile):
                case (RespTagStartOfFields):
                case (RespTagStartOfData):
                    return entry;
            }

            // if reached end of a set of tags let's reset to the NONE state
            if (entry.Trim() == "" || entry.StartsWith(RespTagEndOf))
            {
                return RespTagNone;
            }
            
            // otherwise remain unchanged
            return processingTag;
        }

        private List<string> ProcessDataEntry(string entry)
        {
            List<string> finDataEntry = new List<string>();
            string[] entryCols = entry.Split(RespDataDelimiter);
            if (entryCols.Length < RespIdColumns)
            {
                throw new InvalidDataException($"Invalid vendor response returned. Supported responses must have at least 3 identifier columns before finData. Bad entry={entry}");
            }
            finDataEntry.Add(entryCols[RespIndexTicker]);
            
            // Ignore last column as vendor provides a trailing delimiter
            for (int i = RespIdColumns; i < entryCols.Length - 1; i++)
            {
                finDataEntry.Add(entryCols[i]);
            }

            return finDataEntry;
        }
    }
}