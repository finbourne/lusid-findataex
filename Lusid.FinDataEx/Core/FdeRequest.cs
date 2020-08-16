namespace Lusid.FinDataEx.Core
{
    public class FdeRequest
    {
        public string Uid { get; }
        public string CallerId { get; }
        public string Vendor { get; }
        /// <summary>
        /// Configuration required to connect to the vendor data source
        /// </summary>
        public dynamic ConnectorConfig { get; } 
        public dynamic OutputConfig { get; }
        public dynamic RequestBody { get; }

        
    }
}