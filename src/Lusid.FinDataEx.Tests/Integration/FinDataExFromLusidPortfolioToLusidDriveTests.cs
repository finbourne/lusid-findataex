using System;
using System.Linq;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Model;
using Lusid.FinDataEx.Util.FileUtils.Handler;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Integration
{
    [TestFixture]
    [Ignore("Integration tests are currently unable to run because of licensing issues")]
    [Category("Unsafe")]
    public class FinDataExFromLusidPortfolioToLusidDriveTests : LusidPortfolioTestHelper
    {
        private string _lusidOutputDirPath;
        private string _lusidOutputDirName;
        private string _outputDirId;
        private string _outputFilePath;
        private IFoldersApi _foldersApi;
        private LusidDriveFileHandler _driveHandler;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _lusidOutputDirName = ("Test_Dir_FDE_" + Guid.NewGuid()).Substring(0, 49);
            _lusidOutputDirPath = "/" + _lusidOutputDirName;
            _outputFilePath = _lusidOutputDirPath + "/test_request_output.csv";
            _driveHandler = new LusidDriveFileHandler(TestUtils.DriveApiFactory);
            _foldersApi = TestUtils.DriveApiFactory.Api<IFoldersApi>();
        }

        [SetUp]
        public override void SetUp()
        {
            // base setup to create portfolios for test
            base.SetUp();

            _outputDirId = _foldersApi.GetRootFolder(filter: $"Name eq '{_lusidOutputDirName}'").Values.SingleOrDefault()?.Id;
            var createFolder = new CreateFolder("/", _lusidOutputDirName);
            _outputDirId ??= _foldersApi.CreateFolder(createFolder).Id;
        }

        [TearDown]
        public override void TearDown()
        {
            // base tear down to drop portfolios on test completion
            base.TearDown();

            _foldersApi.DeleteFolder(_outputDirId);
        }

        [Test]
        public void FinDataEx_GetData_OnValidPortfolio_ShouldProduceDataFile()
        {
            var amznHoldingPortfolio = $"{Scope}|{Portfolio}";
            var msftHoldingPortfolio = $"{Scope}|{Portfolio2}";
            var commandArgs = $"getdata -i LusidPortfolioInstrumentSource -a {amznHoldingPortfolio} {msftHoldingPortfolio} -f {_outputFilePath} -s Lusid -d ID_BB_GLOBAL PX_LAST --unsafe";
            FinDataEx.Main(commandArgs.Split(" "));

            var entries = _driveHandler.Read(_outputFilePath, ',');

            Assert.That(entries[0], Is.EqualTo("timeStarted|timeFinished|ID_BB_GLOBAL|PX_LAST"));

            var instrumentEntry1 = entries[1].Split("|");
            Assert.That(instrumentEntry1[2], Is.EqualTo("BBG000BVPV84"));
            // timestamps and price will change with each call so just check not empty
            Assert.That(instrumentEntry1[0], Is.Not.Empty);
            Assert.That(instrumentEntry1[1], Is.Not.Empty);
            Assert.That(instrumentEntry1[3], Is.Not.Empty);

            var instrumentEntry2 = entries[2].Split("|");
            Assert.That(instrumentEntry2[2], Is.EqualTo("BBG000BPHFS9"));
            // price will change with each call so just check not empty
            Assert.That(instrumentEntry2[0], Is.Not.Empty);
            Assert.That(instrumentEntry2[1], Is.Not.Empty);
            Assert.That(instrumentEntry2[3], Is.Not.Empty);
        }
    }
}