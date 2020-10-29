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
        
        [Test]
        public void FinDataEx_GetData_ForBondnstrumentMaster_ShouldProduceBondInsMasterFile()
        {
            var commandArgs = $"GetData -i BBG00HPJL7D0 BBG00RN2M5S4 -o {_tempOutputDir} -d ID_BB_GLOBAL TICKER ID_ISIN ID_CUSIP SECURITY_TYP CRNCY COUNTRY_ISO EXCH_CODE INDUSTRY_SECTOR AMT_ISSUED SECURITY_DES " +
                              $"ISSUE_DT MATURITY CPN PAR_AMT CPN_CRNCY CPN_FREQ DAY_CNT_DES FLT_CPN_CONVENTION";
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
            Assert.That(entries[0], Is.EqualTo("ID_BB_GLOBAL|TICKER|ID_ISIN|ID_CUSIP|SECURITY_TYP|CRNCY|COUNTRY_ISO|" +
                                               "EXCH_CODE|INDUSTRY_SECTOR|AMT_ISSUED|SECURITY_DES|ISSUE_DT|MATURITY|" +
                                               "CPN|PAR_AMT|CPN_CRNCY|CPN_FREQ|DAY_CNT_DES|FLT_CPN_CONVENTION"));

            // check instrument 1 entry
            var instrumentEntry1 = entries[1].Split("|");
            Assert.That(instrumentEntry1[0], Is.EqualTo("BBG00HPJL7D0"));
            Assert.That(instrumentEntry1[1], Is.EqualTo("SOFTBK"));
            Assert.That(instrumentEntry1[2], Is.EqualTo("XS1684385161"));
            Assert.That(instrumentEntry1[3], Is.EqualTo("AP1167257"));
            Assert.That(instrumentEntry1[4], Is.EqualTo("EURO NON-DOLLAR"));
            Assert.That(instrumentEntry1[5], Is.EqualTo("EUR"));
            Assert.That(instrumentEntry1[6], Is.EqualTo("JP"));
            Assert.That(instrumentEntry1[7], Is.EqualTo("SGX-ST"));
            Assert.That(instrumentEntry1[8], Is.EqualTo("Communications"));
            Assert.That(instrumentEntry1[9], Is.EqualTo("1500000000.00")); 
            Assert.That(instrumentEntry1[10], Is.EqualTo("SOFTBK 3 1/8 09/19/25"));
            Assert.That(instrumentEntry1[11], Is.EqualTo("09/19/2017"));
            Assert.That(instrumentEntry1[12], Is.EqualTo("09/19/2025"));
            Assert.That(instrumentEntry1[13], Is.EqualTo("3.125000"));
            Assert.That(instrumentEntry1[14], Is.EqualTo("1000.000000000"));
            Assert.That(instrumentEntry1[15], Is.EqualTo("EUR"));
            Assert.That(instrumentEntry1[16], Is.EqualTo("2"));
            Assert.That(instrumentEntry1[17], Is.EqualTo("ISMA-30/360"));
            Assert.That(instrumentEntry1[18], Is.EqualTo(""));

            // check instrument 2 entry
            var instrumentEntry2 = entries[2].Split("|");
            Assert.That(instrumentEntry2[0], Is.EqualTo("BBG00RN2M5S4"));
            Assert.That(instrumentEntry2[1], Is.EqualTo("T"));
            Assert.That(instrumentEntry2[2], Is.EqualTo("US912828Z864"));
            Assert.That(instrumentEntry2[3], Is.EqualTo("912828Z86"));
            Assert.That(instrumentEntry2[4], Is.EqualTo("US GOVERNMENT"));
            Assert.That(instrumentEntry2[5], Is.EqualTo("USD"));
            Assert.That(instrumentEntry2[6], Is.EqualTo("US"));
            Assert.That(instrumentEntry2[7], Is.EqualTo("FRANKFURT"));
            Assert.That(instrumentEntry2[8], Is.EqualTo("Government"));
            Assert.That(instrumentEntry2[9], Is.EqualTo("54900000000")); 
            Assert.That(instrumentEntry2[10], Is.EqualTo("T 1 3/8 02/15/23"));
            Assert.That(instrumentEntry2[11], Is.EqualTo("02/18/2020"));
            Assert.That(instrumentEntry2[12], Is.EqualTo("02/15/2023"));
            Assert.That(instrumentEntry2[13], Is.EqualTo("1.375000"));
            Assert.That(instrumentEntry2[14], Is.EqualTo("100"));
            Assert.That(instrumentEntry2[15], Is.EqualTo("USD"));
            Assert.That(instrumentEntry2[16], Is.EqualTo("2"));
            Assert.That(instrumentEntry2[17], Is.EqualTo("ACT/ACT"));
            Assert.That(instrumentEntry2[18], Is.EqualTo(""));
        }
        
        [Test]
        public void FinDataEx_GetData_ForMultiInstrumentMaster_ShouldProduceBondMultiInstrumentMasterFile()
        {
            var commandArgs = $"GetData -i BBG000BPHFS9 BBG000BVPV84 BBG00HPJL7D0 BBG00RN2M5S4 -o {_tempOutputDir} -d ID_BB_GLOBAL TICKER ID_ISIN ID_CUSIP SECURITY_TYP CRNCY COUNTRY_ISO EXCH_CODE INDUSTRY_SECTOR AMT_ISSUED SECURITY_DES " +
                              $"ISSUE_DT MATURITY CPN PAR_AMT CPN_CRNCY CPN_FREQ DAY_CNT_DES FLT_CPN_CONVENTION";
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
            Assert.That(entries[0], Is.EqualTo("ID_BB_GLOBAL|TICKER|ID_ISIN|ID_CUSIP|SECURITY_TYP|CRNCY|COUNTRY_ISO|" +
                                               "EXCH_CODE|INDUSTRY_SECTOR|AMT_ISSUED|SECURITY_DES|ISSUE_DT|MATURITY|" +
                                               "CPN|PAR_AMT|CPN_CRNCY|CPN_FREQ|DAY_CNT_DES|FLT_CPN_CONVENTION"));

            
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
            Assert.That(instrumentEntry1[11], Is.EqualTo(""));
            Assert.That(instrumentEntry1[12], Is.EqualTo(""));
            Assert.That(instrumentEntry1[13], Is.EqualTo(""));
            Assert.That(instrumentEntry1[14], Is.EqualTo(".000006250"));
            Assert.That(instrumentEntry1[15], Is.EqualTo(""));
            Assert.That(instrumentEntry1[16], Is.EqualTo(""));
            Assert.That(instrumentEntry1[17], Is.EqualTo(""));
            Assert.That(instrumentEntry1[18], Is.EqualTo(""));
            
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
            Assert.That(instrumentEntry2[11], Is.EqualTo(""));
            Assert.That(instrumentEntry2[12], Is.EqualTo(""));
            Assert.That(instrumentEntry2[13], Is.EqualTo(""));
            Assert.That(instrumentEntry2[14], Is.EqualTo(".010000000"));
            Assert.That(instrumentEntry2[15], Is.EqualTo(""));
            Assert.That(instrumentEntry2[16], Is.EqualTo(""));
            Assert.That(instrumentEntry2[17], Is.EqualTo(""));
            Assert.That(instrumentEntry2[18], Is.EqualTo(""));
            
            // check instrument 3 entry
            var instrumentEntry3 = entries[3].Split("|");
            Assert.That(instrumentEntry3[0], Is.EqualTo("BBG00HPJL7D0"));
            Assert.That(instrumentEntry3[1], Is.EqualTo("SOFTBK"));
            Assert.That(instrumentEntry3[2], Is.EqualTo("XS1684385161"));
            Assert.That(instrumentEntry3[3], Is.EqualTo("AP1167257"));
            Assert.That(instrumentEntry3[4], Is.EqualTo("EURO NON-DOLLAR"));
            Assert.That(instrumentEntry3[5], Is.EqualTo("EUR"));
            Assert.That(instrumentEntry3[6], Is.EqualTo("JP"));
            Assert.That(instrumentEntry3[7], Is.EqualTo("SGX-ST"));
            Assert.That(instrumentEntry3[8], Is.EqualTo("Communications"));
            Assert.That(instrumentEntry3[9], Is.EqualTo("1500000000.00")); 
            Assert.That(instrumentEntry3[10], Is.EqualTo("SOFTBK 3 1/8 09/19/25"));
            Assert.That(instrumentEntry3[11], Is.EqualTo("09/19/2017"));
            Assert.That(instrumentEntry3[12], Is.EqualTo("09/19/2025"));
            Assert.That(instrumentEntry3[13], Is.EqualTo("3.125000"));
            Assert.That(instrumentEntry3[14], Is.EqualTo("1000.000000000"));
            Assert.That(instrumentEntry3[15], Is.EqualTo("EUR"));
            Assert.That(instrumentEntry3[16], Is.EqualTo("2"));
            Assert.That(instrumentEntry3[17], Is.EqualTo("ISMA-30/360"));
            Assert.That(instrumentEntry3[18], Is.EqualTo(""));

            // check instrument 4 entry
            var instrumentEntry4 = entries[4].Split("|");
            Assert.That(instrumentEntry4[0], Is.EqualTo("BBG00RN2M5S4"));
            Assert.That(instrumentEntry4[1], Is.EqualTo("T"));
            Assert.That(instrumentEntry4[2], Is.EqualTo("US912828Z864"));
            Assert.That(instrumentEntry4[3], Is.EqualTo("912828Z86"));
            Assert.That(instrumentEntry4[4], Is.EqualTo("US GOVERNMENT"));
            Assert.That(instrumentEntry4[5], Is.EqualTo("USD"));
            Assert.That(instrumentEntry4[6], Is.EqualTo("US"));
            Assert.That(instrumentEntry4[7], Is.EqualTo("FRANKFURT"));
            Assert.That(instrumentEntry4[8], Is.EqualTo("Government"));
            Assert.That(instrumentEntry4[9], Is.EqualTo("54900000000")); 
            Assert.That(instrumentEntry4[10], Is.EqualTo("T 1 3/8 02/15/23"));
            Assert.That(instrumentEntry4[11], Is.EqualTo("02/18/2020"));
            Assert.That(instrumentEntry4[12], Is.EqualTo("02/15/2023"));
            Assert.That(instrumentEntry4[13], Is.EqualTo("1.375000"));
            Assert.That(instrumentEntry4[14], Is.EqualTo("100"));
            Assert.That(instrumentEntry4[15], Is.EqualTo("USD"));
            Assert.That(instrumentEntry4[16], Is.EqualTo("2"));
            Assert.That(instrumentEntry4[17], Is.EqualTo("ACT/ACT"));
            Assert.That(instrumentEntry4[18], Is.EqualTo(""));
        }
        
        
        /* Corporate Actions */
        [Test]
        public void FinDataEx_GetAction_OnValidEquityBbgId_ShouldProduceCorpActionFile()
        {
            var commandArgs = $"GetAction -i BBG000BPHFS9 BBG000BVPV84 -o {_tempOutputDir} -d ID_BB_GLOBAL PX_LAST";
            FinDataEx.Main(commandArgs.Split(" "));
            
            //verify
            var expectedDataFiles = Directory.GetFiles(_tempOutputDir);
            
            // ensure only GetData file created and name is in correct format
            // more than one file is a contaminated test and should be investigated
            Assert.That(expectedDataFiles.Length, Is.EqualTo(1));
            StringAssert.EndsWith("_GetAction.csv", expectedDataFiles[0]);

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
        
        
    }
}