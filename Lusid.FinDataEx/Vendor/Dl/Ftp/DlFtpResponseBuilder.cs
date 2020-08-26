using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lusid.FinDataEx.Util;

namespace Lusid.FinDataEx.Vendor.Dl.Ftp
{
    /// <summary>
    /// Loads in and parses DL files output from DL ftp server after a request
    /// has been processed.
    ///
    /// Currently supports price, instrument reference, and corporate action data retrieval.
    /// 
    /// </summary>
    public class DlFtpResponseBuilder
    {

        /* Character to identify comment line in DL responses */
        private const char DlCommentChar = '#';
        /* DL Section Tags required to correctly identify data for parsing from BBG files*/
        private const string DlSectionTagNone = "NONE";
        private const string DlSectionTagStartOfFile = "START-OF-FILE";
        private const string DlSectionTagStartOfFields = "START-OF-FIELDS"; 
        private const string DlSectionTagStartOfData = "START-OF-DATA";
        private const string DlSectionTagEndOf = "END-OF-";
        private const string DlSectionTagTimeStarted = "TIMESTARTED=";
        private const string DlSectionTagTimeFinished = "TIMEFINISHED=";

        private const int RespTickerIndex = 0;
        private const int RespCorpActionColumnIndex = 5;
        private const int RespNoOfIdColumns = 3;
        private const int RespNoIdColumnsCorpAction = 5;
        private const char RespDataDelimiter = '|';

        /// <summary>
        /// Load a DL response file returned from DL ftp server and parse contents into
        /// a DlRequestType.
        /// 
        /// </summary>
        /// <param name="dlRequestType">Type of DL request being processed (e.g. Corporate Action, Price Information, etc...)</param>
        /// <param name="dlResponseFileUrl">Location of decrypted DL response file returned from DL ftp server</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"> on unrecognised DL request type</exception>
        public DlFtpResponse CreateFromFile(DlRequestType dlRequestType, string dlResponseFileUrl)
        {
            string[] dlRequestFileEntries = LoadDlRequestEntriesFromFile(dlResponseFileUrl);
            switch (dlRequestType)
            {
                case DlRequestType.Prices:
                    return CreatePricesResponse(dlRequestFileEntries);
                case DlRequestType.CorpActions:
                    return CreateCorpActionsResponse(dlRequestFileEntries);
                default:
                    throw new ArgumentException($"{dlRequestType} is not a currently supported DL request type.");
            }
        }
        
        /// <summary>
        ///  Process and validate the contents of a DL response file containing price information. Expects a strict structure
        ///  for DL price responses to follow. 
        /// 
        /// </summary>
        /// <param name="dlResponseFileEntries"> contents of dl response from dl server</param>
        /// <returns>a valid vendor response</returns>
        internal DlFtpResponse CreatePricesResponse(string[] dlResponseFileEntries)
        {
            // Setup headers and price data containers to populate from DL response
            List<string> priceDataHeaders = new List<string>{"Ticker"};
            List<List<string>> priceData = new List<List<string>> {priceDataHeaders};
            
            // flags to validate price files are processed in the expected order.
            bool processedStartOfFile = false;
            bool processedStartOfFields = false;
            bool processedStartOfData= false;

            // Process the price data response in expected order -> {START-OF-FILE, START-OF-FIELDS, START-OF-DATA}
            string currentProcessingTag = DlSectionTagNone;
            foreach (var dlResponseEntry in dlResponseFileEntries)
            {
                currentProcessingTag = UpdateProcessingTag(currentProcessingTag, dlResponseEntry);
                
                // processing tag entries should be ignored
                if (dlResponseEntry == currentProcessingTag)
                {
                    continue;
                }
                
                // otherwise process the entry
                switch (currentProcessingTag)
                {
                    // price responses contain no required data within the Start tags.
                    case (DlSectionTagStartOfFile):
                        processedStartOfFile = true;
                        break;
                    // price responses contain header information under the Field tags.
                    case (DlSectionTagStartOfFields):
                        CheckTagInCorrectOrder(processedStartOfFile, DlSectionTagStartOfFile, DlSectionTagStartOfFields);
                        processedStartOfFields = true;
                        priceDataHeaders.Add(dlResponseEntry);
                        break;
                    // price responses contain price data under the Data tags.
                    case (DlSectionTagStartOfData):
                        CheckTagInCorrectOrder(processedStartOfFields, DlSectionTagStartOfFields, DlSectionTagStartOfData);
                        if (IsDlCommentEntry(dlResponseEntry))
                        {
                            break;
                        }
                        processedStartOfData = true;
                        string[] splitDlEntry = SplitDlEntry(dlResponseEntry);
                        priceData.Add(ProcessPriceDataEntry(splitDlEntry, priceDataHeaders));
                        break;
                }
            }
            
            // price response should have processed a Data tag otherwise the response is not supported.
            CheckTagInCorrectOrder(processedStartOfData,  DlSectionTagStartOfData, DlSectionTagEndOf);
            
            // create response
            Dictionary<string, List<List<string>>> pricesDataMap = new Dictionary<string, List<List<string>>>
            {
                {DlRequestType.Prices.ToString(), priceData}
            };
            return new DlFtpResponse(pricesDataMap);
        }
        
