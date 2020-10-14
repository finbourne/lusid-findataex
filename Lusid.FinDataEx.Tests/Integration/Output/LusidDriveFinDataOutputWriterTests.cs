using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Model;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Output;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Integration.Output
{
    [TestFixture]
    public class LusidDriveFinDataOutputWriterTests
    {

        private LusidDriveFinDataOutputWriter _outputWriter;
        
        private string _lusidOutputDirPath;
        private string _lusidOutputDirName;
        private ILusidApiFactory _factory;
        private IFoldersApi _foldersApi;
        private IFilesApi _filesApi;
        private string _processedResponseFolderId;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _lusidOutputDirName = ("Test_Dir_FDE_" + Guid.NewGuid()).Substring(0,49);
            _lusidOutputDirPath = "/" + _lusidOutputDirName;
            _factory = LusidApiFactoryBuilder.Build("secrets.json");
            _foldersApi = _factory.Api<IFoldersApi>();
            _filesApi = _factory.Api<IFilesApi>();
        }
        
        [SetUp]
        public void SetUp()
        {
            // setup temp test folder in LUSID drive for each run
            _processedResponseFolderId = _foldersApi.GetRootFolder(filter: $"Name eq '{_lusidOutputDirName}'").Values.SingleOrDefault()?.Id;
            var createFolder = new CreateFolder("/", _lusidOutputDirName);
            _processedResponseFolderId ??= _foldersApi.CreateFolder(createFolder).Id;
            _outputWriter = new LusidDriveFinDataOutputWriter(_lusidOutputDirPath, _factory);
        }
        
        [TearDown]
        public void TearDown()
        {
            // remove folders in drive at end of each test.
            // note if debugging ensure to clean lusid drive if terminate tests early
            _foldersApi.DeleteFolder(_processedResponseFolderId);
        }
        
        [Test]
        public void Write_OnValidFinData_ShouldWriteToOutputDir()
        {
            //when
            var finDataOutputs = new List<FinDataOutput>
            {
                CreateFinDataEntry("id_1_GetData")
            };
            //execute
            var writeResult = _outputWriter.Write(finDataOutputs);
            
            //verify
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            Assert.That(writeResult.FilesWritten.Count, Is.EqualTo(1));
            AssertFileExistsInLusidDrive(writeResult.FilesWritten[0]);
        }
        
        [Test]
        public void Write_OnNoFinData_ShouldDoNothingButReturnOk()
        {
            //when
            var finDataOutputs = new List<FinDataOutput>();
            
            //execute
            var writeResult =  _outputWriter.Write(finDataOutputs);
            
            //verify
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            AssertLusidDriveFolderIsEmpty(_processedResponseFolderId);
        }

        private void AssertFileExistsInLusidDrive(string lusidDriveFileId)
        {
            try
            {
                StorageObject storageObject = _filesApi.GetFile(lusidDriveFileId);
                Assert.That(storageObject.Id, Is.EqualTo(lusidDriveFileId));
                Assert.That(storageObject.Name, Is.EqualTo("id_1_GetData.csv"));
                Assert.That(storageObject.Size, Is.EqualTo(92));
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected file id={lusidDriveFileId} could not be retrieved from Lusid Drive due " +
                            $"to exception={e.Message}");
            }
        }
        
        private void AssertLusidDriveFolderIsEmpty(string lusdiDriveFolderId)
        {
            PagedResourceListOfStorageObject contents = _foldersApi.GetFolderContents(lusdiDriveFolderId);
            CollectionAssert.IsEmpty(contents.Values);
        }
        
        private FinDataOutput CreateFinDataEntry(string id)
        {
            var headers = new List<string>{"h1","h2","h3"};
            var records = new List<Dictionary<string,string>>
            {
                new Dictionary<string, string>
                {
                    ["h1"] = "entry1Record1",
                    ["h2"] = "entry2Record1",
                    ["h3"] = "entry3Record1",
                },
                new Dictionary<string, string>
                {
                    ["h1"] = "entry1Record2",
                    ["h2"] = "entry2Record2",
                    ["h3"] = "entry3Record2",
                }
            };
            return new FinDataOutput(id, headers, records);
        } 
        
    }
}