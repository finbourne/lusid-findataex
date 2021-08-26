using System.IO;
using System.Linq;
using NUnit.Framework;
using static Lusid.FinDataEx.Tests.Unit.TestUtils;

namespace Lusid.FinDataEx.Tests.Integration
{
    [TestFixture]
    [Category("Unsafe")]
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
            var filepath = $"{_tempOutputDir + Path.DirectorySeparatorChar}dl_request_output.csv";
            var commandArgs = $"getdata -i InstrumentSource -a BBG000BPHFS9 BBG000BVPV84 -f {filepath} -d ID_BB_GLOBAL PX_LAST --unsafe";
            var exitCode = FinDataEx.Main(commandArgs.Split(" "));
            
            // ensure ran to success
            Assert.That(exitCode, Is.EqualTo(0));
            
            // ensure file is properly populated
            var entries = File.ReadAllLines(filepath);
            
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
        public void FinDataEx_GetData_ForFxUsingTicker_ShouldProduceFxRateFile()
        {
            var filepath = $"{_tempOutputDir + Path.DirectorySeparatorChar}dl_request_output.csv";
            var commandArgs = new[] {"getdata", "-f", filepath, "-i", "InstrumentSource", "-a", "GBP", "USD", "-y", "Curncy", "-t", "TICKER", "-d", "TICKER", "PX_LAST", "--unsafe"}; 
            var exitCode = FinDataEx.Main(commandArgs.ToArray());
            
            // ensure ran to success
            Assert.That(exitCode, Is.EqualTo(0));
            
            // ensure file is properly populated
            var entries = File.ReadAllLines(filepath);
            
            // check headers
            Assert.That(entries[0], Is.EqualTo("timeStarted|timeFinished|TICKER|PX_LAST"));

            // check instrument 1 entry
            var instrumentEntry1 = entries[1].Split("|");
            Assert.That(instrumentEntry1[0], Is.Not.Empty);
            Assert.That(instrumentEntry1[1], Is.Not.Empty);
            Assert.That(instrumentEntry1[2], Is.EqualTo("GBP"));
            Assert.That(instrumentEntry1[3], Is.Not.Empty); // price will change so check not empty
            
            var instrumentEntry2 = entries[2].Split("|");
            Assert.That(instrumentEntry2[0], Is.Not.Empty);
            Assert.That(instrumentEntry2[1], Is.Not.Empty);
            Assert.That(instrumentEntry2[2], Is.EqualTo("USD"));
            Assert.That(instrumentEntry2[3], Is.EqualTo("1.000000")); // should always be
        }

        [Test]
        public void FinDataEx_GetData_ForEquityInstrumentMaster_ShouldProduceEqInsMasterFile()
        {
            var filepath = $"{_tempOutputDir + Path.DirectorySeparatorChar}dl_request_output.csv";
            var commandArgs = $"getdata -i InstrumentSource -a BBG000BPHFS9 BBG000BVPV84 -f {filepath} -d ID_BB_GLOBAL TICKER ID_ISIN ID_CUSIP SECURITY_TYP CRNCY COUNTRY_ISO EXCH_CODE INDUSTRY_SECTOR AMT_ISSUED SECURITY_DES --unsafe";
            var exitCode = FinDataEx.Main(commandArgs.Split(" "));
            
            // ensure ran to success
            Assert.That(exitCode, Is.EqualTo(0));
            
            // ensure file is properly populated
            var entries = File.ReadAllLines(filepath);
            
            // check headers
            Assert.That(entries[0], Is.EqualTo("timeStarted|timeFinished|ID_BB_GLOBAL|TICKER|ID_ISIN|ID_CUSIP|SECURITY_TYP|CRNCY|COUNTRY_ISO|EXCH_CODE|INDUSTRY_SECTOR|AMT_ISSUED|SECURITY_DES"));

            // check instrument 1 entry
            var instrumentEntry1 = entries[1].Split("|");
            Assert.That(instrumentEntry1[0], Is.Not.Empty);
            Assert.That(instrumentEntry1[1], Is.Not.Empty);
            Assert.That(instrumentEntry1[2], Is.EqualTo("BBG000BPHFS9"));
            Assert.That(instrumentEntry1[3], Is.EqualTo("MSFT"));
            Assert.That(instrumentEntry1[4], Is.EqualTo("US5949181045"));
            Assert.That(instrumentEntry1[5], Is.EqualTo("594918104"));
            Assert.That(instrumentEntry1[6], Is.EqualTo("Common Stock"));
            Assert.That(instrumentEntry1[7], Is.EqualTo("USD"));
            Assert.That(instrumentEntry1[8], Is.EqualTo("US"));
            Assert.That(instrumentEntry1[9], Is.EqualTo("UW"));
            Assert.That(instrumentEntry1[10], Is.EqualTo("Technology"));
            Assert.That(instrumentEntry1[11], Is.EqualTo("")); 
            Assert.That(instrumentEntry1[12], Is.EqualTo("MSFT"));
            
            // check instrument 2 entry
            var instrumentEntry2 = entries[2].Split("|");
            Assert.That(instrumentEntry2[0], Is.Not.Empty);
            Assert.That(instrumentEntry2[1], Is.Not.Empty);
            Assert.That(instrumentEntry2[2], Is.EqualTo("BBG000BVPV84"));
            Assert.That(instrumentEntry2[3], Is.EqualTo("AMZN"));
            Assert.That(instrumentEntry2[4], Is.EqualTo("US0231351067"));
            Assert.That(instrumentEntry2[5], Is.EqualTo("023135106"));
            Assert.That(instrumentEntry1[6], Is.EqualTo("Common Stock"));
            Assert.That(instrumentEntry2[7], Is.EqualTo("USD"));
            Assert.That(instrumentEntry2[8], Is.EqualTo("US"));
            Assert.That(instrumentEntry2[9], Is.EqualTo("US"));
            Assert.That(instrumentEntry2[10], Is.EqualTo("Communications"));
            Assert.That(instrumentEntry2[11], Is.EqualTo("")); 
            Assert.That(instrumentEntry2[12], Is.EqualTo("AMZN"));
        }
        