        /// <summary>
        ///  Process and validate the contents of a DL response file containing corporate action information. Expects a strict structure
        ///  for DL corporate action responses to follow. 
        ///
        ///  Note while this method has similarities to CreatePricesResponse there are enough differences to warrant keeping them apart. Especially
        ///  given the fact it's likely the Corporate Actions use case will become more complex going forward.
        /// 
        /// </summary>
        /// <param name="dlResponseFileEntries"> contents of dl response from dl server</param>
        /// <returns>a valid vendor response</returns>
        internal DlFtpResponse CreateCorpActionsResponse(string[] dlResponseFileEntries)
        {
            // Setup corp action data containers to populate from DL response.
            // Corp action data will be split by corp action type as their corresponding output structures will differ.
            Dictionary<String,List<List<string>>> corpActionDataMap = new Dictionary<string, List<List<string>>>();
            
            // flags to validate price files are processed in the expected order.
            bool processedStartOfFile = false;
            bool processedStartOfData= false;

            // Process the price data response in expected order -> {START-OF-FILE, START-OF-DATA}
            // Note : Corp action responses do not contain START-OF-FIELDS information.
            string currentProcessingTag = DlSectionTagNone;
            foreach (var dlResponseEntry in dlResponseFileEntries)
            {
                currentProcessingTag = UpdateProcessingTag(currentProcessingTag, dlResponseEntry);
                // processing tag entries should be ignored
                if (dlResponseEntry == currentProcessingTag)
                {
                    continue;
                }
                switch (currentProcessingTag)
                {
                    // corp actions responses contain no required data within the Start tags.
                    case (DlSectionTagStartOfFile):
                        processedStartOfFile = true;
                        break;
                    
                    // corp actions responses do not support field tags
                    case (DlSectionTagStartOfFields):
                        throw new InvalidDataException("Invalid vendor DL response. Corporate actions response should not include" +
                                                       " request DL fields.");
                    
                    // corp action responses contain price data under the Data tags.
                    case (DlSectionTagStartOfData):
                        CheckTagInCorrectOrder(processedStartOfFile, DlSectionTagStartOfFile, DlSectionTagStartOfData);
                        processedStartOfData = true;
                        if (IsDlCommentEntry(dlResponseEntry))
                        {
                            break;
                        }
                        string[] entryCols = SplitDlEntry(dlResponseEntry);
                        if (HasCorporateActions(entryCols))
                        {
                            string corpActionType = entryCols[RespCorpActionColumnIndex];
                            List<List<string>> corpActionData = corpActionDataMap.GetOrCreateAndPut(corpActionType);
                            List<string> corpActionEntry = ProcessCorpActionDataEntry(entryCols);
                            corpActionData.Add(corpActionEntry);
                        }
                        break;
                }
            }
            
            // Corp action response should have processed a Data tag otherwise the response is not supported.
            CheckTagInCorrectOrder(processedStartOfData,  DlSectionTagStartOfData, DlSectionTagEndOf);
            
            // As no Field tags are included in corp action DL responses manually populate headers for each corporate action type
            PopulateCorporateActionsHeaders(corpActionDataMap);

            return new DlFtpResponse(corpActionDataMap);
        }

        /// <summary>
        /// Check if a processing stage has complete and parsing has  arrived at a new stage (e.g. from START-OF-FIELDS to
        /// START-OF-DATA) otherwise continue in current processing stage.
        /// 
        /// Processing tags inform the current stage of DL request parsing and the parsing logic.   
        /// 
        /// </summary>
        /// <param name="currentProcessingTag"> current stage of parsing of the DL request</param>
        /// <param name="entry"> current line being parsed from the DL request</param>
        /// <returns>the latest processing tag</returns>
        private string UpdateProcessingTag(string currentProcessingTag, string entry)
        {
            // if arrived at a processing tag then return to update our current processing tag
            switch (entry)
            {
                case (DlSectionTagStartOfFile):
                case (DlSectionTagStartOfFields):
                case (DlSectionTagStartOfData):
                    return entry;
            }
            
            // if it's a meta time stamping DL entry then log only
            if (entry.StartsWith(DlSectionTagTimeStarted) || entry.StartsWith(DlSectionTagTimeFinished))
            {
                Console.WriteLine(entry);
                return entry;
            }
            
            // if reached a termination condition for a processing tag let's reset to the NONE state
            // until a the next processing tag
            if (entry.Trim() == "" || entry.StartsWith(DlSectionTagEndOf))
            {
                return DlSectionTagNone;
            }
            
            // otherwise the entry is not a processing tag and our current processing tag remains unchanged
            return currentProcessingTag;
        }
        
