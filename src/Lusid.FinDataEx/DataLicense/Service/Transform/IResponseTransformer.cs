using PerSecurity_Dotnet;
using System.Collections.Generic;

namespace Lusid.FinDataEx.DataLicense.Service.Transform
{
    public interface IResponseTransformer
    {
        List<Dictionary<string, string>> Transform(PerSecurityResponse perSecurityResponse);
    }
}