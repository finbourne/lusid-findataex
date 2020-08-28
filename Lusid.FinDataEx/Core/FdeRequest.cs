using System.Collections.Generic;

namespace Lusid.FinDataEx.Core
{
    /// <summary>
    /// Standard request for a financial data extract. Contains all the information required
    /// to retrieve financial data from a supported vendor and then process the data
    /// 
    /// </summary>
    public class FdeRequest
    {
        /// <summary> unique id of the request </summary>
        public string Uid { get; set; }

        /// <summary> id of the application making the request </summary>
        public string CallerId { get; set;}
        
        /// <summary> the output processor used to process the response of financial
        /// data from a vendor (e.g. lusidtools)</summary>
        public string Output { get; set; }
        
        /// <summary> the data vendor used to retrieve data </summary>
        public string Vendor { get; set;}
        
        /// <summary> configuration required to connect to the vendor data source</summary>
        public Dictionary<string,object> ConnectorConfig { get; set;} 
        
        /// <summary> contains all the request parameters required to fetch data from the vendor source</summary>
        public Dictionary<string,object> RequestBody { get; set;}

        
    }
}