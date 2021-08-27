using System;
using System.Collections.Generic;
using System.Linq;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Model;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Output;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Integration.Output
{
    [TestFixture]
    [Category("Unsafe")]
    public class LusidDriveOutputWriterTests
    {
        private LusidDriveOutputWriter _outputWriter;

        private string _lusidOutputDirPath;
        private string _lusidOutputDirName;
        private ILusidApiFactory _factory;
        private IFoldersApi _foldersApi;
        private IFilesApi _filesApi;
        private string _outputDirId;
        private readonly string _outputFilename = "{REQUEST_ID}_dlws_output.csv";

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
            _outputDirId = _foldersApi.GetRootFolder(filter: $"Name eq '{_lusidOutputDirName}'").Values.SingleOrDefault()?.Id;
            var createFolder = new CreateFolder("/", _lusidOutputDirName);
            _outputDirId ??= _foldersApi.CreateFolder(createFolder).Id;
            _outputWriter = new LusidDriveOutputWriter(_lusidOutputDirPath + "/" + _outputFilename, _factory);
        }
        
        [TearDown]
        public void TearDown()
        {
            // remove folders in drive at end of each test.
            // note if debugging ensure to clean lusid drive if terminate tests early
            _foldersApi.DeleteFolder(_outputDirId);
        }
        
        [Test]
        public void Write_OnValidFinData_ShouldWriteToOutputDir()
        {
            //when
            var finDataOutput = CreateFinDataEntry("req_id_1");
            //execute
            var writeResult = _outputWriter.Write(finDataOutput);
            
            //verify
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            Assert.That(writeResult.FileOutputPath, Is.Not.Empty);
            AssertFileExistsInLusidDrive(writeResult.FileOutputPath);
        }
        
        [Test]
        public void Write_OnEmptyFinData_ShouldDoNothingButReturnOk()
        {
            //when
            var finDataOutputs = DataLicenseOutput.Empty();
            
            //execute
            var writeResult = _outputWriter.Write(finDataOutputs);
            
            //verify
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.NotRun));
            AssertLusidDriveFolderIsEmpty(_outputDirId);
        }

        private void AssertFileExistsInLusidDrive(string lusidDriveFileId)
        {
            try
            {
                StorageObject storageObject = _filesApi.GetFile(lusidDriveFileId);
                Assert.That(storageObject.Id, Is.EqualTo(lusidDriveFileId));
                Assert.That(storageObject.Name, Is.EqualTo("req_id_1_dlws_output.csv"));
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
            var contents = _foldersApi.GetFolderContents(lusdiDriveFolderId);
            CollectionAssert.IsEmpty(contents.Values);
        }
        
        private DataLicenseOutput CreateFinDataEntry(string id)
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
            return new DataLicenseOutput(id, headers, records);
        } 
        
    }
}