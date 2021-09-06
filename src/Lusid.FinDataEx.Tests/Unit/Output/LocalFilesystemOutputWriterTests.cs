using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lusid.FinDataEx.DataLicense.Service.Transform;
using Lusid.FinDataEx.Output;
using NUnit.Framework;
using static Lusid.FinDataEx.Tests.Unit.TestUtils;

namespace Lusid.FinDataEx.Tests.Unit.Output
{
    [TestFixture]
    public class LocalFilesystemOutputWriterTests
    {
        
        private static readonly string TempOutputDir = $"TempTestDir_{nameof(LocalFilesystemOutputWriterTests)}";
        private readonly string _outputFilePath = TempOutputDir + Path.DirectorySeparatorChar + "Output_{REQUEST_ID}.csv";
        private LocalFilesystemOutputWriter _outputWriter;
        [SetUp]
        public void SetUp()
        {
            SetupTempTestDirectory(TempOutputDir);
            var options = new DataLicenseOptions
            {
                OutputPath = _outputFilePath
            };
            _outputWriter = new LocalFilesystemOutputWriter(options);
        }
        
        [TearDown]
        public void TearDown()
        {
            TearDownTempTestDirectory(TempOutputDir);
        }

        [Test]
        public void Write_OnValidFinData_ShouldWriteToOutputDir()
        {
            var finDataOutput = CreateFinDataEntry("id_1_GetData");

            var writeResult = _outputWriter.Write(finDataOutput);
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            Assert.That(writeResult.FileOutputPath, Is.EqualTo($"{TempOutputDir}{Path.DirectorySeparatorChar}Output_id_1_GetData.csv"));
            Assert.That(writeResult.FileOutputPath, Does.Exist);
            
            // ensure file is properly populated
            var entries = File.ReadAllLines(writeResult.FileOutputPath);
            // check headers
            Assert.That(entries[0], Is.EqualTo("h1|h2|h3"));
            Assert.That(entries[1], Is.EqualTo("entry1Record1|entry2Record1|entry3Record1"));
            Assert.That(entries[2], Is.EqualTo("entry1Record2|entry2Record2|entry3Record2"));
        }
        
        [Test]
        public void Write_OnValidCorpActionData_ShouldWriteToOutputDir()
        {
            var responseId = "1603798418-1052073180_ValidActions";
            var retrieveGetActionResponse = LoadGetActionsResponseFromFile(responseId);
            var getActionOutput = new GetActionResponseTransformer().Transform(retrieveGetActionResponse);

            var writeResult = _outputWriter.Write(getActionOutput);
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            Assert.That(writeResult.FileOutputPath, Is.EqualTo($"{TempOutputDir}{Path.DirectorySeparatorChar}Output_1603798418-1052073180_ValidActions.csv"));
            Assert.That(writeResult.FileOutputPath, Does.Exist);
            
            // ensure file is properly populated
            var entries = File.ReadAllLines(writeResult.FileOutputPath);
            // check headers
            Assert.That(entries[0], Is.EqualTo("timeStarted|timeFinished|companyId|companyIdSpecified|securityId|securityIdSpecified|actionId|actionIdSpecified|mnemonic|flag|companyName|secIdType|secId|currency|marketSectorDes|marketSectorDesSpecified|bbUnique|announceDate|effectiveDate|amendDate|bbGlobal|bbGlobalCompany|bbSecNumDes|feedSource|CP_RECORD_DT|CP_PAY_DT|CP_FREQ|CP_NET_AMT|CP_TAX_AMT|CP_GROSS_AMT|CP_FRANKED_AMT|CP_DVD_CRNCY|CP_DVD_TYP|CP_SPPL_AMT|CP_FOREIGN_AMT|CP_PAR_PCT|CP_STOCK_OPT|CP_REINVEST_RATIO|CP_PX|CP_TAX_RT|CP_ADJ|CP_ADJ_DT|CP_INDICATOR|CP_DVD_DRP_DISCOUNT|CP_EUSD_TID|CP_EUSD_TID_SW|CP_DIST_AMT_STATUS|CP_TERMS|CP_TKR1|CP_RIGHTS_PER_SHARE|CP_SHARES_PER_RIGHT|CP_SUB_START_DT|CP_SUB_END_DT|CP_TRADE_START_DT|CP_TRADE_END_DT|CP_RIGHTS_SHARES|CP_RIGHTS_TKR1|CP_TKR1_ID_BB_GLOBAL|CP_TKR1_ID_BB_GLOBAL_COMPANY|CP_TKR1_ID_BB_SEC_NUM_DES|CP_TKR1_FEED_SOURCE|CP_RIGHTS1_ID_BB_GLOBAL|CP_RIGHTS1_ID_BB_GLOBAL_COMPANY|CP_RIGHTS1_ID_BB_SEC_NUM_DES|CP_RIGHTS1_FEED_SOURCE|CP_NOTES|CP_ISSUANCE_FEE|CP_DEPOSITARY|CP_ELECTION_DT|CP_ACTION_STATUS|CP_INFO_SOURCE"));
            Assert.That(entries[1], Is.EqualTo("27/10/2020 18:06:20 +00:00|27/10/2020 18:06:22 +00:00|101710|True|1000|True|223403897|True|DVD_CASH|U|Cintas Corp|CUSIP|172908105|USD|Equity|True|EQ0010171000001000|10/27/2020|11/05/2020|10/27/2020|BBG000H3YXF8|BBG001FDZS57|CTAS|US|11/06/2020|12/04/2020|4|N.A.| |3.51| |USD|1000| | | |U|N.A.|N.A.| |1.000000|11/05/2020|N|N.A.| | |F|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|N.A.|R|10"));
        }
        
        [Test]
        public void Write_OnEmptyInput_ShouldWriteFileWithHeaderOnly()
        {
            var headers = new List<string>{"h1","h2","h3"};
            var records = new List<Dictionary<string, string>>();
            var finDataOutput = new DataLicenseOutput("id_GetData", headers, records);

            var writeResult = _outputWriter.Write(finDataOutput);
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            
            // ensure file is properly populated
            var entries = File.ReadAllLines($"{TempOutputDir}{Path.DirectorySeparatorChar}Output_id_GetData.csv");
            // contains headers only
            Assert.That(entries.Length, Is.EqualTo(1));
            // check headers
            Assert.That(entries[0], Is.EqualTo("h1|h2|h3"));
        }
        
        [Test]
        public void Write_OnNoFinData_ShouldDoNothingButReturnOk()
        {
            var finDataOutput = DataLicenseOutput.Empty("requestId_that_returned_no_data");

            var writeResult = _outputWriter.Write(finDataOutput);
            
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.NotRun));
            Assert.False(Directory.EnumerateFileSystemEntries(TempOutputDir).Any());
        }
        
        [Test]
        public void Write_OnNonExistingOutputDir_ShouldReturnFail()
        {
            var nonExistingPath = Path.Combine(new[]{"this","Should","Not","Exist123"});
            var options = new DataLicenseOptions
            {
                OutputPath = nonExistingPath
            };
            _outputWriter = new LocalFilesystemOutputWriter(options);

            DataLicenseOutput finDataOutput = CreateFinDataEntry("id_1_GetData");
            var writeResult = _outputWriter.Write(finDataOutput);
            
            //verify
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Fail));
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