        [Test]
        public void FinDataEx_GetData_ForBondInstrumentMaster_ShouldProduceBondInsMasterFile()
        {
            var filepath = $"{_tempOutputDir + Path.DirectorySeparatorChar}dl_request_output.csv";
            var commandArgs = $"getdata -i InstrumentSource -a BBG00HPJL7D0 BBG00RN2M5S4 -f {filepath} -d ID_BB_GLOBAL TICKER ID_ISIN ID_CUSIP SECURITY_TYP CRNCY COUNTRY_ISO EXCH_CODE INDUSTRY_SECTOR AMT_ISSUED SECURITY_DES " +
                              $"ISSUE_DT MATURITY CPN PAR_AMT CPN_CRNCY CPN_FREQ DAY_CNT_DES FLT_CPN_CONVENTION --unsafe";
            var exitCode = FinDataEx.Main(commandArgs.Split(" "));

            // ensure ran to success
            Assert.That(exitCode, Is.EqualTo(0));
            
            // ensure file is properly populated
            var entries = File.ReadAllLines(filepath);
            
            // check headers
            Assert.That(entries[0], Is.EqualTo("timeStarted|timeFinished|ID_BB_GLOBAL|TICKER|ID_ISIN|ID_CUSIP|SECURITY_TYP|CRNCY|COUNTRY_ISO|" +
                                               "EXCH_CODE|INDUSTRY_SECTOR|AMT_ISSUED|SECURITY_DES|ISSUE_DT|MATURITY|" +
                                               "CPN|PAR_AMT|CPN_CRNCY|CPN_FREQ|DAY_CNT_DES|FLT_CPN_CONVENTION"));

            // check instrument 1 entry
            var instrumentEntry1 = entries[1].Split("|");
            Assert.That(instrumentEntry1[0], Is.Not.Empty);
            Assert.That(instrumentEntry1[1], Is.Not.Empty);
            Assert.That(instrumentEntry1[2], Is.EqualTo("BBG00HPJL7D0"));
            Assert.That(instrumentEntry1[3], Is.EqualTo("SOFTBK"));
            Assert.That(instrumentEntry1[4], Is.EqualTo("XS1684385161"));
            Assert.That(instrumentEntry1[5], Is.EqualTo("AP1167257"));
            Assert.That(instrumentEntry1[6], Is.EqualTo("EURO NON-DOLLAR"));
            Assert.That(instrumentEntry1[7], Is.EqualTo("EUR"));
            Assert.That(instrumentEntry1[8], Is.EqualTo("JP"));
            Assert.That(instrumentEntry1[9], Is.EqualTo("SGX-ST"));
            Assert.That(instrumentEntry1[10], Is.EqualTo("Communications"));
            Assert.That(instrumentEntry1[11], Is.EqualTo("1500000000.00")); 
            Assert.That(instrumentEntry1[12], Is.EqualTo("SOFTBK 3 1/8 09/19/25"));
            Assert.That(instrumentEntry1[13], Is.EqualTo("09/19/2017"));
            Assert.That(instrumentEntry1[14], Is.EqualTo("09/19/2025"));
            Assert.That(instrumentEntry1[15], Is.EqualTo("3.125000"));
            Assert.That(instrumentEntry1[16], Is.EqualTo("1000.000000000"));
            Assert.That(instrumentEntry1[17], Is.EqualTo("EUR"));
            Assert.That(instrumentEntry1[18], Is.EqualTo("2"));
            Assert.That(instrumentEntry1[19], Is.EqualTo("ISMA-30/360"));
            Assert.That(instrumentEntry1[20], Is.EqualTo(""));

            // check instrument 2 entry
            var instrumentEntry2 = entries[2].Split("|");
            Assert.That(instrumentEntry2[0], Is.Not.Empty);
            Assert.That(instrumentEntry2[1], Is.Not.Empty);
            Assert.That(instrumentEntry2[2], Is.EqualTo("BBG00RN2M5S4"));
            Assert.That(instrumentEntry2[3], Is.EqualTo("T"));
            Assert.That(instrumentEntry2[4], Is.EqualTo("US912828Z864"));
            Assert.That(instrumentEntry2[5], Is.EqualTo("912828Z86"));
            Assert.That(instrumentEntry2[6], Is.EqualTo("US GOVERNMENT"));
            Assert.That(instrumentEntry2[7], Is.EqualTo("USD"));
            Assert.That(instrumentEntry2[8], Is.EqualTo("US"));
            Assert.That(instrumentEntry2[9], Is.EqualTo("FRANKFURT"));
            Assert.That(instrumentEntry2[10], Is.EqualTo("Government"));
            Assert.That(instrumentEntry2[11], Is.EqualTo("54900000000")); 
            Assert.That(instrumentEntry2[12], Is.EqualTo("T 1 3/8 02/15/23"));
            Assert.That(instrumentEntry2[13], Is.EqualTo("02/18/2020"));
            Assert.That(instrumentEntry2[14], Is.EqualTo("02/15/2023"));
            Assert.That(instrumentEntry2[15], Is.EqualTo("1.375000"));
            Assert.That(instrumentEntry2[16], Is.EqualTo("100"));
            Assert.That(instrumentEntry2[17], Is.EqualTo("USD"));
            Assert.That(instrumentEntry2[18], Is.EqualTo("2"));
            Assert.That(instrumentEntry2[19], Is.EqualTo("ACT/ACT"));
            Assert.That(instrumentEntry2[20], Is.EqualTo(""));
        }
        
