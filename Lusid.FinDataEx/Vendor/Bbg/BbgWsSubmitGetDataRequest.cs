using System.Collections.Generic;

namespace Lusid.FinDataEx.Vendor.Bbg
{
    public class BbgWsSubmitGetDataRequest : IVendorResponse
    {
        public List<string> RequestHeader { get; }
        public List<string> Fields { get; }
        public List<string> Securities { get; }

    }
}