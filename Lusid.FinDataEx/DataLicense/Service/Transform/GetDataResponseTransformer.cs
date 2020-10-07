using System.Collections.Generic;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Transform
{
    public class GetDataResponseTransformer : IBbgResponseTransformer<RetrieveGetDataResponse>
    {
        public List<FinDataOutput> Transform(RetrieveGetDataResponse perSecurityResponse)
        {
            return new List<FinDataOutput>();
        }
    }
}