using System;
using System.IO;
using System.Linq;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Client;
using Lusid.Drive.Sdk.Model;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Output;
using NUnit.Framework;
using Polly;

namespace Lusid.FinDataEx.Tests.Integration
{
    [TestFixture]
    [Explicit]
    public class FinDataExLusidDriveTests
    {
        private const string OutputFileName = "test_request_output.csv";
        private const string InputFileName = "real_instruments.csv";

        private string _lusidTestDirPath;
        private string _lusidTestDirName;

        private ILusidApiFactory _factory;
        private IFoldersApi _foldersApi;
        private IFilesApi _filesApi;

        private string _testDirId;
        private string _inputFilePath;
        private string _outputFilePath;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _lusidTestDirName = "Test_Dir_FDE_" + Guid.NewGuid().ToString().Substring(0,36);
            _lusidTestDirPath = "/" + _lusidTestDirName;

            _inputFilePath = _lusidTestDirPath + "/" + InputFileName;
            _outputFilePath = _lusidTestDirPath + "/" + OutputFileName;

            _factory = LusidApiFactoryBuilder.Build("secrets.json");
            _foldersApi = _factory.Api<IFoldersApi>();
            _filesApi = _factory.Api<IFilesApi>();
        }
        
        [SetUp]
        public void SetUp()
        {
            // Setup temp test folder in LUSID drive for each run
            _testDirId = _foldersApi.GetRootFolder(filter: $"Name eq '{_lusidTestDirName}'").Values.SingleOrDefault()?.Id;
            _testDirId ??= _foldersApi.CreateFolder(new CreateFolder("/", _lusidTestDirName)).Id;

            // Upload a test file to the folder
            var testDataFile = File.ReadAllBytes(Path.Combine("Integration", "DataLicense", "Instrument", "TestData", InputFileName));
            var fileId = _filesApi.CreateFile(InputFileName, _lusidTestDirPath, testDataFile.Length, testDataFile).Id;

            // Wait for the file to become available for download
            Policy.Handle<ApiException>()
                .WaitAndRetry(5, attempts => TimeSpan.FromSeconds(10))
                .Execute(() =>
                {
                    using var _ = _filesApi.DownloadFile(fileId);
                });
        }
        
        [TearDown]
        public void TearDown()
        {
            // remove folders in drive at end of each test.
            // note if debugging ensure to clean lusid drive if terminate tests early
            _foldersApi.DeleteFolder(_testDirId);
        }

        [Test]
        public void FinDataEx_GetData_OnValidBbgId_ShouldProduceDataFile()
        {
            var commandArgs = $"getdata -i InstrumentSource -a BBG000BPHFS9 BBG000BVPV84 -f {_outputFilePath} -s Lusid -d ID_BB_GLOBAL PX_LAST --unsafe";
            FinDataEx.Main(commandArgs.Split(" "));

            //verify
            var entries = GetFileAsStringsFromFolderInDrive(_testDirId);
            
            // check headers
            Assert.That(entries[0], Is.EqualTo("timeStarted|timeFinished|ID_BB_GLOBAL|PX_LAST"));

            // check instrument 1 entry
            var instrumentEntry1 = entries[1].Split("|");
            Assert.That(instrumentEntry1[2], Is.EqualTo("BBG000BPHFS9"));
            // timestamps and price will change with each call so just check not empty
            Assert.That(instrumentEntry1[0], Is.Not.Empty);
            Assert.That(instrumentEntry1[1], Is.Not.Empty);
            Assert.That(instrumentEntry1[3], Is.Not.Empty);
            
            // check instrument 2 entry
            var instrumentEntry2 = entries[2].Split("|");
            Assert.That(instrumentEntry2[2], Is.EqualTo("BBG000BVPV84"));
            // price will change with each call so just check not empty
            Assert.That(instrumentEntry2[0], Is.Not.Empty);
            Assert.That(instrumentEntry2[1], Is.Not.Empty);
            Assert.That(instrumentEntry2[3], Is.Not.Empty);
        }

        [Test]
        public void FinDataEx_GetData_OnValidBbgIdFromCsvInstrumentSource_ShouldProduceDataFile()
        {
            //const string instrumentSourceDriveCsvPath = "/findataex-tests/testdata/real_instruments.csv";
            var commandArgs = $"getdata -i DriveCsvInstrumentSource -a {_inputFilePath} -f {_outputFilePath} -s Lusid -d ID_BB_GLOBAL PX_LAST --unsafe";

            FinDataEx.Main(commandArgs.Split(" "));

            //verify
            var entries = GetFileAsStringsFromFolderInDrive(_testDirId);
            
            // check headers
            Assert.That(entries[0], Is.EqualTo("timeStarted|timeFinished|ID_BB_GLOBAL|PX_LAST"));

            // check instrument 1 entry
            var instrumentEntry1 = entries[1].Split("|");
            Assert.That(instrumentEntry1[2], Is.EqualTo("BBG000BPHFS9"));
            // timestamps and price will change with each call so just check not empty
            Assert.That(instrumentEntry1[0], Is.Not.Empty);
            Assert.That(instrumentEntry1[1], Is.Not.Empty);
            Assert.That(instrumentEntry1[3], Is.Not.Empty);
            
            // check instrument 2 entry
            var instrumentEntry2 = entries[2].Split("|");
            Assert.That(instrumentEntry2[2], Is.EqualTo("BBG000BVPV84"));
            // price will change with each call so just check not empty
            Assert.That(instrumentEntry2[0], Is.Not.Empty);
            Assert.That(instrumentEntry2[1], Is.Not.Empty);
            Assert.That(instrumentEntry2[3], Is.Not.Empty);
        }
        
        

        private string[] GetFileAsStringsFromFolderInDrive(string lusdiDriveFolderId)
        {
            var contents = _foldersApi.GetFolderContents(lusdiDriveFolderId);
            // ensure only one file in folder otherwise test folder is contaminated and test corrupt
            Assert.That(contents.Values.Count, Is.EqualTo(1));
            
            var expectedPricesStorageObject = contents.Values[0];
            StringAssert.EndsWith("test_request_output.csv", expectedPricesStorageObject.Name);
            
            return new StreamReader(_filesApi.DownloadFile(expectedPricesStorageObject.Id))
                .ReadToEnd()
                .Split(LusidDriveOutputWriter.OutputFileEntrySeparator);
        }
    }
}