using System;

namespace Lusid.FinDataEx.Util
{
    /// <summary>
    /// Utility class to support parsing AutoGenPatterns which are used to construct filenames with
    /// components that are generated at runtime
    ///
    /// e.g
    ///     - Generating an output file name that contains the timestamp
    ///     - Retrieving an instrument source file that changes on each day so requires date when constructing
    ///       the file.
    /// </summary>
    public static class AutoGenPatternUtils
    {
        public const string RequestIdPattern = "{REQUEST_ID}";
        public const string AsAtKeyPattern = "{AS_AT}";
        public const string AsAtDatePattern = "{AS_AT_DATE}";
        public const string TestPattern = "{TEST}";

        public static string ApplyAsAt(string filePath)
        {
            return filePath.Replace(AsAtKeyPattern,
                DateTime.Now.ToUniversalTime().ToString("yyyyMMdd_HHmmssFFF"));
        }

        public static string ApplyAsAtDate(string filePath)
        {
            return filePath.Replace(AsAtDatePattern,
                DateTime.Now.ToUniversalTime().ToString("yyyyMMdd"));
        }
        
        public static string ApplyDataLicenseRequestId(string filePath, string dataLicenseRequestId)
        {
            return filePath.Replace(RequestIdPattern, dataLicenseRequestId);
        }

        public static string ApplyAllPatterns(string filePath, string dataLicenseRequestId)
        {
            return ApplyDataLicenseRequestId(ApplyAsAtDate(ApplyAsAt(ApplyTestPattern(filePath))), dataLicenseRequestId);
        }
        
        public static string ApplyDateTimePatterns(string filePath)
        {
            return ApplyAsAtDate(ApplyAsAt(ApplyTestPattern(filePath)));
        }
        
        /// <summary>
        ///  Replacement used for integration and unit testing where need static replacement values 
        /// </summary>
        public static string ApplyTestPattern(string filePath)
        {
            return filePath.Replace(TestPattern, "TestAutoGenPattern");
        }

    }
}