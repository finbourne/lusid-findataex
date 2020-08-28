using System;
using System.Collections.Generic;
using System.IO;
using Lusid.FinDataEx.Core;
using Lusid.FinDataEx.Vendor;
using Lusid.FinDataEx.Vendor.Dl.Ftp;
using Moq;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Vendor.Dl.Ftp
{
    [TestFixture]
    public class DlFtpExtractorTest
    {
        private DlFtpExtractor _dlFtpExtractor;
        private IVendorClient<DlFtpRequest, DlFtpResponse> _vendorClient;

        private readonly string _uid = "dl_req_01";
        private readonly string _callerId = "daily_scheduler_01";
        private readonly string _url = "sftp://dlserver.com:22";
        private readonly string _type = "ftp";
        private readonly string _requestPath = "/some/loc/DL12.req";
        private readonly string _user = "user_1";
        private readonly string _pass = "passwd";


        [SetUp]
        public void SetUp()
        {
            _vendorClient = Mock.Of<IVendorClient<DlFtpRequest, DlFtpResponse>>();
            _dlFtpExtractor = new DlFtpExtractor(_vendorClient);
        }
        
        [Test]
        public void ToVendorRequest_OnWellFormedFdeRequest_ShouldReturnDlVendorRequest()
        {
            var wellFormedDlRequest = CreateWellFormedDlFdeRequest();
            
            var vendorRequest =  _dlFtpExtractor.ToVendorRequest(wellFormedDlRequest);
            
            Assert.That(_url, Is.EqualTo(vendorRequest.FtpUrl));
            Assert.That(_user, Is.EqualTo(vendorRequest.User));
            Assert.That(_pass, Is.EqualTo(vendorRequest.Password));
            Assert.That(_requestPath, Is.EqualTo(vendorRequest.RequestFilePath));
        }
        
        [Test]
        public void ToVendorRequest_OnFdeRequestMissingConnectorConfig_ShouldThrowException()
        {
            var incompleteConInfoDlFdeRequest = CreateIncompleteConnectorInfoDlFdeRequest();

            Assert.Throws<InvalidDataException>(() => _dlFtpExtractor.ToVendorRequest(incompleteConInfoDlFdeRequest),
                "Expected a bad data exception due to missing connection parameters.");
        }
        
        [Test]
        public void ToVendorRequest_OnFdeRequestMissingRequestBody_ShouldReturnDlVendorRequest()
        {
            var incompleteRequestBodyDlFdeRequests = CreateIncompleteRequestBodyDlFdeRequest();

            Assert.Throws<InvalidDataException>(() => _dlFtpExtractor.ToVendorRequest(incompleteRequestBodyDlFdeRequests),
                "Expected a bad data exception due to missing request body parameters.");
        }
        
        [Test]
        public void ToVendorRequest_OnBadlyFormedDlRequest_ShouldThrowException()
        {
            // FdeRequests general structure should have been tested before reaching the extractor method so expecting
            // a null reference
            var badlyFormedFdeRequest = CreateBadlyFormedFdeRequest();

            Assert.Throws<NullReferenceException>(() => _dlFtpExtractor.ToVendorRequest(badlyFormedFdeRequest),
                "Expected an exception due to badly formed request.");
        }
        
        private FdeRequest CreateWellFormedDlFdeRequest()
        {
            var request = new FdeRequest
            {
                Uid = _uid,
                CallerId = _callerId,
                ConnectorConfig =
                    new Dictionary<string, object>
                    {
                        ["type"] = _type, ["url"] = _url, ["user"] = _user, ["password"] = _pass
                    },
                RequestBody = new Dictionary<string, object>()
                {
                    ["source"] = "file", ["sourceData"] = _requestPath, ["requestType"] = "Prices",
                },
                Vendor = Vendors.DL
            };
            return request;
        }
        
        private FdeRequest CreateIncompleteConnectorInfoDlFdeRequest()
        {
            var request = new FdeRequest
            {
                Uid = _uid,
                CallerId = _callerId,
                ConnectorConfig = new Dictionary<string, object>()
                {
                    // Missing User information
                    ["type"] = _type, ["url"] = _url, ["password"] = _pass
                },
                RequestBody = new Dictionary<string, object>() {["source"] = "file", ["sourceData"] = _requestPath},
                Vendor = Vendors.DL
            };
            return request;
        }
        
        private FdeRequest CreateIncompleteRequestBodyDlFdeRequest()
        {
            var request = new FdeRequest
            {
                Uid = _uid,
                CallerId = _callerId,
                ConnectorConfig = new Dictionary<string, object>()
                {
                    ["type"] = _type, ["url"] = _url, ["user"] = _user, ["password"] = _pass
                },
                RequestBody = new Dictionary<string, object>()
                {
                    ["source"] = "file",
                    //missing source data file for dl request
                },
                Vendor = Vendors.DL
            };
            return request;
        }
        
        private FdeRequest CreateBadlyFormedFdeRequest()
        {
            var request = new FdeRequest {Uid = _uid, CallerId = _callerId, Vendor = Vendors.DL};
            //missing connection and request config
            return request;
        }
        
    }
}