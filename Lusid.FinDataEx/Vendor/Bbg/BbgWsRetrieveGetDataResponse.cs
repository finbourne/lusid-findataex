using System.Collections.Generic;

namespace Lusid.FinDataEx.Vendor.Bbg
{
    struct StatusCode{
        
    }
    public class BbgWsRetrieveGetDataResponse : IVendorRequest
    {
        public List<string> RequestHeader { get; }
        public List<string> Fields { get; }
        public List<string> EnrichedSecurities { get; }
        public string TimeStarted { get; }
        public string TimeFinsihed { get; }

    }
}