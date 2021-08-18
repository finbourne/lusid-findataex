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
        private string _processedResponseFolderId;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _processedResponseFolderName = ("Test_Folder_FDExResponseProcessor_" + Guid.NewGuid()).Substring(0,49);
            _processedResponseFolder = "/" + _processedResponseFolderName;
            _factory = LusidApiFactoryBuilder.Build("secrets.json");
            _foldersApi = _factory.Api<IFoldersApi>();
        }
        
        [SetUp]
        public void SetUp()
        {
            // setup temp test folder in LUSID drive for each run
            _processedResponseFolderId = _foldersApi.GetRootFolder(filter: $"Name eq '{_processedResponseFolderName}'").Values.SingleOrDefault()?.Id;
            var createFolder = new CreateFolder("/", _processedResponseFolderName);
            _processedResponseFolderId ??= _foldersApi.CreateFolder(createFolder).Id;
            
            _responseProcessor = new LusidDriveVendorResponseProcessor(_processedResponseFolder, _factory);
        }
        
        [TearDown]
        public void TearDown()
        {
            // remove folders in drive at end of each test.
            // note if debugging ensure to clean lusid drive if terminate tests early
            _foldersApi.DeleteFolder(_processedResponseFolderId);
        }

        [Test]
        public void ProcessResponse_OnValidFinData_ShouldOutputFileToLusidDrive()
        {
            //when
            FdeRequest request = CreateFdeRequest();
            IVendorResponse vendorResponse = CreateMockVendorResponse();
            
            //execute
            ProcessResponseResult processResponseResult = _responseProcessor.ProcessResponse(request, vendorResponse);
            
            //verify status ok
            Assert.AreEqual(ProcessResponseResultStatus.Ok,processResponseResult.Status);
            
            //verify process response results as expected
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
        public void ProcessResponse_OnInvalidRequestDueToBadFileName_ShouldReturnFailedStatus()
        {
            //when
            FdeRequest request = CreateFdeRequestWithBadFileName();
            IVendorResponse vendorResponse = CreateMockVendorResponse();
            
            //execute
            ProcessResponseResult processResponseResult = _responseProcessor.ProcessResponse(request, vendorResponse);
            
            //verify status fail
            Assert.AreEqual(ProcessResponseResultStatus.Fail,processResponseResult.Status);
            
            //verify process response results as expected
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
        
        private FdeRequest CreateFdeRequestWithBadFileName()
        {
            // Illegal name with multiple "." which are not valid on LUSID drive.
            return new FdeRequest()
            {
                Uid = "some_bad$£_name$$><",
                Output = _processedResponseFolder
            };
        }
        
    }
}