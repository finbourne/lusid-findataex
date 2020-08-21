using System;
using System.Collections.Generic;
using Lusid.FinDataEx.Core;
using Lusid.FinDataEx.Vendor;
using Lusid.FinDataEx.Vendor.Dl;
using Lusid.FinDataEx.Vendor.Dl.Ftp;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Vendor.Dl.Ftp
{
    [TestFixture]
    public class DlFtpExtractorTests
    {
        private DlFtpExtractor _dlFtpExtractor;
        private IVendorClient<DlFtpRequest, DlFtpResponse> _vendorClient;
        
        [Test]
        public void Extract_OnValidDLPriceOnlyRequest_ShouldReturnVendorResponseWithPrices()
        {
            // when
            _vendorClient = new DlFileSystemClient(new DlFtpResponseBuilder());
            _dlFtpExtractor = new DlFtpExtractor(_vendorClient);
            FdeRequest dlPricesFdeRequest = LoadValidDLPriceOnlyRequest();
            
            // execute
            IVendorResponse dlPricesFdeResponse = _dlFtpExtractor.Extract(dlPricesFdeRequest);

            //verify
            Dictionary<string, List<List<string>>> finDataMap = dlPricesFdeResponse.GetFinData();
            List<List<string>> finData = finDataMap.GetValueOrDefault(DlRequestType.Prices.ToString(), new List<List<string>>());
            
            // ensure all records have been returned
            Assert.That(finData, Has.Exactly(10).Items);
            
            // ensure correct headers constructed
            CollectionAssert.AreEqual(
                new List<string>(){"Ticker", "PX_ASK", "PX_LAST", "NoMapping"}, 
                finData[0]);
            
            // ensure correct records
            CollectionAssert.AreEqual(
                new List<string>(){"BRIGHTI Equity                  ", "6.174400", "6.174400", "FLD UNKNOWN"}, 
                finData[2]);
            
            CollectionAssert.AreEqual(
                new List<string>(){"CNTRY_1  11/08/18 Govt          ", "N.A.", " ", "FLD UNKNOWN"}, 
                finData[4]);
            
        }
        
        [Test]
        public void Extract_OnValidDLAllFieldsRequest_ShouldReturnVendorResponseWithAllFields()
        {
            // when
            _vendorClient = new DlFileSystemClient(new DlFtpResponseBuilder());
            _dlFtpExtractor = new DlFtpExtractor(_vendorClient);
            FdeRequest dlPricesFdeRequest = LoadValidDLAllFieldsRequest();
            
            // execute
            IVendorResponse dlPricesFdeResponse = _dlFtpExtractor.Extract(dlPricesFdeRequest);

            //verify
            Dictionary<string, List<List<string>>> finDataMap = dlPricesFdeResponse.GetFinData();
            List<List<string>> finData = finDataMap.GetValueOrDefault(DlRequestType.Prices.ToString(), new List<List<string>>());
            // ensure all records have been returned
            Assert.That(finData, Has.Exactly(14).Items);


            // ensure correct headers constructed
            CollectionAssert.AreEqual(
                new List<string>(){"Ticker","PX_LAST","PX_BID","PX_ASK","LAST_UPDATE","SECURITY_DES","EXCH_CODE","MARKET_SECTOR_DES","TICKER","FWD_RT_1WK","FWD_RT_1MO","FWD_RT_2MO","FWD_RT_3MO","FWD_RT_4MO","FWD_RT_5MO","FWD_RT_6MO","FWD_RT_7MO","FWD_RT_8MO","FWD_RT_9MO","FWD_RT_10MO","FWD_RT_11MO","FWD_RT_12MO","FWD_RT_13MO","FWD_RT_14MO","FWD_RT_15MO","FWD_RT_16MO","FWD_RT_17MO","FWD_RT_18MO","FWD_RT_19MO","FWD_RT_20MO","FWD_RT_21MO","FWD_RT_22MO","FWD_RT_23MO","FWD_RT_24MO","FWD_RT_3YR","FWD_RT_4YR","FWD_RT_5YR","PX_VOLUME","VOLUME_AVG_3M","EQY_LST_DVD/SHS","EQY_DVD_EX_DT","EQY_DVD_RECORD_DT","EQY_DVD_PAY_DT","EQY_DVD_DECLARED_DT","EQY_WEIGHTED_AVG_PX","EQY_BETA","GILTS_EX_DVD_DT","AMT_ISSUED","CPN","NXT_CALL_DT","NXT_CALL_PX","NXT_PUT_DT","NXT_PUT_PX","CUR_FACTOR","IDX_RATIO","SW_PREMIUM"}, 
                finData[0]);
            

            // ensure correct records
            CollectionAssert.AreEqual(
                new List<string>(){"AUD Curncy                      ",".712400",".712300",".712500","22:16:27","AUD Curncy"," ","Curncy","AUD",".712412",".712480",".712540",".712580",".712607",".712625",".712645",".712632",".712619",".712606",".712543",".712487",".712426",".712346",".712273",".712190",".712086",".711970",".711851",".711759",".711660",".711560",".711458",".711359",".711250",".709403",".707296",".704650","N.A.","N.S."," "," "," "," "," ","N.S.","N.S."," "," "," "," "," "," "," ","N.S."," ","N.S."}, 
                finData[2]);
            
            
            CollectionAssert.AreEqual(
                new List<string>(){"TZ 1.5 09/19/39 Corp            ","104.086000","103.442000","104.729000","21:56:34","VZ 1 1/2 09/19/39","MUNICH","Corp","VZ"," "," "," "," "," "," "," "," "," "," "," "," "," "," "," "," "," "," "," "," "," "," "," "," "," "," "," "," "," ","N.S."," "," "," "," "," ","N.S.","N.S."," ","500000000.00","1.500000","03/19/2039","100.000000"," "," ","N.S."," ","N.S."},
                    finData[9]);
            
        }

        private FdeRequest LoadValidDLPriceOnlyRequest()
        {
            FdeRequestBuilder fdeRequestBuilder = new FdeRequestBuilder();
            return fdeRequestBuilder.LoadFromFile("Vendor\\Dl\\TestData\\fde_request_dl_prices_file.json");
        }
        
        private FdeRequest LoadValidDLAllFieldsRequest()
        {
            FdeRequestBuilder fdeRequestBuilder = new FdeRequestBuilder();
            return fdeRequestBuilder.LoadFromFile("Vendor\\Dl\\TestData\\fde_request_dl_all_file.json");
        }
        
    }
}