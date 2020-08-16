namespace Lusid.FinDataEx.Core
{
    public class FdeRequest
    {
        public string Uid { get; set; }
        public string CallerId { get; set;}
        public string Vendor { get; set;}
        /// <summary>
        /// Configuration required to connect to the vendor data source
        /// </summary>
        public dynamic ConnectorConfig { get; set;} 
        public dynamic OutputConfig { get; set;}
        public dynamic RequestBody { get; set;}

        
    }
}