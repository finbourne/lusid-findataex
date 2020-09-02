using System;
using System.Collections.Generic;
using System.Linq;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Model;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Core;
using Lusid.FinDataEx.Vendor;
using Moq;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Core
{
    [TestFixture]
    public class LusidDriveVendorResponseProcessorTests
    {
        private LusidDriveVendorResponseProcessor _responseProcessor;
        
        private string _processedResponseFolder;
        private string _processedResponseFolderName;
        private ILusidApiFactory _factory;
        private IFoldersApi _foldersApi;
        private IFilesApi _filesApi;
        private string _processedResponseFolderId;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _processedResponseFolderName = ("Test_Folder_FDExResponseProcessor_" + Guid.NewGuid()).Substring(0,49);
            _processedResponseFolder = "/" + _processedResponseFolderName;
            _factory = LusidApiFactoryBuilder.Build("secrets.json");
            _filesApi = _factory.Api<IFilesApi>();
            _foldersApi = _factory.Api<IFoldersApi>();
        }
        
        [SetUp]
        public void SetUp()
        {
            _processedResponseFolderId = _foldersApi.GetRootFolder(filter: $"Name eq '{_processedResponseFolderName}'").Values.SingleOrDefault()?.Id;
            var createFolder = new CreateFolder("/", _processedResponseFolderName);
            _processedResponseFolderId ??= _foldersApi.CreateFolder(createFolder).Id;
            
            _responseProcessor = new LusidDriveVendorResponseProcessor(_processedResponseFolder, _factory);
        }
        
        [TearDown]
        public void TearDown()
        {
            _foldersApi.DeleteFolder(_processedResponseFolderId);
        }

        [Test]
        public void ProcessResponse_OnValidFinData_ShouldOutputFileToLusidDrive()
        {
            FdeRequest request = CreateFdeRequest();
            IVendorResponse vendorResponse = CreateMockVendorResponse();
            ProcessResponseResult processResponseResult = _responseProcessor.ProcessResponse(request, vendorResponse);
            
            Assert.AreEqual(ProcessResponseResultStatus.Ok,processResponseResult.Status);
            LusidDriveUploadResults quoteDataLusidDriveUploadResults = 
                processResponseResult.Properties["MyVendorQuoteData"] as LusidDriveUploadResults;
            
            Assert.AreEqual(quoteDataLusidDriveUploadResults.FdeRequestId, "fde_req_001");
            Assert.AreEqual(quoteDataLusidDriveUploadResults.FinDataKey, "MyVendorQuoteData");
            Assert.AreEqual(quoteDataLusidDriveUploadResults.LuisdDriveUploadStatus, ProcessResponseResultStatus.Ok);
            Assert.AreEqual(quoteDataLusidDriveUploadResults.LusidDriveFolder, _processedResponseFolder);
            Assert.AreEqual(quoteDataLusidDriveUploadResults.LusidDriveFileName, "fde_req_001_MyVendorQuoteData.csv");
            Assert.AreEqual(quoteDataLusidDriveUploadResults.LusidDriveFileSize, 56);
        }
        
        [Test]
        public void ProcessResponse_OnValidInvalidRequestDueToRequestIdSize_ShouldReturnFailedStatus()
        {
            FdeRequest request = CreateFdeRequestWithIllegalReqId();
            IVendorResponse vendorResponse = CreateMockVendorResponse();
            ProcessResponseResult processResponseResult = _responseProcessor.ProcessResponse(request, vendorResponse);
            
            Assert.AreEqual(ProcessResponseResultStatus.Fail,processResponseResult.Status);
            LusidDriveUploadResults quoteDataLusidDriveUploadResults = 
                processResponseResult.Properties["MyVendorQuoteData"] as LusidDriveUploadResults;
            
            Assert.AreEqual(quoteDataLusidDriveUploadResults.FdeRequestId, request.Uid);
            Assert.AreEqual(quoteDataLusidDriveUploadResults.FinDataKey, "MyVendorQuoteData");
            Assert.AreEqual(quoteDataLusidDriveUploadResults.LuisdDriveUploadStatus, ProcessResponseResultStatus.Fail);
            Assert.IsNull(quoteDataLusidDriveUploadResults.LusidDriveFileId);
            Assert.IsNull(quoteDataLusidDriveUploadResults.LusidDriveFolder);
            Assert.IsNull(quoteDataLusidDriveUploadResults.LusidDriveFileName);
            Assert.IsNull(quoteDataLusidDriveUploadResults.LusidDriveFileSize);
        }

        private FdeRequest CreateFdeRequest()
        {
            return new FdeRequest()
            {
                Uid = "fde_req_001",
                Output = _processedResponseFolder
            };
        }

        private IVendorResponse CreateMockVendorResponse()
        {
            IVendorResponse vendorResponse = Mock.Of<IVendorResponse>();
            Mock.Get(vendorResponse).Setup(m => m.GetFinData()).Returns(CreateTestFinData());
            return vendorResponse;
        }

        private Dictionary<string, List<List<string>>> CreateTestFinData()
        {
            return new Dictionary<string, List<List<string>>>()
            {
                ["MyVendorQuoteData"] = new List<List<string>>()
                {
                    new List<string>() {"Ticker", "Quote"},
                    new List<string>() {"Equity A", "125.60"},
                    new List<string>() {"Equity B", "43.05"},
                    new List<string>() {"EUR/USD", "1.20"},
                }
            };
            
        }
        
        private FdeRequest CreateFdeRequestWithIllegalReqId()
        {
            // Illegal Id in this case for LUSID drive is a request ID that is too long (>50chars)
            return new FdeRequest()
            {
                Uid = "some_long_long_long_long_long_long_long_long_long_long_long_long_long_long_long_long_uid_" + Guid.NewGuid(),
                Output = _processedResponseFolder
            };
        }
        
    }
}