        [Test]
        public void FinDataEx_GetData_ForMultiInstrumentMaster_ShouldProduceBondMultiInstrumentMasterFile()
        {
            var filepath = $"{_tempOutputDir + Path.DirectorySeparatorChar}dl_request_output.csv";
            var commandArgs = $"getdata -i InstrumentSource -a BBG000BPHFS9 BBG000BVPV84 BBG00HPJL7D0 BBG00RN2M5S4 -f {filepath} -d ID_BB_GLOBAL TICKER ID_ISIN ID_CUSIP SECURITY_TYP CRNCY COUNTRY_ISO EXCH_CODE INDUSTRY_SECTOR AMT_ISSUED SECURITY_DES " +
                              $"ISSUE_DT MATURITY CPN PAR_AMT CPN_CRNCY CPN_FREQ DAY_CNT_DES FLT_CPN_CONVENTION --unsafe";
            var exitCode = FinDataEx.Main(commandArgs.Split(" "));
            
            // ensure ran to success
            Assert.That(exitCode, Is.EqualTo(0));
            
            // ensure file is properly populated
            var entries = File.ReadAllLines(filepath);
            
            // check headers
            Assert.That(entries[0], Is.EqualTo("timeStarted|timeFinished|ID_BB_GLOBAL|TICKER|ID_ISIN|ID_CUSIP|SECURITY_TYP|CRNCY|COUNTRY_ISO|" +
                                               "EXCH_CODE|INDUSTRY_SECTOR|AMT_ISSUED|SECURITY_DES|ISSUE_DT|MATURITY|" +
                                               "CPN|PAR_AMT|CPN_CRNCY|CPN_FREQ|DAY_CNT_DES|FLT_CPN_CONVENTION"));

            
            // check instrument 1 entry
            var instrumentEntry1 = entries[1].Split("|");
            Assert.That(instrumentEntry1[0], Is.Not.Empty);
            Assert.That(instrumentEntry1[1], Is.Not.Empty);
            Assert.That(instrumentEntry1[2], Is.EqualTo("BBG000BPHFS9"));
            Assert.That(instrumentEntry1[3], Is.EqualTo("MSFT"));
            Assert.That(instrumentEntry1[4], Is.EqualTo("US5949181045"));
            Assert.That(instrumentEntry1[5], Is.EqualTo("594918104"));
            Assert.That(instrumentEntry1[6], Is.EqualTo("Common Stock"));
            Assert.That(instrumentEntry1[7], Is.EqualTo("USD"));
            Assert.That(instrumentEntry1[8], Is.EqualTo("US"));
            Assert.That(instrumentEntry1[9], Is.EqualTo("UW"));
            Assert.That(instrumentEntry1[10], Is.EqualTo("Technology"));
            Assert.That(instrumentEntry1[11], Is.EqualTo("")); 
            Assert.That(instrumentEntry1[12], Is.EqualTo("MSFT"));
            Assert.That(instrumentEntry1[13], Is.EqualTo(""));
            Assert.That(instrumentEntry1[14], Is.EqualTo(""));
            Assert.That(instrumentEntry1[15], Is.EqualTo(""));
            Assert.That(instrumentEntry1[16], Is.EqualTo(".000006250"));
            Assert.That(instrumentEntry1[17], Is.EqualTo(""));
            Assert.That(instrumentEntry1[18], Is.EqualTo(""));
            Assert.That(instrumentEntry1[19], Is.EqualTo(""));
            Assert.That(instrumentEntry1[20], Is.EqualTo(""));
            
            // check instrument 2 entry
            var instrumentEntry2 = entries[2].Split("|");
            Assert.That(instrumentEntry2[0], Is.Not.Empty);
            Assert.That(instrumentEntry2[1], Is.Not.Empty);
            Assert.That(instrumentEntry2[2], Is.EqualTo("BBG000BVPV84"));
            Assert.That(instrumentEntry2[3], Is.EqualTo("AMZN"));
            Assert.That(instrumentEntry2[4], Is.EqualTo("US0231351067"));
            Assert.That(instrumentEntry2[5], Is.EqualTo("023135106"));
            Assert.That(instrumentEntry1[6], Is.EqualTo("Common Stock"));
            Assert.That(instrumentEntry2[7], Is.EqualTo("USD"));
            Assert.That(instrumentEntry2[8], Is.EqualTo("US"));
            Assert.That(instrumentEntry2[9], Is.EqualTo("US"));
            Assert.That(instrumentEntry2[10], Is.EqualTo("Communications"));
            Assert.That(instrumentEntry2[11], Is.EqualTo("")); 
            Assert.That(instrumentEntry2[12], Is.EqualTo("AMZN"));
            Assert.That(instrumentEntry2[13], Is.EqualTo(""));
            Assert.That(instrumentEntry2[14], Is.EqualTo(""));
            Assert.That(instrumentEntry2[15], Is.EqualTo(""));
            Assert.That(instrumentEntry2[16], Is.EqualTo(".010000000"));
            Assert.That(instrumentEntry2[17], Is.EqualTo(""));
            Assert.That(instrumentEntry2[18], Is.EqualTo(""));
            Assert.That(instrumentEntry2[19], Is.EqualTo(""));
            Assert.That(instrumentEntry2[20], Is.EqualTo(""));
            
            // check instrument 3 entry
            var instrumentEntry3 = entries[3].Split("|");
            Assert.That(instrumentEntry3[0], Is.Not.Empty);
            Assert.That(instrumentEntry3[1], Is.Not.Empty);
            Assert.That(instrumentEntry3[2], Is.EqualTo("BBG00HPJL7D0"));
            Assert.That(instrumentEntry3[3], Is.EqualTo("SOFTBK"));
            Assert.That(instrumentEntry3[4], Is.EqualTo("XS1684385161"));
            Assert.That(instrumentEntry3[5], Is.EqualTo("AP1167257"));
            Assert.That(instrumentEntry3[6], Is.EqualTo("EURO NON-DOLLAR"));
            Assert.That(instrumentEntry3[7], Is.EqualTo("EUR"));
            Assert.That(instrumentEntry3[8], Is.EqualTo("JP"));
            Assert.That(instrumentEntry3[9], Is.EqualTo("SGX-ST"));
            Assert.That(instrumentEntry3[10], Is.EqualTo("Communications"));
            Assert.That(instrumentEntry3[11], Is.EqualTo("1500000000.00")); 
            Assert.That(instrumentEntry3[12], Is.EqualTo("SOFTBK 3 1/8 09/19/25"));
            Assert.That(instrumentEntry3[13], Is.EqualTo("09/19/2017"));
            Assert.That(instrumentEntry3[14], Is.EqualTo("09/19/2025"));
            Assert.That(instrumentEntry3[15], Is.EqualTo("3.125000"));
            Assert.That(instrumentEntry3[16], Is.EqualTo("1000.000000000"));
            Assert.That(instrumentEntry3[17], Is.EqualTo("EUR"));
            Assert.That(instrumentEntry3[18], Is.EqualTo("2"));
            Assert.That(instrumentEntry3[19], Is.EqualTo("ISMA-30/360"));
            Assert.That(instrumentEntry3[20], Is.EqualTo(""));

            // check instrument 4 entry
            var instrumentEntry4 = entries[4].Split("|");
            Assert.That(instrumentEntry4[0], Is.Not.Empty);
            Assert.That(instrumentEntry4[1], Is.Not.Empty);
            Assert.That(instrumentEntry4[2], Is.EqualTo("BBG00RN2M5S4"));
            Assert.That(instrumentEntry4[3], Is.EqualTo("T"));
            Assert.That(instrumentEntry4[4], Is.EqualTo("US912828Z864"));
            Assert.That(instrumentEntry4[5], Is.EqualTo("912828Z86"));
            Assert.That(instrumentEntry4[6], Is.EqualTo("US GOVERNMENT"));
            Assert.That(instrumentEntry4[7], Is.EqualTo("USD"));
            Assert.That(instrumentEntry4[8], Is.EqualTo("US"));
            Assert.That(instrumentEntry4[9], Is.EqualTo("FRANKFURT"));
            Assert.That(instrumentEntry4[10], Is.EqualTo("Government"));
            Assert.That(instrumentEntry4[11], Is.EqualTo("54900000000")); 
            Assert.That(instrumentEntry4[12], Is.EqualTo("T 1 3/8 02/15/23"));
            Assert.That(instrumentEntry4[13], Is.EqualTo("02/18/2020"));
            Assert.That(instrumentEntry4[14], Is.EqualTo("02/15/2023"));
            Assert.That(instrumentEntry4[15], Is.EqualTo("1.375000"));
            Assert.That(instrumentEntry4[16], Is.EqualTo("100"));
            Assert.That(instrumentEntry4[17], Is.EqualTo("USD"));
            Assert.That(instrumentEntry4[18], Is.EqualTo("2"));
            Assert.That(instrumentEntry4[19], Is.EqualTo("ACT/ACT"));
            Assert.That(instrumentEntry4[20], Is.EqualTo(""));
        }
        
