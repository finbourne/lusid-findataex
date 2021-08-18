﻿using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Transform
{
    /// <summary>
    ///  Transformer of BBG DL responses into standardised FinDataOutput
    /// </summary>
    /// <typeparam name="T">BBG DL Response Type</typeparam>
    public interface IDataLicenseResponseTransformer<in T> where T : PerSecurityResponse
    {
        /// <summary>
        ///  Transform a specific security response from BBG DL (e.g. corp action response, prices response) into
        ///  a standard FinDataOutput format.
        /// </summary>
        /// <param name="perSecurityResponse">Response from BBG DLWS</param>
        /// <returns>FinDataOutput containing the BBG DL data from response</returns>
        DataLicenseOutput Transform(T perSecurityResponse);
        
    }
}