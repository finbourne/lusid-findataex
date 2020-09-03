using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Model;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Tests.Core;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Vendor
{
    [TestFixture]
    public class FinDataExRuntimeToLusidDriveTests
    {
        private string _processedResponseFolder;
        private string _processedResponseFolderName;
        private ILusidApiFactory _factory;
        private IFoldersApi _foldersApi;
        private string _processedResponseFolderId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _processedResponseFolderName = ("FinDataEx_CI_TempDataFolder_" + Guid.NewGuid()).Substring(0,49);
            _processedResponseFolder = "/" + _processedResponseFolderName;
            _factory = LusidApiFactoryBuilder.Build("secrets.json");
            _foldersApi = _factory.Api<IFoldersApi>();
        }
        
        [SetUp]
        public void SetUp()
        {
            // setup temporary test folder in LUSID drive
            _processedResponseFolderId = _foldersApi.GetRootFolder(filter: $"Name eq '{_processedResponseFolderName}'").Values.SingleOrDefault()?.Id;
            var createFolder = new CreateFolder("/", _processedResponseFolderName);
            _processedResponseFolderId ??= _foldersApi.CreateFolder(createFolder).Id;
        }
        
        [TearDown]
        public void TearDown()
        {
            // remove temporary folders.
            // note if debugging and terminating test early ensure folder is delete on LUSID drive
            _foldersApi.DeleteFolder(_processedResponseFolderId);
        }
        
        [Test]
        public void run_OnRequestFromFileSystemWithOutputToLusidDrive_ShouldProcessAndWriteToLusidDrive()
        {
            //when
            var fdeValidRequestWithOutputToLusidDrive = Path.Combine(new[]{"Vendor","Dl","TestData","fde_request_dl_prices_lusid_drive.json"});
            
            //execute
            FinDataExRuntime.Main(new string[]{"FileSystem", fdeValidRequestWithOutputToLusidDrive, _processedResponseFolder});
            
            // verify
            AssertProcessedOutputExistsInLusidDrive();
        }
        
        [Test]
        public void run_OnRequestFromLusidDriveWithOutputToLusidDrive_ShouldProcessAndWriteToLusidDrive()
        {
            //when
            var fdeValidRequestFileLusidDriveId = FdeRequestBuilderTests.CiTestFdePricesRequestLusidDriveId;
            
            //execute
            FinDataExRuntime.Main(new string[]{"LusidDrive", fdeValidRequestFileLusidDriveId, _processedResponseFolder});
            
            // verify
            AssertProcessedOutputExistsInLusidDrive();
        }
        
        [Test]
        public void run_OnRequestWithSchedulerTypeArguments_ShouldProcessAndWriteToLusidDrive()
        {
            //when
            var fdeValidRequestFileLusidDriveId = FdeRequestBuilderTests.CiTestFdePricesRequestLusidDriveId;
            
            //execute
            FinDataExRuntime.Main(new string[]{"fdeRequestSource=LusidDrive", "requestPath=" + fdeValidRequestFileLusidDriveId, "outputDirectory=" + _processedResponseFolder});
            
            // verify
            AssertProcessedOutputExistsInLusidDrive();
        }
        
        [Test]
        public void run_WithBadRequestSource_ShouldThrowException()
        {
            //when
            var fdeValidRequestFileLusidDriveId = FdeRequestBuilderTests.CiTestFdePricesRequestLusidDriveId;
            
            //execute and verify
            Assert.Throws<ArgumentException>(() => 
                FinDataExRuntime.Main(new string[]{"fdeRequestSource=SomeNonExistingSource", "requestPath=" + fdeValidRequestFileLusidDriveId, "outputDirectory=" + _processedResponseFolder}));
            
            // even with exception verify no files were created
            AssertProcessedOutputDoesNotExistInLusidDrive();
        }

        private void AssertProcessedOutputExistsInLusidDrive()
        {

            PagedResourceListOfStorageObject listOfFilesInFolder = _foldersApi.GetFolderContents(_processedResponseFolderId);
            List<StorageObject> filesInFolder = listOfFilesInFolder.Values;
            
            // verify only one file created
            Assert.That(filesInFolder.Count, Is.EqualTo(1));
            
            // verify processed file is as expected
            StorageObject processedOutputFile = filesInFolder[0]; 
            Assert.That(processedOutputFile.Name, Is.EqualTo("DL12345_Prices.csv"));
            Assert.That(processedOutputFile.Type, Is.EqualTo("File"));
            Assert.That(processedOutputFile.Size, Is.EqualTo(424));
        }
        
        private void AssertProcessedOutputDoesNotExistInLusidDrive()
        {

            PagedResourceListOfStorageObject listOfFilesInFolder = _foldersApi.GetFolderContents(_processedResponseFolderId);
            List<StorageObject> filesInFolder = listOfFilesInFolder.Values;
            // verify no files created
            Assert.That(filesInFolder.Count, Is.EqualTo(0));
        }

    }
}