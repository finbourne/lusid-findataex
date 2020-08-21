using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lusid.FinDataEx.Util;
using static Lusid.FinDataEx.Util.DictionaryUtils;

namespace Lusid.FinDataEx.Vendor.Dl.Ftp
{
    public class DlFtpResponseBuilder
    {

        public const string RespTagNone = "NONE";
        public const string RespTagStartOfFile = "START-OF-FILE"; 
        public const string RespTagStartOfFields = "START-OF-FIELDS"; 
        public const string RespTagStartOfData = "START-OF-DATA";
        public const string RespTagEndOf = "END-OF-";
        public const string RespParamActions = "ACTIONS=";

        private const int RespIndexTicker = 0;
        private const int RespIdColumns = 3;
        private const int RespIdColumnsCorpAction = 4;
        private const int RespCorpActionColumn = 5;
        private const char RespDataDelimiter = '|';

        public DlFtpResponse CreateFromFile(DlRequestType dlRequestType, string dlResponseFileUrl)
        {
            string[] dlRequestFileEntries = LoadDlRequestEntriesFromFile(dlResponseFileUrl);
            switch (dlRequestType)
            {
                case DlRequestType.Prices:
                    return ToVendorResponseFromPrices(dlRequestFileEntries);
                case DlRequestType.CorpActions:
                    return ToVendorResponseFromCorpActions(dlRequestFileEntries);
                default:
                    throw new ArgumentException($"{dlRequestType} is not a currently supported DL request type.");
            }
        }
        
        private string[] LoadDlRequestEntriesFromFile(string responseFileUrl)
        {
            return File.ReadAllLines(responseFileUrl);
        }
        
        internal DlFtpResponse ToVendorResponseFromPrices(string[] dlResponseFileEntries)
        {
            // setup our response lists to populate and return
            List<string> finDataHeaders = new List<string>();
            finDataHeaders.Add("Ticker");
            List<List<string>> finData = new List<List<string>>();
            finData.Add(finDataHeaders);
            
            // flags to ensure response sections have been processed in correct order
            bool processedStartOfFile = false;
            bool processedStartOfFields = false;
            bool processedStartOfData= false;

            // iterate over vendor responses ensuring we apply right logic for different stages of the file
            string processingTag = RespTagNone;
            foreach (var entry in dlResponseFileEntries)
            {
                processingTag = UpdateProcessingTag(processingTag, entry);
                // all tag lines should be skipped
                if (entry == processingTag)
                {
                    continue;
                }
                switch (processingTag)
                {
                    case (RespTagStartOfFile):
                        processedStartOfFile = true;
                        break;
                    // field tag data contains the headers of our extracted data
                    case (RespTagStartOfFields):
                        ValidateTagOrder(processedStartOfFile, RespTagStartOfFile, RespTagStartOfFields);
                        processedStartOfFields = true;
                        finDataHeaders.Add(entry);
                        break;
                    // data tag contains the extracted data formatted as per vendor standards and the fields requested
                    case (RespTagStartOfData):
                        processedStartOfData = true;
                        ValidateTagOrder(processedStartOfFields, RespTagStartOfFields, RespTagStartOfData);
                        string[] entryCols = SplitDlEntry(entry);
                        finData.Add(ProcessPriceDataEntry(entryCols, finDataHeaders));
                        break;
                    
                }
            }
            
            ValidateTagOrder(processedStartOfData,  RespTagStartOfData, RespTagEndOf);
            
            Dictionary<string, List<List<string>>> pricesDataMap = new Dictionary<string, List<List<string>>>
            {
                {DlRequestType.Prices.ToString(), finData}
            };
            return new DlFtpResponse(pricesDataMap);
        }
        
        
        internal DlFtpResponse ToVendorResponseFromCorpActions(string[] dlResponseFileEntries)
        {
            Dictionary<String,List<List<string>>> corpActionDataMap = new Dictionary<string, List<List<string>>>();
            
            // setup our response lists to populate and return
            List<List<string>> finData = new List<List<string>>();

            // flags to ensure response sections have been processed in correct order
            bool processedStartOfFile = false;
            bool processedStartOfData= false;

            // iterate over vendor responses ensuring we apply right logic for different stages of the file
            string processingTag = RespTagNone;
            foreach (var entry in dlResponseFileEntries)
            {
                processingTag = UpdateProcessingTag(processingTag, entry);
                // all tag lines should be skipped
                if (entry == processingTag)
                {
                    continue;
                }
                switch (processingTag)
                {
                    case (RespTagStartOfFile):
                        processedStartOfFile = true;
                        break;
                    
                    // field tag data contains the headers of our extracted data
                    case (RespTagStartOfFields):
                        throw new InvalidDataException("Invalid vendor DL response. Corporate actions response should not include" +
                                                       " request DL fields.");
                    
                    // data tag contains the extracted data formatted as per vendor standards and the fields requested
                    case (RespTagStartOfData):
                        ValidateTagOrder(processedStartOfFile, RespTagStartOfFile, RespTagStartOfData);
                        processedStartOfData = true;
                        string[] entryCols = SplitDlEntry(entry);
                        if (HasCorporateActions(entryCols))
                        {
                            string corpActionType = entryCols[RespCorpActionColumn];
                            List<List<string>> corpActionData = corpActionDataMap.GetOrCreateAndPut(corpActionType);
                            List<string> corpActionEntry = ProcessCorpActionDataEntry(entryCols);
                            corpActionData.Add(corpActionEntry);
                        }
                        break;
                }
            }
            // ensure processed data
            ValidateTagOrder(processedStartOfData,  RespTagStartOfData, RespTagEndOf);
            
            // populate headers for each corporate action type
            PopulateCorporateActionsHeaders(corpActionDataMap);

            return new DlFtpResponse(corpActionDataMap);
        }

