using System.IO;
using NUnit.Framework;
using static Lusid.FinDataEx.Tests.Vendor.Util.TestUtils;

namespace Lusid.FinDataEx.Tests.Vendor
{
    [TestFixture]
    public class FinDataExRuntimeTests
    {
        private readonly string _tempOutputDir = $"TempTestDir_{nameof(FinDataExRuntimeTests)}";

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
        public void run_OnValidRequestWithLusidToolsOutput_ShouldProcessAndWriteToFile()
        {
            //when
            var fdeValidRequest = Path.Combine(new[]{"Vendor","Dl","TestData","fde_request_dl_prices_file.json"});
            
            //execute
            FinDataExRuntime.Main(new string[]{fdeValidRequest, _tempOutputDir});
            
            // verify
            Assert.That(_tempOutputDir + Path.DirectorySeparatorChar + "DL12345_Prices.csv", Does.Exist);
        }
        
        [Test]
        public void run_OnValidCorpActionsRequestWithLusidToolsOutput_ShouldProcessAndWriteToFile()
        {
            //when
            var fdeValidRequest = Path.Combine(new[]{"Vendor","Dl","TestData","fde_request_dl_corpactions_file.json"});
            
            //execute
            FinDataExRuntime.Main(new string[]{fdeValidRequest, _tempOutputDir});
            
            // verify
            Assert.That(_tempOutputDir + Path.DirectorySeparatorChar + "DL30001_DVD_CASH.csv", Does.Exist);
            Assert.That(_tempOutputDir + Path.DirectorySeparatorChar + "DL30001_STOCK_SPLT.csv", Does.Exist);
        }
        
        [Test]
        public void run_OnUnsupportedVendorRequest_ShouldNotProduceFileAndShouldThrowException()
        {
            //when
            var unsupportedVendorFdeRequest = Path.Combine(new[]{"Vendor","Dl","TestData","unsupported_vendor_request_file.json"});
            
            //execute
            
            Assert.Throws<InvalidDataException>(() => FinDataExRuntime.Main(new string[]{unsupportedVendorFdeRequest, _tempOutputDir}),
                "Should have thrown an invalid data exception due to unknown vendor");
            
            // verify
            Assert.That(_tempOutputDir + Path.DirectorySeparatorChar + "VendorABC_123456_Prices.csv", Does.Not.Exist);
        }

    }
}