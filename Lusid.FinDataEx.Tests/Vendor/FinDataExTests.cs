using System.IO;
using Lusid.FinDataEx.Core;
using Lusid.FinDataEx.Vendor;
using NUnit.Framework;
using static Lusid.FinDataEx.Tests.Vendor.Util.TestUtils;

namespace Lusid.FinDataEx.Tests.Vendor
{
    [TestFixture]
    public class FinDataExTests
    {
        private readonly string _tempOutputDir = $"TempTestDir_{nameof(FinDataExTests)}";
        
        private FinDataEx _finDataEx;
        private FdeRequestBuilder _fdeRequestBuilder;
        private VendorExtractorBuilder _vendorExtractorBuilder;
        private FdeResponseProcessorBuilder _fdeResponseProcessorBuilder;

        [SetUp]
        public void SetUp()
        {
            SetupTempTestDirectory(_tempOutputDir);
            
            _fdeRequestBuilder = new FdeRequestBuilder();
            _vendorExtractorBuilder = new VendorExtractorBuilder();
            _fdeResponseProcessorBuilder = new FdeResponseProcessorBuilder(_tempOutputDir);
            _finDataEx = new FinDataEx(_fdeRequestBuilder, _vendorExtractorBuilder, _fdeResponseProcessorBuilder);
        }
        
        [TearDown]
        public void TearDown()
        {
            TearDownTempTestDirectory(_tempOutputDir);
        }
        
        [Test]
        public void Process_OnValidDlRequestToLptOutput_ShouldReturnIVendorResponseAndWriteToFile()
        {
            //when
            string fde_valid_request = "Vendor\\Dl\\TestData\\fde_request_dl_prices_file.json";

            //execute
            _finDataEx.Process(fde_valid_request);

            //verify
            string outputFile = _tempOutputDir + Path.DirectorySeparatorChar + "DL12345_Prices.csv";
            // ensure written to csv
            Assert.That(outputFile, Does.Exist);
            
            // check contents of csv as expected
            string[] finData = File.ReadAllLines(outputFile);
            
            // ensure all records have been returned
            Assert.That(finData, Has.Exactly(10).Items);
            
            // ensure correct headers constructed correctly
            Assert.AreEqual("Ticker|PX_ASK|PX_LAST|NoMapping", finData[0]);
            
            // ensure correct records
            Assert.AreEqual(
                "ABC Index|N.A.|1234.220000|FLD UNKNOWN", 
                finData[1]);
            
            Assert.AreEqual(
                "BRIGHTI Equity|6.174400|6.174400|FLD UNKNOWN", 
                finData[2]);
            
            Assert.AreEqual(
                "CNTRY_1  11/08/18 Govt|N.A.||FLD UNKNOWN", 
                finData[4]);

        }
        