        internal string UpdateProcessingTag(string processingTag, string entry)
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
            
            // otherwise remain unchanged in current tag and process entry
            return processingTag;
        }
        
        private List<string> ProcessPriceDataEntry(string[] entry, List<string> finDataHeaders)
        {
            List<string> finDataEntry = new List<string>();
            // Add ticker column
            finDataEntry.Add(entry[RespIndexTicker]);
            
            for (int i = RespIdColumns; i < entry.Length; i++)
            {
                finDataEntry.Add(entry[i]);
            }
            
            if (finDataEntry.Count != finDataHeaders.Count)
            {
                throw new InvalidDataException($"Invalid vendor response returned. Expected {finDataHeaders.Count} columns but only received {finDataEntry.Count} data columns.");
            }

            return finDataEntry;
        }
        
        private List<string> ProcessCorpActionDataEntry(string[] entry)
        {
            List<string> corpActionDataEntry = new List<string>();
            // columns in each entry = ticker (1) + meta cols (2) + data_columns
            // RespIdCols and Headers both include ticker column hence the -1
            if (entry.Length < RespIdColumnsCorpAction)
            {
                throw new InvalidDataException($"Invalid vendor response returned. Expected at least {RespIdColumns} columns but only received {entry.Length}");
            }
            // add ticker
            corpActionDataEntry.Add(entry[RespIndexTicker]);
            
            for (int i = RespIdColumnsCorpAction; i < entry.Length; i++)
            {
                corpActionDataEntry.Add(entry[i]);
            }

            return corpActionDataEntry;
        }

        private string[] SplitDlEntry(string entry)
        {
            // note dl responses have a trailing delimiter hence the need to remove last char.
            return entry.Remove(entry.Length-1).Split(RespDataDelimiter);
        }
        
        private bool HasCorporateActions(string[] entry)
        {
            return entry.Length >= RespIdColumnsCorpAction;
        }

        private void ValidateTagOrder(bool preReqTag, string preReqName, string tag)
        {
            if (!preReqTag)
            {
                throw new InvalidDataException("Invalid vendor response request. " +
                                               $"Attempting to process {tag} data without having processed {preReqName}.");
            }
        }

        /// <summary>
        /// Populate the corporate action headers. Currently these are automatically generated until more details are available
        /// from DL docs on specifics of the headers for each doc.
        /// 
        /// </summary>
        /// <param name="corpActionsDataMap"></param>
        /// <returns></returns>
        private void PopulateCorporateActionsHeaders(Dictionary<string,List<List<string>>> corpActionsDataMap)
        {
            foreach (KeyValuePair<string,List<List<string>>> corpActionsData in corpActionsDataMap)
            {
                string corpActionType = corpActionsData.Key;
                List<List<string>> corpActionsEntries = corpActionsData.Value;
                if (corpActionsEntries != null && corpActionsEntries.Any())
                {
                    int colCount = corpActionsEntries.First().Count;
                    List<string> corpActionHeaders = new List<string>();
                    corpActionHeaders.Add("Ticker");
                    // start at 1 to exclude ticker column
                    for (int i = 1; i < colCount; i++)
                    {
                        corpActionHeaders.Add($"CorpActionHeader_{i}");
                    }
                    // Unlikely size of list will greater than order of 10k. If so look at moving to LinkedList if this append
                    // starts slowing down.
                    corpActionsEntries.Insert(0, corpActionHeaders);
                }
            }
        }
    }
}