using System.Collections.Generic;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Transform
{
    /// <summary>
    ///  Transformer of BBG DL responses into standardised FinDataOutput
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBbgResponseTransformer<in T> where T : PerSecurityResponse
    {
        /// <summary>
        ///  Transform a specific security repsonse from BBG DL (e.g. corp action response, prices response) into
        ///  a standard FinDataOutput format.
        /// </summary>
        /// <param name="perSecurityResponse">Response from BBG DLWS</param>
        /// <returns>FinDataOutput containing the BBG DL data from response</returns>
        List<FinDataOutput> Transform(T perSecurityResponse);
    }
}