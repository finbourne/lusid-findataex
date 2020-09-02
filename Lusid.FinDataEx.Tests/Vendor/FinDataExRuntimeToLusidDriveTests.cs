using System;
using System.IO;
using System.Linq;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Model;
using Lusid.Drive.Sdk.Utilities;
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
        private IFilesApi _filesApi;
        private string _processedResponseFolderId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _processedResponseFolderName = ("FinDataEx_CI_TempDataFolder_" + Guid.NewGuid()).Substring(0,49);
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
        }
        
        [TearDown]
        public void TearDown()
        {
            _foldersApi.DeleteFolder(_processedResponseFolderId);
        }
        
        [Test]
        public void run_OnValidPricesRequestWithLusidDriveOutput_ShouldProcessAndWriteToFile()
        {
            //when
            var fdeValidRequestWithOutputToLusidDrive = Path.Combine(new[]{"Vendor","Dl","TestData","fde_request_dl_prices_lusid_drive.json"});
            
            //execute
            FinDataExRuntime.Main(new string[]{fdeValidRequestWithOutputToLusidDrive, _processedResponseFolder});
            
            // verify - get files from LUSID Drive ensure it exists
            Assert.Fail("Unfinished. Implement assertions that fetch from drive and confirm existence of file");
        }

    }
}