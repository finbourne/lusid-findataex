using System.Collections.Generic;
using System.IO;
using Lusid.FinDataEx.Core;
using Lusid.FinDataEx.Vendor;
using Lusid.FinDataEx.Vendor.Dl.Ftp;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Vendor
{
    [TestFixture]
    public class VendorExtractorBuilderTest
    {
        private VendorExtractorBuilder _vendorExtractorBuilder;

        [SetUp]
        public void SetUp()
        {
            _vendorExtractorBuilder = new VendorExtractorBuilder();
        }
        
        [Test]
        public void CreateFdeExtractor_OnValidDlFtpFdeRequest_ShouldReturnDlFtpExtractor()
        {
            var dlFtpRequest = CreateDlFtpFdeRequest();
            var fdeExtractor = _vendorExtractorBuilder.CreateFdeExtractor(dlFtpRequest);
            Assert.IsInstanceOf(typeof(DlFtpExtractor), fdeExtractor);
        }
        
        [Test]
        public void CreateFdeExtractor_OnNonFtpConnectorConfig_ShouldThrowException()
        {
            var dlFtpRequest = new FdeRequest
            {
                Vendor = Vendors.DL,
                ConnectorConfig = new Dictionary<string, object>(){["type"] = "webservice"}
            };
            
            Assert.Throws<InvalidDataException>(() => _vendorExtractorBuilder.CreateFdeExtractor(dlFtpRequest),
                "Exception should have been thrown due to no connector config");
        }
        
        [Test]
        public void CreateFdeExtractor_OnUnsupportedVendor_ShouldThrowException()
        {
            var dlFtpRequest = new FdeRequest {Vendor = "SomeUnsupportedVendor"};
            
            Assert.Throws<InvalidDataException>(() => _vendorExtractorBuilder.CreateFdeExtractor(dlFtpRequest),
                "Exception should have been thrown due to unsupported vendor");
        }
        
        private FdeRequest CreateDlFtpFdeRequest()
        {
            FdeRequest request = new FdeRequest();
            request.ConnectorConfig = new Dictionary<string, object>()
            {
                ["type"] = "ftp", 
                ["url"] = "http://url", 
                ["user"] = "user_1",
                ["password"] = "pass"
            };
            request.Vendor = Vendors.DL;
            return request;
        }
        
    }
}