using System.IO;
using NUnit.Framework;
using static Lusid.FinDataEx.Tests.Unit.TestUtils
    ;
namespace Lusid.FinDataEx.Tests.Integration
{
    public class FinDataExTests
    {
        private readonly string _tempOutputDir = $"TempTestDir_{nameof(FinDataExTests)}";
        
        [SetUp]
        public void SetUp()
        {
            SetupTempTestDirectory(_tempOutputDir);
        }
        
        [TearDown]
        public void TearDown()
        {
            TearDownTempTestDirectory(_tempOutputDir);
        }

        [Test]
        public void FinDataEx_GetData_OnValidBbgId_ShouldProduceDataFile()
        {
            var commandArgs = $"GetData -i BBG000BPHFS9 BBG000BVPV84 -o {_tempOutputDir} -d ID_BB_GLOBAL PX_LAST";
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
            Assert.That(instrumentEntry1[0], Is.EqualTo("BBG000BPHFS9"));
            // price will change with each call so just check not empty
            Assert.That(instrumentEntry1[0], Is.Not.Empty);
            
            // check instrument 2 entry
            var instrumentEntry2 = entries[2].Split("|");
            Assert.That(instrumentEntry2[0], Is.EqualTo("BBG000BVPV84"));
            // price will change with each call so just check not empty
            Assert.That(instrumentEntry2[0], Is.Not.Empty);
        }
        
        [Test]
        public void FinDataEx_GetData_ForEquityInstrumentMaster_ShouldProduceEqInsMasterFile()
        {
            var commandArgs = $"GetData -i BBG000BPHFS9 BBG000BVPV84 -o {_tempOutputDir} -d ID_BB_GLOBAL TICKER ID_ISIN ID_CUSIP SECURITY_TYP CRNCY COUNTRY_ISO EXCH_CODE INDUSTRY_SECTOR AMT_ISSUED SECURITY_DES";
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
            Assert.That(entries[0], Is.EqualTo("ID_BB_GLOBAL|TICKER|ID_ISIN|ID_CUSIP|SECURITY_TYP|CRNCY|COUNTRY_ISO|EXCH_CODE|INDUSTRY_SECTOR|AMT_ISSUED|SECURITY_DES"));

            // check instrument 1 entry
            var instrumentEntry1 = entries[1].Split("|");
            Assert.That(instrumentEntry1[0], Is.EqualTo("BBG000BPHFS9"));
            Assert.That(instrumentEntry1[1], Is.EqualTo("MSFT"));
            Assert.That(instrumentEntry1[2], Is.EqualTo("US5949181045"));
            Assert.That(instrumentEntry1[3], Is.EqualTo("594918104"));
            Assert.That(instrumentEntry1[4], Is.EqualTo("Common Stock"));
            Assert.That(instrumentEntry1[5], Is.EqualTo("USD"));
            Assert.That(instrumentEntry1[6], Is.EqualTo("US"));
            Assert.That(instrumentEntry1[7], Is.EqualTo("UW"));
            Assert.That(instrumentEntry1[8], Is.EqualTo("Technology"));
            Assert.That(instrumentEntry1[9], Is.EqualTo("")); 
            Assert.That(instrumentEntry1[10], Is.EqualTo("MSFT"));
            
            // check instrument 2 entry
            var instrumentEntry2 = entries[2].Split("|");
            Assert.That(instrumentEntry2[0], Is.EqualTo("BBG000BVPV84"));
            Assert.That(instrumentEntry2[1], Is.EqualTo("AMZN"));
            Assert.That(instrumentEntry2[2], Is.EqualTo("US0231351067"));
            Assert.That(instrumentEntry2[3], Is.EqualTo("023135106"));
            Assert.That(instrumentEntry1[4], Is.EqualTo("Common Stock"));
            Assert.That(instrumentEntry2[5], Is.EqualTo("USD"));
            Assert.That(instrumentEntry2[6], Is.EqualTo("US"));
            Assert.That(instrumentEntry2[7], Is.EqualTo("US"));
            Assert.That(instrumentEntry2[8], Is.EqualTo("Communications"));
            Assert.That(instrumentEntry2[9], Is.EqualTo("")); 
            Assert.That(instrumentEntry2[10], Is.EqualTo("AMZN"));
        }
    }
}