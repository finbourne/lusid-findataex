using System.Collections.Generic;
using System.IO;
using Lusid.FinDataEx.Vendor.Dl;
using Lusid.FinDataEx.Vendor.Dl.Ftp;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Vendor.Dl.Ftp
{
    public class DlFtpResponseBuilderTest
    {

        private DlFtpResponseBuilder _dlFtpResponseBuilder;

        [SetUp]
        public void SetUp()
        {
            _dlFtpResponseBuilder = new DlFtpResponseBuilder();
        }
        
        [Test]
        public void ToVendorResponse_OnValidDLResponseWithPriceOnly_ShouldReturnResponse()
        {
            var dlResponseWithIncorrectColsOnData = new[]
            {
                "START-OF-FILE",
                "RUNDATE=20200813",
                "REPLYFILENAME=DL12345.out",
                "",
                "START-OF-FIELDS",
                "PX_ASK",
                "PX_LAST",
                "NoMapping",
                "",
                "START-OF-DATA",
                "ABC Index|0|3|100.10|100.10|FLD UNKNOWN|",
                "CTY_10YR Index|0|3|.650|.670|FLD UNKNOWN|"
            };

            var dlFtpResponse =  _dlFtpResponseBuilder.CreatePricesResponse(dlResponseWithIncorrectColsOnData);
            var finDataMap = dlFtpResponse.GetFinData();
            var finData = finDataMap.GetValueOrDefault(DlRequestType.Prices.ToString(), new List<List<string>>());

            // ensure all records have been returned
            Assert.That(finData, Has.Exactly(3).Items);
            CollectionAssert.AreEqual(
                new List<string>(){"Ticker", "PX_ASK", "PX_LAST", "NoMapping"}, 
                finData[0]);
            // ensure correct records
            CollectionAssert.AreEqual(
                new List<string>(){"ABC Index", "100.10", "100.10", "FLD UNKNOWN"}, 
                finData[1]);
            
            CollectionAssert.AreEqual(
                new List<string>(){"CTY_10YR Index", ".650", ".670", "FLD UNKNOWN"}, 
                finData[2]);
        }
        
        [Test]
        public void ToVendorResponse_OnPriceResponseWithDataComments_ShouldIgnoreCommentsAndReturnResponse()
        {
            string[] dlResponseWithIncorrectColsOnData = new string[]
            {
                "START-OF-FILE",
                "RUNDATE=20200813",
                "REPLYFILENAME=DL12345.out",
                "",
                "START-OF-FIELDS",
                "PX_ASK",
                "PX_LAST",
                "NoMapping",
                "",
                "START-OF-DATA",
                "# some comment related to the data returned",
                "## some other comments related to data returned", 
                "ABC Index|0|3|100.10|100.10|FLD UNKNOWN|",
                "CTY_10YR Index|0|3|.650|.670|FLD UNKNOWN|"
            };

            DlFtpResponse dlFtpResponse =  _dlFtpResponseBuilder.CreatePricesResponse(dlResponseWithIncorrectColsOnData);
            Dictionary<string, List<List<string>>> finDataMap = dlFtpResponse.GetFinData();
            List<List<string>> finData = finDataMap.GetValueOrDefault(DlRequestType.Prices.ToString(), new List<List<string>>());

            // ensure all records have been returned
            Assert.That(finData, Has.Exactly(3).Items);
            CollectionAssert.AreEqual(
                new List<string>(){"Ticker", "PX_ASK", "PX_LAST", "NoMapping"}, 
                finData[0]);
            // ensure correct records
            CollectionAssert.AreEqual(
                new List<string>(){"ABC Index", "100.10", "100.10", "FLD UNKNOWN"}, 
                finData[1]);
            
            CollectionAssert.AreEqual(
                new List<string>(){"CTY_10YR Index", ".650", ".670", "FLD UNKNOWN"}, 
                finData[2]);
        }
        
        [Test]
        public void ToVendorResponse_OnResponseNoFieldMappingDataForCorporateAction_ShouldReturnResponse()
        {
            // This usecase exists for the Corporate Actions extract from DL.

            var dlCorpActionResponse = new[]
            {
                "START-OF-FILE",
                "RUNDATE=20200813",
                "ACTIONS=STOCK_SPLT | DVD_STOCK | DVD_CASH",
                "REPLYFILENAME=DL12345.out",
                "START-OF-DATA",
                "EQ 1|Ticker|12345|1000|0|DVD_CASH|Meta1|Meta2|Meta3|",
                "EQ 2|Ticker|2500|4000|0|DVD_STOCK|Meta1|Meta2|Meta3|Meta4|Meta5|",
                "EQ 2|Ticker|2500|1000|0|DVD_CASH|Meta3|Meta4|Meta5|",
                "EQ 3|Ticker|3000|1000|0|",
                "EQ 1|Ticker|12345|4000|0|DVD_STOCK|Meta6|Meta7|Meta8|Meta9|Meta10|",
                "EQ 3|Ticker|3000|1000|0|"
            };
            
            var dlFtpResponse =  _dlFtpResponseBuilder.CreateCorpActionsResponse(dlCorpActionResponse);
            var finDataMap = dlFtpResponse.GetFinData();
            
            // Verify only two corp actions produced and all requests with no corporate actions ignored
            Assert.That(finDataMap, Has.Exactly(2).Items);
            
            // Verify specific corp actions responses for each corp action type
            var dvdCashFinData = finDataMap.GetValueOrDefault("DVD_CASH", new List<List<string>>());
            var dvdStockFinData = finDataMap.GetValueOrDefault("DVD_STOCK", new List<List<string>>());

            // Verify Dvd Cash Entries
            Assert.That(dvdCashFinData, Has.Exactly(3).Items);
            CollectionAssert.AreEqual(
                new List<string>(){"Ticker", "CorpActionHeader_1", "CorpActionHeader_2", "CorpActionHeader_3", "CorpActionHeader_4"}, 
                dvdCashFinData[0]);
            // ensure correct records
            CollectionAssert.AreEqual(
                new List<string>(){"EQ 1", "DVD_CASH", "Meta1", "Meta2", "Meta3"}, 
                dvdCashFinData[1]);
            CollectionAssert.AreEqual(
                new List<string>(){"EQ 2", "DVD_CASH", "Meta3", "Meta4", "Meta5"}, 
                dvdCashFinData[2]);
            
            //Verify Dvd Stock Entries
            Assert.That(dvdStockFinData, Has.Exactly(3).Items);
            CollectionAssert.AreEqual(
                new List<string>(){"Ticker", "CorpActionHeader_1", "CorpActionHeader_2", "CorpActionHeader_3", "CorpActionHeader_4", "CorpActionHeader_5", "CorpActionHeader_6"}, 
                dvdStockFinData[0]);
            // ensure correct records
            CollectionAssert.AreEqual(
                new List<string>(){"EQ 2", "DVD_STOCK", "Meta1", "Meta2", "Meta3", "Meta4", "Meta5"}, 
                dvdStockFinData[1]);
            CollectionAssert.AreEqual(
                new List<string>(){"EQ 1", "DVD_STOCK", "Meta6", "Meta7", "Meta8", "Meta9", "Meta10"}, 
                dvdStockFinData[2]);
        }
        
         [Test]
        public void ToVendorResponse_OnCorpActionsResponseWithDataComments_ShouldIgnoreCommentsAndReturnResponse()
        {
            // This usecase exists for the Corporate Actions extract from DL.

            var dlCorpActionResponse = new[]
            {
                "START-OF-FILE",
                "RUNDATE=20200813",
                "ACTIONS=STOCK_SPLT | DVD_STOCK | DVD_CASH",
                "REPLYFILENAME=DL12345.out",
                "START-OF-DATA",
                // these comments should be ignored in data returned
                "# Returning corporate actions entered between 00:00 EDT on 20200729 and 23:59 EDT on 2020080",
                "# Returning corporate actions effective between 20200729 and 20200806",
                "EQ 1|Ticker|12345|1000|0|DVD_CASH|Meta1|Meta2|Meta3|",
                "EQ 2|Ticker|2500|4000|0|DVD_STOCK|Meta1|Meta2|Meta3|Meta4|Meta5|",
                "EQ 2|Ticker|2500|1000|0|DVD_CASH|Meta3|Meta4|Meta5|",
                "EQ 3|Ticker|3000|1000|0|",
                "EQ 1|Ticker|12345|4000|0|DVD_STOCK|Meta6|Meta7|Meta8|Meta9|Meta10|",
                "EQ 3|Ticker|3000|1000|0|"
            };
            
            var dlFtpResponse =  _dlFtpResponseBuilder.CreateCorpActionsResponse(dlCorpActionResponse);
            var finDataMap = dlFtpResponse.GetFinData();
            
            // Verify only two corp actions produced and all requests with no corporate actions ignored
            Assert.That(finDataMap, Has.Exactly(2).Items);
            
            // Verify specific corp actions responses for each corp action type
            var dvdCashFinData = finDataMap.GetValueOrDefault("DVD_CASH", new List<List<string>>());
            var dvdStockFinData = finDataMap.GetValueOrDefault("DVD_STOCK", new List<List<string>>());

            // Verify Dvd Cash Entries
            Assert.That(dvdCashFinData, Has.Exactly(3).Items);
            CollectionAssert.AreEqual(
                new List<string>(){"Ticker", "CorpActionHeader_1", "CorpActionHeader_2", "CorpActionHeader_3", "CorpActionHeader_4"}, 
                dvdCashFinData[0]);
            // ensure correct records
            CollectionAssert.AreEqual(
                new List<string>(){"EQ 1", "DVD_CASH", "Meta1", "Meta2", "Meta3"}, 
                dvdCashFinData[1]);
            CollectionAssert.AreEqual(
                new List<string>(){"EQ 2", "DVD_CASH", "Meta3", "Meta4", "Meta5"}, 
                dvdCashFinData[2]);
            
            //Verify Dvd Stock Entries
            Assert.That(dvdStockFinData, Has.Exactly(3).Items);
            CollectionAssert.AreEqual(
                new List<string>(){"Ticker", "CorpActionHeader_1", "CorpActionHeader_2", "CorpActionHeader_3", "CorpActionHeader_4", "CorpActionHeader_5", "CorpActionHeader_6"}, 
                dvdStockFinData[0]);
            // ensure correct records
            CollectionAssert.AreEqual(
                new List<string>(){"EQ 2", "DVD_STOCK", "Meta1", "Meta2", "Meta3", "Meta4", "Meta5"}, 
                dvdStockFinData[1]);
            CollectionAssert.AreEqual(
                new List<string>(){"EQ 1", "DVD_STOCK", "Meta6", "Meta7", "Meta8", "Meta9", "Meta10"}, 
                dvdStockFinData[2]);
        }
        
        [Test]
        public void ToVendorResponse_OnResponseNoFieldMappingDataForNonCorporateAction_ShouldThrowException()
        {
            // Missing fields parameters for non corporate actions requests are unexpected or incorrect.

            var dlResponseWithNoFieldsForNonCorpAction = new[]
            {
                "START-OF-FILE",
                "RUNDATE=20200813",
                "REPLYFILENAME=DL12345.out",
                "START-OF-DATA",
                "HDB US Equity|Ticker|",
                "DSHNBWY_OTC_ Equity|Ticker|"
            };
            
            Assert.Throws<InvalidDataException>(() => _dlFtpResponseBuilder.CreatePricesResponse(dlResponseWithNoFieldsForNonCorpAction),
                "Exception should have been thrown due to missing fields in the DL response which is only" +
                "supported for corporate actions");
        }


        [Test]
        public void ToVendorResponse_OnResponseWithTooFewColumnsInResponse_ShouldThrowDataException()
        {
            var dlResponseWithTooFewColsOnData = new[]
            {
                "START-OF-FILE",
                "RUNDATE=20200813",
                "REPLYFILENAME=DL12345.out",
                "",
                "START-OF-FIELDS",
                "PX_ASK",
                "PX_LAST",
                "NoMapping",
                "",
                "START-OF-DATA",
                "ABC Index|0|3|",
                "CTY_10YR Index|0|3|"
            };

            Assert.Throws<InvalidDataException>(() => _dlFtpResponseBuilder.CreatePricesResponse(dlResponseWithTooFewColsOnData),
                "Exception should have been thrown due to too few columns");
        }
        
        [Test]
        public void ToVendorResponse_OnResponseWithTooManyColumnsInResponse_ShouldThrowDataException()
        {
            var dlResponseWithTooManyColsOnData = new string[]
            {
                "START-OF-FILE",
                "RUNDATE=20200813",
                "REPLYFILENAME=DL12345.out",
                "",
                "START-OF-FIELDS",
                "PX_ASK",
                "PX_LAST",
                "NoMapping",
                "",
                "START-OF-DATA",
                "ABC Index|0|3|N.A.|1234.220000|FLD UNKNOWN|ExtraCol|ExtraCol2|"
            };
            
            Assert.Throws<InvalidDataException>(() => _dlFtpResponseBuilder.CreatePricesResponse(dlResponseWithTooManyColsOnData),
                "Exception should have been thrown due to too many columns");
        }
        
        
    }
}