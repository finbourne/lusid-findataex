namespace Lusid.FinDataEx.DataLicense.Util
{
    /// <summary>
    ///  Container class for BBG DLWS related enums
    /// </summary>
    public static class DataLicenseTypes
    {
        /// <summary>
        ///  Different sets of data types supported by BBG DL and FinDataEx (e.g. Corporate Actions
        /// (GetActions), security reference data (GetData).
        /// </summary>
        public enum DataTypes
        {
            GetData,
            GetActions
        }

        /// <summary>
        /// ProgramTypes supported by BBG DLWS and FinDataEx. Decide on the operation mode which impacts
        /// behaviour of the call to BBG DL (e.g. running job at specific time, delay in job time, etc...)
        /// </summary>
        public enum ProgramTypes
        {
            Adhoc,
            Scheduled
        }

        /// <summary>
        ///  Corporate Actions supported by BBG DLWS and FinDataEx.
        ///  Currently using BBG DL naming conventions so do NOT amend unless also amended by BBG DL.
        /// </summary>
        public enum CorpActionType
        {
            DVD_CASH,
            DVD_STOCK,
            STOCK_SPLT
        }
    }
}