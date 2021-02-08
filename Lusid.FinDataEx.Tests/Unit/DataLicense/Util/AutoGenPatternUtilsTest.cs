using System;
using System.Globalization;
using Lusid.FinDataEx.Util;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Unit.DataLicense.Util
{
    [TestFixture]
    public class AutoGenPatternUtilsTest
    {

        [Test]
        public void ApplyAsAt_WhenPatternIncluded_ShouldReplaceWithTimestamp()
        {
            var filePath = "/some/dir/my_file_{AS_AT}";
            var filePathAutoGen = AutoGenPatternUtils.ApplyAsAt(filePath);
            Assert.IsTrue(DateTime.TryParseExact(filePathAutoGen[^17..], 
                "yyyyMMddHHmmssFFF", CultureInfo.InvariantCulture, DateTimeStyles.None, out _));
        }
        
        [Test]
        public void ApplyAsAtDate_WhenPatternIncluded_ShouldReplaceWithTimestamp()
        {
            var filePath = "/some/dir/my_file_{AS_AT_DATE}";
            var filePathAutoGen = AutoGenPatternUtils.ApplyAsAtDate(filePath);
            Assert.IsTrue(DateTime.TryParseExact(filePathAutoGen[^8..], 
                "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _));
        }
        
        [Test]
        public void ApplyRequestId_WhenPatternIncluded_ShouldReplaceWithRequestId()
        {
            const string filePath = "/some/dir/my_file_{REQUEST_ID}";
            const string requestId = "req_123";
            var filePathAutoGen = AutoGenPatternUtils.ApplyDataLicenseRequestId(filePath, requestId);
            Assert.AreEqual(filePathAutoGen, "/some/dir/my_file_req_123");
        }
        
        [Test]
        public void ApplyAll_WhenPatternIncluded_ShouldReplaceWithTimestampAndRequestId()
        {
            const string filePath = "{REQUEST_ID}/some/dir/my_file_{AS_AT_DATE}";
            const string requestId = "req_123";
            var filePathAutoGen = AutoGenPatternUtils.ApplyAllPatterns(filePath, requestId);
            Assert.IsTrue(DateTime.TryParseExact(filePathAutoGen[^8..], 
                "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _));
            Assert.IsTrue(filePathAutoGen.StartsWith("req_123"));
        }
        
        [Test]
        public void ApplyRequestId_WhenPatternMissingBraces_ShouldReplaceNothing()
        {
            const string filePath = "/some/dir/my_file_REQUEST_ID";
            const string requestId = "req_123";
            var filePathAutoGen = AutoGenPatternUtils.ApplyDataLicenseRequestId(filePath, requestId);
            Assert.AreEqual(filePathAutoGen, "/some/dir/my_file_REQUEST_ID");
        }
        
        [Test]
        public void ApplyTestPattern_WhenPatternIncluded_ShouldReplaceWithStaticText()
        {
            const string filePath = "/some/dir/my_file_{TEST}";
            var filePathAutoGen = AutoGenPatternUtils.ApplyTestPattern(filePath);
            Assert.AreEqual(filePathAutoGen, "/some/dir/my_file_TestAutoGenPattern");
        }
        
    }
}