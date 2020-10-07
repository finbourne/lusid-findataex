using System.Collections.Generic;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Transform
{
    public interface IBbgResponseTransformer<in T> where T : PerSecurityResponse
    {
        List<FinDataOutput> Transform(T perSecurityResponse);
    }
}