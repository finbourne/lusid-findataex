namespace Lusid.FinDataEx.DataLicense.Util
{
    /// <summary>
    ///  Container class for BBG DLWS related enums
    /// </summary>
    public class DlTypes
    {
        /// <summary>
        ///  Different sets of data types supported by BBG DL and FinDataEx (e.g. Corporate Actions
        /// (GetActions), security reference data (GetData).
        /// </summary>
        public enum DataTypes
        {
            GetData,
            GetActions,
            GetPrices
        }
        
        /// <summary>
        /// ProgramTypes supported by BBG DLWS and FinDataEx. Decide on the operation mode whic impacts
        /// behaviour of the call to BBG DL (e.g. running job at specific time, delay in job time, etc...)
        /// </summary>
        public enum ProgramTypes
        {
            Adhoc,
            Scheduled
        }
    }
}