//using System;
//using System.IO;
//using System.Linq;
//using Lusid.Drive.Sdk.Api;
//using Lusid.Drive.Sdk.Model;
//using Lusid.Drive.Sdk.Utilities;
//using Lusid.FinDataEx.Output;
//using NUnit.Framework;

//namespace Lusid.FinDataEx.Tests.Integration
//{
//    [TestFixture]
//    [Category("Unsafe")]
//    public class FinDataExFromLusidPortfolioToLusidDriveTests : BaseLusidPortfolioTests
//    {
//        private string _lusidOutputDirPath;
//        private string _lusidOutputDirName;
//        private ILusidApiFactory _factory;
//        private IFoldersApi _foldersApi;
//        private IFilesApi _filesApi;
//        private string outputDirId;
//        private string _outputFilePath;

//        [OneTimeSetUp]
//        public void OneTimeSetUp()
//        {
//            _lusidOutputDirName = ("Test_Dir_FDE_" + Guid.NewGuid()).Substring(0,49);
//            _lusidOutputDirPath = "/" + _lusidOutputDirName;
//            _outputFilePath = _lusidOutputDirPath + "/test_request_output.csv";
//            _factory = Unit.TestUtils.DriveApiFactory;
//            _foldersApi = _factory.Api<IFoldersApi>();
//            _filesApi = _factory.Api<IFilesApi>();
//        }
        
//        [SetUp]
//        public override void SetUp()
//        {
//            // base setup to create portfolios for test
//            base.SetUp();
            
//            // setup temp test folder in LUSID drive for each run
//            outputDirId = _foldersApi.GetRootFolder(filter: $"Name eq '{_lusidOutputDirName}'").Values.SingleOrDefault()?.Id;
//            var createFolder = new CreateFolder("/", _lusidOutputDirName);
//            outputDirId ??= _foldersApi.CreateFolder(createFolder).Id;
//        }
        
//        [TearDown]
//        public override void TearDown()
//        {
//            // base tear down to drop portfolios on test completion
//            base.TearDown();
            
//            // remove folders in drive at end of each test.
//            // note if debugging ensure to clean lusid drive if terminate tests early
//            _foldersApi.DeleteFolder(outputDirId);
//        }

//        [Test]
//        public void FinDataEx_GetData_OnValidPortfolio_ShouldProduceDataFile()
//        {
//            // amzn holding portfolio (BBG000BVPV84)
//            var amznHoldingPortfolio = $"{Scope}|{Portfolio}";
//            // msft holding portfolio (BBG000BPHFS9)
//            var msftHoldingPortfolio = $"{Scope}|{Portfolio2}";
//            var commandArgs = $"getdata -i LusidPortfolioInstrumentSource -a {amznHoldingPortfolio} {msftHoldingPortfolio} -f {_outputFilePath} -s Lusid -d ID_BB_GLOBAL PX_LAST --unsafe";
//            FinDataEx.Main(commandArgs.Split(" "));
            
//            //verify
//            var entries = GetFileAsStringsFromFolderInDrive(outputDirId);
            
//            // check headers
//            Assert.That(entries[0], Is.EqualTo("timeStarted|timeFinished|ID_BB_GLOBAL|PX_LAST"));

//            // check instrument 1 entry
//            var instrumentEntry1 = entries[1].Split("|");
//            Assert.That(instrumentEntry1[2], Is.EqualTo("BBG000BVPV84"));
//            // timestamps and price will change with each call so just check not empty
//            Assert.That(instrumentEntry1[0], Is.Not.Empty);
//            Assert.That(instrumentEntry1[1], Is.Not.Empty);
//            Assert.That(instrumentEntry1[3], Is.Not.Empty);
            
//            // check instrument 2 entry
//            var instrumentEntry2 = entries[2].Split("|");
//            Assert.That(instrumentEntry2[2], Is.EqualTo("BBG000BPHFS9"));
//            // price will change with each call so just check not empty
//            Assert.That(instrumentEntry2[0], Is.Not.Empty);
//            Assert.That(instrumentEntry2[1], Is.Not.Empty);
//            Assert.That(instrumentEntry2[3], Is.Not.Empty);
//        }

//        private string[] GetFileAsStringsFromFolderInDrive(string lusdiDriveFolderId)
//        {
//            var contents = _foldersApi.GetFolderContents(lusdiDriveFolderId);
//            // ensure only one file in folder otherwise test folder is contaminated and test corrupt
//            Assert.That(contents.Values.Count, Is.EqualTo(1));
            
//            var expectedPricesStorageObject = contents.Values[0];
//            StringAssert.EndsWith("test_request_output.csv", expectedPricesStorageObject.Name);
            
//            return new StreamReader(_filesApi.DownloadFile(expectedPricesStorageObject.Id))
//                .ReadToEnd()
//                .Split(LusidDriveOutputWriter.OutputFileEntrySeparator);
//        }
        
        
//    }
//}