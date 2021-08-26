using System.Collections.Generic;

namespace Lusid.FinDataEx.Output
{
    /// <summary>
    /// Writer of FinDataOutput to an output destination  
    /// 
    /// </summary>
    public interface IOutputWriter
    {

        public const char BbgDlDelimiter = '|';

        /// <summary>
        /// Write financial data retrieved from BBG DLWS in form
        /// of FinDataOutputs to an output destination
        /// 
        /// </summary>
        /// <param name="dataLicenseOutput">Financial data to write</param>
        /// <returns>Result status of the write</returns>
        WriteResult Write(DataLicenseOutput dataLicenseOutput);
    }
}