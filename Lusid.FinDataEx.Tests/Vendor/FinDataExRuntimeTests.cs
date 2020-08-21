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
            string fde_valid_request = "Vendor\\Dl\\TestData\\fde_request_dl_prices_file.json";
            
            //execute
            FinDataExRuntime.Main(new string[]{fde_valid_request, _tempOutputDir});
            
            // verify
            Assert.That(_tempOutputDir + Path.DirectorySeparatorChar + "DL12345_Prices.csv", Does.Exist);
        }
        
        [Test]
        public void run_OnUnsupportedVendorRequest_ShouldNotProduceFileAndShouldThrowException()
        {
            //when
            string unsupported_vendor_fde_request = "Vendor\\Dl\\TestData\\unsupported_vendor_request_file.json";
            
            //execute
            try
            {
                FinDataExRuntime.Main(new string[]{unsupported_vendor_fde_request, _tempOutputDir});
                Assert.Fail("Should have thrown an invalid data exception due to unknown vendor");
            }
            catch (InvalidDataException e) {}
            
            // verify
            Assert.That(_tempOutputDir + Path.DirectorySeparatorChar + "VendorABC_123456_Prices.csv", Does.Not.Exist);
        }

    }
}