        [Test]
        public void Process_OnValidDlCorpActionsRequestToLptOutput_ShouldReturnIVendorResponseAndWriteToFile()
        {
            //when
            string fde_valid_request = "Vendor\\Dl\\TestData\\fde_request_dl_corpactions_file.json";

            //execute
            _finDataEx.Process(fde_valid_request);

            //verify
            string dvdCashCorpActionFile = _tempOutputDir + Path.DirectorySeparatorChar + "DL30001_DVD_CASH.csv";
            string stockSplitCorpActionFile = _tempOutputDir + Path.DirectorySeparatorChar + "DL30001_STOCK_SPLT.csv";
            
            // ensure written to csv
            Assert.That(dvdCashCorpActionFile, Does.Exist);
            Assert.That(stockSplitCorpActionFile, Does.Exist);

            // check contents of dvd cash as expected
            string[] dvdCashFinData = File.ReadAllLines(dvdCashCorpActionFile);
            
            // ensure all records have been returned
            Assert.That(dvdCashFinData, Has.Exactly(7).Items);
            
            // ensure correct headers constructed correctly
            Assert.AreEqual("Ticker|CorpActionHeader_1|CorpActionHeader_2|CorpActionHeader_3|CorpActionHeader_4|CorpActionHeader_5|CorpActionHeader_6|CorpActionHeader_7|CorpActionHeader_8|CorpActionHeader_9|CorpActionHeader_10|CorpActionHeader_11|CorpActionHeader_12|CorpActionHeader_13|CorpActionHeader_14|CorpActionHeader_15|CorpActionHeader_16|CorpActionHeader_17|CorpActionHeader_18|CorpActionHeader_19|CorpActionHeader_20|CorpActionHeader_21|CorpActionHeader_22|CorpActionHeader_23|CorpActionHeader_24|CorpActionHeader_25|CorpActionHeader_26|CorpActionHeader_27|CorpActionHeader_28|CorpActionHeader_29|CorpActionHeader_30|CorpActionHeader_31|CorpActionHeader_32|CorpActionHeader_33|CorpActionHeader_34|CorpActionHeader_35|CorpActionHeader_36|CorpActionHeader_37|CorpActionHeader_38|CorpActionHeader_39|CorpActionHeader_40|CorpActionHeader_41|CorpActionHeader_42|CorpActionHeader_43|CorpActionHeader_44|CorpActionHeader_45|CorpActionHeader_46|CorpActionHeader_47|CorpActionHeader_48|CorpActionHeader_49|CorpActionHeader_50|CorpActionHeader_51|CorpActionHeader_52|CorpActionHeader_53|CorpActionHeader_54|CorpActionHeader_55|CorpActionHeader_56|CorpActionHeader_57|CorpActionHeader_58|CorpActionHeader_59|CorpActionHeader_60|CorpActionHeader_61|CorpActionHeader_62|CorpActionHeader_63|CorpActionHeader_64|CorpActionHeader_65|CorpActionHeader_66|CorpActionHeader_67|CorpActionHeader_68|CorpActionHeader_69|CorpActionHeader_70|CorpActionHeader_71|CorpActionHeader_72|CorpActionHeader_73|CorpActionHeader_74|CorpActionHeader_75|CorpActionHeader_76|CorpActionHeader_77|CorpActionHeader_78|CorpActionHeader_79|CorpActionHeader_80|CorpActionHeader_81|CorpActionHeader_82|CorpActionHeader_83|CorpActionHeader_84|CorpActionHeader_85|CorpActionHeader_86|CorpActionHeader_87|CorpActionHeader_88|CorpActionHeader_89|CorpActionHeader_90|CorpActionHeader_91|CorpActionHeader_92|CorpActionHeader_93|CorpActionHeader_94|CorpActionHeader_95|CorpActionHeader_96|CorpActionHeader_97|CorpActionHeader_98|CorpActionHeader_99|CorpActionHeader_100|CorpActionHeader_101|CorpActionHeader_102|CorpActionHeader_103|CorpActionHeader_104|CorpActionHeader_105|CorpActionHeader_106|CorpActionHeader_107|CorpActionHeader_108|CorpActionHeader_109|CorpActionHeader_110", dvdCashFinData[0]);
            
            // ensure correct records
            Assert.AreEqual(
                "FD Equity|DVD_CASH|N|FDEquity Name|ISIN|FR12345|EUR|Equity|EQ12345|07/30/2020|01/04/2021|07/30/2020|BBG12345|BBG12345|FP|FP|47|RECORD_DT|01/05/2021|PAY_DT|01/11/2021|FREQ|4|NET_AMT|N.A.|TAX_AMT|N.A.|GROSS_AMT|.66|FRANKED_AMT||DVD_CRNCY|EUR|DVD_TYP|1004|SPPL_AMT||FOREIGN_AMT|N.A.|PAR_PCT|N.A.|STOCK_OPT|S|REINVEST_RATIO|N.A.|PX|N.A.|TAX_RT|N.A.|ADJ|1.000000|ADJ_DT|01/04/2021|INDICATOR|N|DVD_DRP_DISCOUNT|N.A.|EUSD_TID||EUSD_TID_SW||DIST_AMT_STATUS|F|TERMS|N.A.|TKR1|N.A.|RIGHTS_PER_SHARE|N.A.|SHARES_PER_RIGHT|N.A.|SUB_START_DT|N.A.|SUB_END_DT|N.A.|TRADE_START_DT|N.A.|TRADE_END_DT|N.A.|RIGHTS_SHARES|N.A.|RIGHTS_TKR1|N.A.|TKR1_ID_GLOBAL|N.A.|TKR1_ID_GLOBAL_COMPANY|N.A.|TKR1_ID_SEC_NUM_DES|N.A.|TKR1_FEED_SOURCE|N.A.|RIGHTS1_ID_GLOBAL|N.A.|RIGHTS1_ID_GLOBAL_COMPANY|N.A.|RIGHTS1_ID_SEC_NUM_DES|N.A.|RIGHTS1_FEED_SOURCE|N.A.|NOTES|N.A.|ISSUANCE_FEE|N.A.|DEPOSITARY|N.A.|ELECTION_DT|N.A.|ACTION_STATUS|R|INFO_SOURCE|4", 
                dvdCashFinData[1]);
            
            Assert.AreEqual(
                "GD Equity|DVD_CASH|U|Geq Equity|ISIN|BD0001|BDT|Equity|EQ12347|07/15/2020|08/06/2020|08/06/2020|BBG12347|BBG12348|GRAM|BD|47|RECORD_DT|08/05/2020|PAY_DT|N.A.|FREQ|2|NET_AMT|13|TAX_AMT|N.A.|GROSS_AMT|N.A.|FRANKED_AMT||DVD_CRNCY|BDT|DVD_TYP|1003|SPPL_AMT||FOREIGN_AMT|N.A.|PAR_PCT|N.A.|STOCK_OPT|U|REINVEST_RATIO|N.A.|PX|N.A.|TAX_RT|N.A.|ADJ|0.950664|ADJ_DT|08/06/2020|INDICATOR|N|DVD_DRP_DISCOUNT|N.A.|EUSD_TID||EUSD_TID_SW||DIST_AMT_STATUS|F|TERMS|N.A.|TKR1|N.A.|RIGHTS_PER_SHARE|N.A.|SHARES_PER_RIGHT|N.A.|SUB_START_DT|N.A.|SUB_END_DT|N.A.|TRADE_START_DT|N.A.|TRADE_END_DT|N.A.|RIGHTS_SHARES|N.A.|RIGHTS_TKR1|N.A.|TKR1_ID_GLOBAL|N.A.|TKR1_ID_GLOBAL_COMPANY|N.A.|TKR1_ID_SEC_NUM_DES|N.A.|TKR1_FEED_SOURCE|N.A.|RIGHTS1_ID_GLOBAL|N.A.|RIGHTS1_ID_GLOBAL_COMPANY|N.A.|RIGHTS1_ID_SEC_NUM_DES|N.A.|RIGHTS1_FEED_SOURCE|N.A.|NOTES|N.A.|ISSUANCE_FEE|N.A.|DEPOSITARY|N.A.|ELECTION_DT|N.A.|ACTION_STATUS|R|INFO_SOURCE|10", 
                dvdCashFinData[2]);
            
            Assert.AreEqual(
                "ABC US Equity|DVD_CASH|N|ABC Name|CUSIP|123456789|USD|Equity|EQ0010000000000000|07/30/2020|08/07/2020|07/30/2020|BBG0002|BBG001000000|AAPL|US|47|RECORD_DT|08/10/2020|PAY_DT|08/13/2020|FREQ|4|NET_AMT|N.A.|TAX_AMT||GROSS_AMT|.82|FRANKED_AMT||DVD_CRNCY|USD|DVD_TYP|1000|SPPL_AMT||FOREIGN_AMT||PAR_PCT||STOCK_OPT|N|REINVEST_RATIO||PX||TAX_RT||ADJ|1.000000|ADJ_DT|08/07/2020|INDICATOR|N|DVD_DRP_DISCOUNT|N.A.|EUSD_TID||EUSD_TID_SW||DIST_AMT_STATUS|F|TERMS|N.A.|TKR1|N.A.|RIGHTS_PER_SHARE|N.A.|SHARES_PER_RIGHT|N.A.|SUB_START_DT|N.A.|SUB_END_DT|N.A.|TRADE_START_DT|N.A.|TRADE_END_DT|N.A.|RIGHTS_SHARES|N.A.|RIGHTS_TKR1|N.A.|TKR1_ID_GLOBAL|N.A.|TKR1_ID_GLOBAL_COMPANY|N.A.|TKR1_ID_SEC_NUM_DES|N.A.|TKR1_FEED_SOURCE|N.A.|RIGHTS1_ID_GLOBAL|N.A.|RIGHTS1_ID_GLOBAL_COMPANY|N.A.|RIGHTS1_ID_SEC_NUM_DES|N.A.|RIGHTS1_FEED_SOURCE|N.A.|NOTES|N.A.|ISSUANCE_FEE|N.A.|DEPOSITARY|N.A.|ELECTION_DT|N.A.|ACTION_STATUS|R|INFO_SOURCE|15", 
                dvdCashFinData[3]);
            
            Assert.AreEqual(
                "PT US EQ|DVD_CASH|U|PT EQ Name Ltd|CUSIP|13589|USD|Equity|EQ0010000000000002|06/15/2020|06/19/2020|08/03/2020|BBG0004|BBG0003ABC|PTR|US|47|RECORD_DT|06/22/2020|PAY_DT|08/10/2020|FREQ|N.A.|NET_AMT|.299477|TAX_AMT|0.033275|GROSS_AMT|.332752|FRANKED_AMT||DVD_CRNCY|USD|DVD_TYP|1001|SPPL_AMT||FOREIGN_AMT|N.A.|PAR_PCT|N.A.|STOCK_OPT|U|REINVEST_RATIO|N.A.|PX|N.A.|TAX_RT|10.000000|ADJ|0.991518|ADJ_DT|06/19/2020|INDICATOR|N|DVD_DRP_DISCOUNT|N.A.|EUSD_TID||EUSD_TID_SW||DIST_AMT_STATUS|F|TERMS|N.A.|TKR1|N.A.|RIGHTS_PER_SHARE|N.A.|SHARES_PER_RIGHT|N.A.|SUB_START_DT|N.A.|SUB_END_DT|N.A.|TRADE_START_DT|N.A.|TRADE_END_DT|N.A.|RIGHTS_SHARES|N.A.|RIGHTS_TKR1|N.A.|TKR1_ID_GLOBAL|N.A.|TKR1_ID_GLOBAL_COMPANY|N.A.|TKR1_ID_SEC_NUM_DES|N.A.|TKR1_FEED_SOURCE|N.A.|RIGHTS1_ID_GLOBAL|N.A.|RIGHTS1_ID_GLOBAL_COMPANY|N.A.|RIGHTS1_ID_SEC_NUM_DES|N.A.|RIGHTS1_FEED_SOURCE|N.A.|NOTES|N.A.|ISSUANCE_FEE|N.A.|DEPOSITARY|100164|ELECTION_DT|N.A.|ACTION_STATUS|R|INFO_SOURCE|20",
                dvdCashFinData[6]);
            
            
            // check contents of stock split cash as expected
            string[] stockSplitFinData = File.ReadAllLines(stockSplitCorpActionFile);
            
            // ensure all records have been returned
            Assert.That(stockSplitFinData, Has.Exactly(4).Items);
            
            // ensure correct headers constructed correctly
            Assert.AreEqual("Ticker|CorpActionHeader_1|CorpActionHeader_2|CorpActionHeader_3|CorpActionHeader_4|CorpActionHeader_5|CorpActionHeader_6|CorpActionHeader_7|CorpActionHeader_8|CorpActionHeader_9|CorpActionHeader_10|CorpActionHeader_11|CorpActionHeader_12|CorpActionHeader_13|CorpActionHeader_14|CorpActionHeader_15|CorpActionHeader_16|CorpActionHeader_17|CorpActionHeader_18", stockSplitFinData[0]);
            
            // ensure correct records
            Assert.AreEqual(
                "ABC US Equity|STOCK_SPLT|D|ABC Name|CUSIP|123456789|USD|Equity|EQ0010000000000000|07/30/2020|08/31/2020|07/31/2020|BBG0002|BBG001000000|AAPL|US|1|DELETE_REASON|2",
                stockSplitFinData[1]);
            
            Assert.AreEqual(
                "ABC US Equity|STOCK_SPLT|D|ABC Name|CUSIP|123456789|USD|Equity|EQ0010000000000000|07/30/2020|08/31/2020|07/31/2020|BBG0002|BBG001000000|AAPL|US|1|DELETE_REASON|2",
                stockSplitFinData[2]);
            
            Assert.AreEqual(
                "ABC US Equity|STOCK_SPLT|U|ABC Name|CUSIP|123456789|USD|Equity|EQ0010000000000000|07/30/2020|08/31/2020|07/31/2020|BBG0002|BBG001000000|AAPL|US|16|TERMS|4 for 1|RATIO|4.000000|RECORD_DT|08/24/2020|PAY_DT|08/28/2020|STOCK_SPLT_TYP|3000|ADJ|4.000000|ADJ_DT|08/31/2020|SH_FRACTIONAL|N.A.|INDICATOR|N|NOTES|N.A.|ISSUANCE_FEE||DEPOSITARY||DIST_AMT_STATUS|N.A.|DUE_BILL_RED_DT|N.A.|ACTION_STATUS|R|INFO_SOURCE|15",
                stockSplitFinData[3]);

        }

        [Test]
        public void Process_OnUnsupportedVendor_ShouldThrowDataException()
        {
            //when
            string unsupported_vendor_fde_request = "Vendor\\Dl\\TestData\\unsupported_vendor_request_file.json";
            
            //execute
            try
            {
                _finDataEx.Process(unsupported_vendor_fde_request);
                Assert.Fail("Should have thrown an invalid data exception due to unknown vendor");
            }
            catch (InvalidDataException e) {}
        }
        
        
    }
}