        /// <summary>
        ///
        /// Parse a DL price data response entry into an fde price entry 
        /// 
        /// </summary>
        /// <param name="entry"> current pricing line being parsed from the DL request</param>
        /// <param name="priceDataHeaders"></param>
        /// <returns> pricing data</returns>
        /// <exception cref="InvalidDataException"> if pricing data entry does not match the expected headers</exception>
        private List<string> ProcessPriceDataEntry(string[] entry, List<string> priceDataHeaders)
        {
            // initiate a new price data entry starting with the ticker
            var priceDataEntry = new List<string>{entry[RespTickerIndex].Trim()};
            
            // construct the output price data ignoring DL id columns
            for (var i = RespNoOfIdColumns; i < entry.Length; i++)
            {
                priceDataEntry.Add(entry[i].Trim());
            }
            
            if (priceDataEntry.Count != priceDataHeaders.Count)
            {
                throw new InvalidDataException($"Invalid vendor response returned. Expected {priceDataHeaders.Count} columns but only received {priceDataEntry.Count} data columns.");
            }

            return priceDataEntry;
        }
        
        /// <summary>
        ///
        /// Parse corporate action data response entry into an fde price entry 
        /// 
        /// </summary>
        /// <param name="entry"> current corporate action line being parsed from the DL request</param>
        /// <returns> corporate action data</returns>
        /// <exception cref="InvalidDataException"> if corporate data entry does not contain any corporate action data</exception>
        private List<string> ProcessCorpActionDataEntry(string[] entry)
        {
            if (entry.Length < RespNoIdColumnsCorpAction)
            {
                throw new InvalidDataException($"Invalid vendor response returned. Expected at least {RespNoOfIdColumns} columns but only received {entry.Length}");
            }

            // initiate a new corp action data entry starting with the ticker
            List<string> corpActionDataEntry = new List<string>{entry[RespTickerIndex].Trim()};
            for (var i = RespNoIdColumnsCorpAction; i < entry.Length; i++)
            {
                corpActionDataEntry.Add(entry[i].Trim());
            }

            return corpActionDataEntry;
        }

        /// <summary>
        /// Ensure the processing of the DL request is running in the correct order. Incorrect order suggests either
        /// error in DL response or an unexpected structure that needs to be addressed.
        /// 
        /// </summary>
        /// <param name="preReqTag"> has the prerequisite tag been parsed</param>
        /// <param name="preReqName">the name of processing tag expecting to have been parsed<</param>
        /// <param name="tag">the current processing tag that has started to be parsed</param>
        /// <exception cref="InvalidDataException">if processing tag is being parsed before a previous processing
        ///  tag is expected to have been parsed.</exception>
        private void CheckTagInCorrectOrder(bool preReqTag, string preReqName, string tag)
        {
            if (!preReqTag)
            {
                throw new InvalidDataException("Invalid vendor response request. " +
                                               $"Attempting to process {tag} data without having processed {preReqName}.");
            }
        }

        /// <summary>
        /// Populate the corporate action headers. Currently these are automatically generated until more details are available
        /// from DL docs on specifics of the headers for each type.
        /// 
        /// </summary>
        /// <param name="corpActionsDataMap"> corporate action data that has been parsed</param>
        /// <returns></returns>
        private void PopulateCorporateActionsHeaders(Dictionary<string,List<List<string>>> corpActionsDataMap)
        {
            foreach (var corpActionsData in corpActionsDataMap)
            {
                var corpActionsEntries = corpActionsData.Value;
                if (corpActionsEntries != null && corpActionsEntries.Any())
                {
                    int colCount = corpActionsEntries.First().Count;
                    List<string> corpActionHeaders = new List<string>{"Ticker"};
                    // start at 1 to exclude ticker column and pad out remaining columns with dummy headers.
                    for (var i = 1; i < colCount; i++)
                    {
                        corpActionHeaders.Add($"CorpActionHeader_{i}");
                    }
                    // if size of requests grow significantly (unlikely) move to using a LinkedList
                    corpActionsEntries.Insert(0, corpActionHeaders);
                }
            }
        }
        
        private string[] LoadDlRequestEntriesFromFile(string responseFileUrl)
        {
            return File.ReadAllLines(responseFileUrl);
        }
        
        private string[] SplitDlEntry(string entry)
        {
            // remove last char before splitting as DL response contains a trailing delimitter "|"
            return entry.Remove(entry.Length-1).Split(RespDataDelimiter);
        }
        
        /// <summary>
        /// As per DL docs (ref: "4.13.1 No Corporate Action for a Given Security") no corporate actions is reflected with an entry column 
        /// in which only request id details are added. Any additional columns means corp actions have been returned.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private bool HasCorporateActions(string[] entry)
        {
            return entry.Length > RespNoIdColumnsCorpAction;
        }
        
        private bool IsDlCommentEntry(string entry)
        {
            return entry.StartsWith(DlCommentChar);
        }
    }
}