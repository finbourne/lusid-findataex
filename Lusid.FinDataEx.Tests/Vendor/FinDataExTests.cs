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