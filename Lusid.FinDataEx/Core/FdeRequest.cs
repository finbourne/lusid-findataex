using System.Collections.Generic;

namespace Lusid.FinDataEx.Core
{
    public class FdeRequest
    {
        public string Uid { get; set; }
        public string CallerId { get; set;}
        public string Output { get; set; }
        public string Vendor { get; set;}
        /// <summary>
        /// Configuration required to connect to the vendor data source
        /// </summary>
        public Dictionary<string,object> ConnectorConfig { get; set;} 
        public Dictionary<string,object> OutputConfig { get; set;}
        public Dictionary<string,object> RequestBody { get; set;}

        
    }
}