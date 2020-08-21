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
        private readonly string _requestUrl = "/some/loc/DL12.req";
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
            FdeRequest wellFormedDlRequest = CreateWellFormedDlFdeRequest();
            
            DlFtpRequest vendorRequest =  _dlFtpExtractor.ToVendorRequest(wellFormedDlRequest);
            
            Assert.AreEqual(_url, vendorRequest.FtpUrl);
            Assert.AreEqual(_user, vendorRequest.User);
            Assert.AreEqual(_pass, vendorRequest.Password);
            Assert.AreEqual(_requestUrl, vendorRequest.RequestFileUrl);
        }
        
        [Test]
        public void ToVendorRequest_OnFdeRequestMissingConnectorConfig_ShouldThrowException()
        {
            FdeRequest incompleteConInfoDlFdeRequest = CreateIncompleteConnectorInfoDlFdeRequest();

            try
            {
                _dlFtpExtractor.ToVendorRequest(incompleteConInfoDlFdeRequest);
                Assert.Fail("Expected a bad data exception due to missing connection parameters.");
            }
            catch (InvalidDataException) {}
        }
        
        [Test]
        public void ToVendorRequest_OnFdeRequestMissingRequestBody_ShouldReturnDlVendorRequest()
        {
            FdeRequest incompleteRequestBodyDlFdeRequests = CreateIncompleteRequestBodyDlFdeRequest();

            try
            {
                _dlFtpExtractor.ToVendorRequest(incompleteRequestBodyDlFdeRequests);
                Assert.Fail("Expected a bad data exception due to missing request body parameters.");
            }
            catch (InvalidDataException) {}
        }
        
        [Test]
        public void ToVendorRequest_OnBadlyFormedDlRequest_ShouldThrowException()
        {
            // FdeRequests general structure should have been tested before reaching the extractor method.
            // Therefore not catching any specific type of exception as this isn't handled by DL extractor.
            FdeRequest badlyFormedFdeRequest = CreateBadlyFormedFdeRequest();

            try
            {
                _dlFtpExtractor.ToVendorRequest(badlyFormedFdeRequest);
                Assert.Fail("Expected an exception due to badly formed request.");
            }
            catch (Exception e) {}
        }
        
        private FdeRequest CreateWellFormedDlFdeRequest()
        {
            FdeRequest request = new FdeRequest();
            request.Uid = _uid;
            request.CallerId = _callerId;
            request.ConnectorConfig = new Dictionary<string, object>()
                {
                    {"type" , _type},
                    {"url" , _url},
                    {"user" , _user},
                    {"password" , _pass}
                };
            request.RequestBody = new Dictionary<string, object>()
            {
                {"source" , "file"},
                {"sourceData" , _requestUrl},
                {"requestType", "Prices"}
            };
            request.Vendor = Vendors.DL;
            return request;
        }
        
        private FdeRequest CreateIncompleteConnectorInfoDlFdeRequest()
        {
            FdeRequest request = new FdeRequest();
            request.Uid = _uid;
            request.CallerId = _callerId;
            request.ConnectorConfig = new Dictionary<string, object>()
            {
                // Missing User information
                {"type" , _type},
                {"url" , _url},
                {"password" , _pass}
            };
            request.RequestBody = new Dictionary<string, object>()
            {
                {"source" , "file"},
                {"sourceData" , _requestUrl},
            };
            request.Vendor = Vendors.DL;
            return request;
        }
        
        private FdeRequest CreateIncompleteRequestBodyDlFdeRequest()
        {
            FdeRequest request = new FdeRequest();
            request.Uid = _uid;
            request.CallerId = _callerId;
            request.ConnectorConfig = new Dictionary<string, object>()
            {
                {"type" , _type},
                {"url" , _url},
                {"user" , _user},
                {"password" , _pass}
            };
            request.RequestBody = new Dictionary<string, object>()
            {
                {"source" , "file"},
                //missing source data file for dl request
            };
            request.Vendor = Vendors.DL;
            return request;
        }
        
        private FdeRequest CreateBadlyFormedFdeRequest()
        {
            FdeRequest request = new FdeRequest();
            request.Uid = _uid;
            request.CallerId = _callerId;
            //missing connection and request config
            request.Vendor = Vendors.DL;
            return request;
        }
        
    }
}