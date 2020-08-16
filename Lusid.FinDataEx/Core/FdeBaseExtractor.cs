using Lusid.FinDataEx.Vendor;
using Lusid.FinDataEx.Vendor.Bbg;

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

        public FdeResponse Extract(FdeRequest request)
        {
            TReq wsSubmitGetDataRequest = ToVendorRequest(request);
            TResp wsRetrieveGetDataResponse =  VendorClient.Submit(wsSubmitGetDataRequest);
            FdeResponse fdeResponse = ToFdeResponse(wsRetrieveGetDataResponse);
            return fdeResponse;
        }

        protected abstract TReq ToVendorRequest(FdeRequest request);

        protected abstract FdeResponse ToFdeResponse(TResp response);
        

    }
}