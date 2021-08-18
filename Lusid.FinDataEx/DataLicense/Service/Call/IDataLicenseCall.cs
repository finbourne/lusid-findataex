using Lusid.FinDataEx.DataLicense.Util;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Call
{
    /// <summary>
    /// A BBG DL Webservice call (e.g. reference data call, price call,
    /// corporate actions call)
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataLicenseCall<out T> where T : PerSecurityResponse 
    {
        /// <summary>
        /// Executes a call against DLWS and returns a BBG DL response for
        /// a list of instruments. Responses include instrument data and status codes
        /// depending on the results of the call 
        /// </summary>
        /// <param name="instruments">Instruments requesting data against.</param>
        /// <returns>Response with requested data for the instruments if it could be provided.</returns>
        T Get(Instruments instruments);

        DataLicenseTypes.DataTypes GetDataType();
    }
}