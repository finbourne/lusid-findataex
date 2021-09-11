using Lusid.FinDataEx.Util;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Unit.Util
{
    [TestFixture]
    public class AutoGenPatternUtilsTests
    {
        [Test]
        public void CheckApplyTest()
        {
            Assert.That(AutoGenPatternUtils.ApplyTestPattern("{TEST}"), Is.EqualTo("TestAutoGenPattern"));
        }

        [Test]
        public void ReplacesWithoutDistrubingOtherText()
        {
            Assert.That(AutoGenPatternUtils.ApplyTestPattern("abc{TEST}123"), Is.EqualTo("abcTestAutoGenPattern123"));
        }

        [Test]
        public void WithoutBracesNothingIsReplaced()
        {
            Assert.That(AutoGenPatternUtils.ApplyTestPattern("TEST"), Is.EqualTo("TEST"));
        }

        [Test]
        public void CheckAsAt()
        {
            Assert.That(AutoGenPatternUtils.ApplyAsAt("{AS_AT}"), Is.Not.EqualTo("{AS_AT}"));
        }

        [Test]
        public void CheckAsAtDate()
        {
            Assert.That(AutoGenPatternUtils.ApplyAsAtDate("{AS_AT_DATE}"), Is.Not.EqualTo("{AS_AT_DATE}"));
        }

        [Test]
        public void CheckApplyDateTime()
        {
            var output = AutoGenPatternUtils.ApplyDateTimePatterns("{AS_AT}_{AS_AT_DATE}_{TEST}").Split('_');

            Assert.That(AutoGenPatternUtils.ApplyAsAt("{AS_AT}"), Is.Not.EqualTo("{AS_AT}"));
            Assert.That(AutoGenPatternUtils.ApplyAsAtDate("{AS_AT_DATE}"), Is.Not.EqualTo("{AS_AT_DATE}"));
            Assert.That(output[3], Is.EqualTo("TestAutoGenPattern"));
        }

        [Test]
        public void CheckRequestId()
        {
            Assert.That(AutoGenPatternUtils.ApplyDataLicenseRequestId("{REQUEST_ID}", "id"), Is.EqualTo("id"));
        }

        [Test]
        public void CheckApplyAll()
        {
            var output = AutoGenPatternUtils.ApplyAllPatterns("{AS_AT}_{AS_AT_DATE}_{TEST}_{REQUEST_ID}", "id").Split('_');

            Assert.That(AutoGenPatternUtils.ApplyAsAt("{AS_AT}"), Is.Not.EqualTo("{AS_AT}"));
            Assert.That(AutoGenPatternUtils.ApplyAsAtDate("{AS_AT_DATE}"), Is.Not.EqualTo("{AS_AT_DATE}"));
            Assert.That(output[3], Is.EqualTo("TestAutoGenPattern"));
            Assert.That(output[4], Is.EqualTo("id"));
        }
    }
}