        [Test]
        public void FinDataEx_GetData_OnValidBbgIdFromCsvInstrumentSource_ShouldProduceDataFile()
        {
            var filepath = $"{_tempOutputDir + Path.DirectorySeparatorChar}dl_request_output.csv";
            var instrumentSourceCsv = Path.Combine("Integration", "DataLicense", "Instrument", "TestData", "real_instruments.csv");
            
            var commandArgs = $"getdata -i CsvInstrumentSource -a {instrumentSourceCsv} -f {filepath} -d ID_BB_GLOBAL PX_LAST --unsafe";
            var exitCode = FinDataEx.Main(commandArgs.Split(" "));
            
            // ensure ran to success
            Assert.That(exitCode, Is.EqualTo(0));
            
            // ensure file is properly populated
            var entries = File.ReadAllLines(filepath);
            
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
        public void FinDataEx_GetData_ForFxUsingTickerFromCsvInstrumentSource_ShouldProduceFxRateFile()
        {
            var filepath = $"{_tempOutputDir + Path.DirectorySeparatorChar}dl_request_output.csv";
            var instrumentSourceCsv = Path.Combine("Integration", "DataLicense", "Instrument", "TestData", "currency_instruments.csv");
            var commandArgs = new[] {"getdata", "-f", filepath, "-i", "CsvInstrumentSource", "-a", instrumentSourceCsv, "-y", "Curncy", "-t", "TICKER", "-d", "TICKER", "PX_LAST", "--unsafe"}; 
            var exitCode = FinDataEx.Main(commandArgs.ToArray());
            
            // ensure ran to success
            Assert.That(exitCode, Is.EqualTo(0));
            
            // ensure file is properly populated
            var entries = File.ReadAllLines(filepath);
            
            // check headers
            Assert.That(entries[0], Is.EqualTo("timeStarted|timeFinished|TICKER|PX_LAST"));

            // check instrument 1 entry
            var instrumentEntry1 = entries[1].Split("|");
            Assert.That(instrumentEntry1[0], Is.Not.Empty);
            Assert.That(instrumentEntry1[1], Is.Not.Empty);
            Assert.That(instrumentEntry1[2], Is.EqualTo("GBP"));
            Assert.That(instrumentEntry1[3], Is.Not.Empty); // price will change so check not empty
            
            var instrumentEntry2 = entries[2].Split("|");
            Assert.That(instrumentEntry2[0], Is.Not.Empty);
            Assert.That(instrumentEntry2[1], Is.Not.Empty);
            Assert.That(instrumentEntry2[2], Is.EqualTo("USD"));
            Assert.That(instrumentEntry2[3], Is.EqualTo("1.000000")); // should always be
        }
        
        /* Corporate Actions */
        [Test]
        public void FinDataEx_GetAction_OnValidEquityBbgId_ShouldProduceCorpActionFile()
        {
            var filepath = $"{_tempOutputDir + Path.DirectorySeparatorChar}dl_request_output.csv";
            var commandArgs = $"getactions -i InstrumentSource -a BBG000BPHFS9 BBG000BVPV84 -f {filepath} -c DVD_CASH DVD_STOCK STOCK_SPLT --unsafe";
            var exitCode = FinDataEx.Main(commandArgs.Split(" "));
            // ensure ran to success
            Assert.That(exitCode, Is.EqualTo(0));
            // Most days will have no corp actions and as test always uses latest date cannot check for file as will
            // likely not exist. For tests on writing corp actions to file see GetActionsDataLicenseCallTest
            // In this case an successful call to DLWS is enough
        }
        
        /* No instruments sourced  */
        [Test]
        public void FinDataEx_GetData_OnNoInsturmentsFromSource_ShouldDoNothingButReturnAsSuccess()
        {
            var filepath = $"{_tempOutputDir + Path.DirectorySeparatorChar}dl_request_output.csv";
            var instrumentSourceCsv = Path.Combine(new[]{"Integration","DataLicense","Instrument","TestData",
                "empty_instruments.csv"});
            
            var commandArgs = $"getdata -i CsvInstrumentSource -a {instrumentSourceCsv} -f {filepath} -d ID_BB_GLOBAL PX_LAST --unsafe";
            var exitCode = FinDataEx.Main(commandArgs.Split(" "));
            
            // ensure ran to success
            Assert.That(exitCode, Is.EqualTo(0));
            
            // ensure no file is created
            Assert.False(File.Exists(filepath));
        }
        
        /* Maximum instruments threshold breached, */
        [Test]
        public void FinDataEx_GetData_OnBreachMaximumInstruments_ShouldFail()
        {
            var filepath = $"{_tempOutputDir + Path.DirectorySeparatorChar}dl_request_output.csv";
            var commandArgs = $"getdata -i InstrumentSource -a BBG000BPHFS9 BBG000BVPV84 -f {filepath} -d ID_BB_GLOBAL PX_LAST -m 1 --unsafe";
            var exitCode = FinDataEx.Main(commandArgs.Split(" "));
            Assert.That(exitCode, Is.EqualTo(1));
        }
        
        /* Exception Handling */
        [Test]
        public void FinDataEx_OnException_ShouldReturnFailureExitCode()
        {
            var missingAllRequiredArgs = $"getdata -i InstrumentSource --unsafe";
            var exitCode = FinDataEx.Main(missingAllRequiredArgs.Split(" "));
            Assert.That(exitCode, Is.EqualTo(1));
        }
        
        
    }
}