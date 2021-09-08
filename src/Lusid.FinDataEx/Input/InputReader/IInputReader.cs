namespace Lusid.FinDataEx.Input
{
    /// <summary>
    /// Reader of FinDataInput from an input destination
    /// </summary>
    public interface IInputReader
    {
        public const char CsvDelimiter = ',';

        /// <summary>
        /// Read financial data retrieved from an
        /// FinDataInputs to an output destination
        /// </summary>
        /// <returns>Financial data to write</returns>
        DataLicenseOutput Read();
    }
}