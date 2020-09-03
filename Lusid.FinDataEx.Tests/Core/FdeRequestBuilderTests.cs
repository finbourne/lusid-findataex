using System.IO;
using Lusid.Drive.Sdk.Client;
using Lusid.FinDataEx.Core;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Core
{
    
    
    [TestFixture]
    public class FdeRequestBuilderTests
    {
        public const string CiTestFdePricesRequestLusidDriveId = "ad55ada0-598d-4cc8-9e04-38462580219a";

        private FdeRequestBuilder _fdeRequestBuilder;
        
        [SetUp]
        public void SetUp()
        {
            _fdeRequestBuilder = new FdeRequestBuilder();
        }

        [Test]
        public void LoadFromFileSystem_OnExistingFdeRequest_ShouldLoadRequest()
        {
            FdeRequest fdeRequest = _fdeRequestBuilder.LoadFromFile(Path.Combine(new[]{"Vendor","Dl","TestData","fde_request_dl_prices_file.json"}));
            Assert.That(fdeRequest.Uid, Is.EqualTo("DL12345"));
            Assert.That(fdeRequest.CallerId, Is.EqualTo("client_scheduler_uat_01"));
            Assert.That(fdeRequest.Vendor, Is.EqualTo("DL"));
            Assert.That(fdeRequest.Output, Is.EqualTo("lusidtools"));
            Assert.That(fdeRequest.ConnectorConfig["url"], Is.EqualTo("sftp://dl_ftp.com:22"));
            Assert.That(fdeRequest.RequestBody["sourceData"], Is.EqualTo("Vendor/Dl/TestData/DlRequests/DL12345.req"));

        }
        
        [Test]
        public void LoadFromLusidDrive_OnExistingFdeRequestOnDrive_ShouldLoadRequest()
        {
            FdeRequest fdeRequest = _fdeRequestBuilder.LoadFromLusidDrive(CiTestFdePricesRequestLusidDriveId);
            Assert.That(fdeRequest.Uid, Is.EqualTo("DL12345"));
            Assert.That(fdeRequest.CallerId, Is.EqualTo("client_scheduler_uat_01"));
            Assert.That(fdeRequest.Vendor, Is.EqualTo("DL"));
            Assert.That(fdeRequest.Output, Is.EqualTo("lusiddrive"));
            Assert.That(fdeRequest.ConnectorConfig["url"], Is.EqualTo("lusiddrive://52ff9d39-ace8-4873-95dc-d288feda3011"));
            Assert.That(fdeRequest.RequestBody["sourceData"], Is.EqualTo("Vendor/Dl/TestData/DlRequests/DL12345.req"));

        }
        
        [Test]
        public void LoadFromLusidDrive_OnNonExistingFdeRequestOnDrive_ShouldLoadRequest()
        {
            Assert.Throws<ApiException>(() => _fdeRequestBuilder.LoadFromLusidDrive("this_id_does_not_exist_012345"));
        }
    }
}