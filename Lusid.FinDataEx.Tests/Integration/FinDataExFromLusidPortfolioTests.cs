using System.IO;
using NUnit.Framework;
using static Lusid.FinDataEx.Tests.Unit.TestUtils
    ;
namespace Lusid.FinDataEx.Tests.Integration
{
    public class FinDataExFromLusidPortfolioTests : BaseLusidPortfolioTests
    {
        private readonly string _tempOutputDir = $"TempTestDir_{nameof(FinDataExTests)}";
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            SetupTempTestDirectory(_tempOutputDir);
        }
        
        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            TearDownTempTestDirectory(_tempOutputDir);
        }

        [Test]
        public void FinDataEx_GetData_OnValidPortfolios_ShouldProduceDataFile()
        {
            var scopePortfolio1 = $"{Scope}|{Portfolio}";
            var scopePortfolio2 = $"{Scope}|{Portfolio2}";
            var scopePortfolioSameHoldingP1 = $"{Scope}|{PortfolioSameHoldingAsP1}";
            var commandArgs = $"getdata -p {scopePortfolio1} {scopePortfolio2} {scopePortfolioSameHoldingP1} -o {_tempOutputDir} -d ID_BB_GLOBAL PX_LAST";
            FinDataEx.Main(commandArgs.Split(" "));
            
            //verify
            var expectedDataFiles = Directory.GetFiles(_tempOutputDir);
            
            // ensure only GetData file created and name is in correct format
            // more than one file is a contaminated test and should be investigated
            Assert.That(expectedDataFiles.Length, Is.EqualTo(1));
            StringAssert.EndsWith("_GetData.csv", expectedDataFiles[0]);

            // ensure file is properly populated
            var entries = File.ReadAllLines(expectedDataFiles[0]);
            
            // check headers
            Assert.That(entries[0], Is.EqualTo("ID_BB_GLOBAL|PX_LAST"));

            // check instrument 1 entry
            var instrumentEntry1 = entries[1].Split("|");
            Assert.That(instrumentEntry1[0], Is.EqualTo("BBG000BVPV84"));
            // price will change with each call so just check not empty
            Assert.That(instrumentEntry1[0], Is.Not.Empty);
            
            // check instrument 2 entry
            var instrumentEntry2 = entries[2].Split("|");
            Assert.That(instrumentEntry2[0], Is.EqualTo("BBG000BPHFS9"));
            // price will change with each call so just check not empty
            Assert.That(instrumentEntry2[0], Is.Not.Empty);
        }
        
        [Test]
        public void FinDataEx_GetData_OnValidPortfoliosUsingIsin_ShouldProduceDataFile()
        {
            var scopePortfolio1 = $"{Scope}|{Portfolio}";
            var commandArgs = $"getdata -p {scopePortfolio1} -t ISIN -o {_tempOutputDir} -d ID_BB_GLOBAL PX_LAST";
            FinDataEx.Main(commandArgs.Split(" "));
            
            //verify
            var expectedDataFiles = Directory.GetFiles(_tempOutputDir);
            
            // ensure only GetData file created and name is in correct format
            // more than one file is a contaminated test and should be investigated
            Assert.That(expectedDataFiles.Length, Is.EqualTo(1));
            StringAssert.EndsWith("_GetData.csv", expectedDataFiles[0]);

            // ensure file is properly populated
            var entries = File.ReadAllLines(expectedDataFiles[0]);
            
            // check headers - querying BBG with ISIN but returning BBG Global Id (Figi)
            Assert.That(entries[0], Is.EqualTo("ID_BB_GLOBAL|PX_LAST"));

            // check instrument 1 entry
            var instrumentEntry1 = entries[1].Split("|");
            Assert.That(instrumentEntry1[0], Is.EqualTo("BBG000BVPV84"));
            // price will change with each call so just check not empty
            Assert.That(instrumentEntry1[0], Is.Not.Empty);
        }
    }
}