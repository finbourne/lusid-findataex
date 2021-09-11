using Lusid.FinDataEx.DataLicense.Util;
using NUnit.Framework;
using PerSecurity_Dotnet;
using Polly.Fallback;
using Polly.Retry;
using System;

namespace Lusid.FinDataEx.Tests.Unit.DataLicense.Util
{
    [TestFixture]
    public class DataLicenseUtilsTests
    {
        [Test]
        public void RetryPoliciesAreApplied()
        {
            var wrappedResponse = DataLicenseUtils.GetBBGRetryPolicy<RetrieveGetActionsResponse>(TimeSpan.FromSeconds(1));

            Assert.That(wrappedResponse.Inner, Is.TypeOf<RetryPolicy<RetrieveGetActionsResponse>>());
            Assert.That(wrappedResponse.Outer, Is.TypeOf<FallbackPolicy<RetrieveGetActionsResponse>>());
        }
    }
}