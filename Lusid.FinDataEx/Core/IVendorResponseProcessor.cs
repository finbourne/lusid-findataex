using Lusid.FinDataEx.Vendor;

namespace Lusid.FinDataEx.Core
{
    public interface IVendorResponseProcessor
    {
        void ProcessResponse(FdeRequest fdeRequest, IVendorResponse vendorResponse);
    }
}