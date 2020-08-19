using Lusid.FinDataEx.Vendor;

namespace Lusid.FinDataEx.Core
{
    public abstract class FdeBaseExtractor<TReq,TResp> : IFdeExtractor 
        where TReq : IVendorRequest
        where TResp : IVendorResponse 
    {

        private IVendorClient<TReq, TResp> VendorClient { get; }

        protected FdeBaseExtractor(IVendorClient<TReq, TResp> vendorClient)
        {
            VendorClient = vendorClient;
        }

        public IVendorResponse Extract(FdeRequest request)
        {
            TReq vendorRequest = ToVendorRequest(request);
            TResp vendorResponse =  VendorClient.Submit(vendorRequest);
            return vendorResponse;
        }

        protected abstract TReq ToVendorRequest(FdeRequest request);
        

    }
}