using System.Collections.Generic;

namespace Lusid.FinDataEx
{
    /// <summary>
    /// Standardised container for responses returned from BBG DLWS calls.
    /// 
    /// </summary>
    public class DataLicenseOutput
    {
        /// <summary>Id of the specific BBG DL request for data</summary>
        public string Id { get; }
        
        /// <summary>Headers for requested data from BBG DL</summary>
        public IEnumerable<string> Header { get; }
        
        /// <summary>Financial data for each of the instruments requested from BBG DL</summary>
        public IList<Dictionary<string,string>> Records { get; }

        public DataLicenseOutput(string id, IEnumerable<string> header, IList<Dictionary<string, string>> records)
        {
            Id = id;
            Header = header;
            Records = records;
        }
    }
}