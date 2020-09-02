using System;
using Lusid.FinDataEx.Core;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Core
{
    
    
    [TestFixture]
    public class FdeRequestBuilderTests
    {
        private const string CiTestFdePricesRequestLusidDriveId = "e2dd4c7d-edcb-4af2-8c02-6ee61308c6dd";

        private FdeRequestBuilder _fdeRequestBuilder;
        
        [SetUp]
        public void setUp()
        {
            _fdeRequestBuilder = new FdeRequestBuilder();
        }

        [Test]
        public void LoadFromLusidDrive_OnExistingFdeRequestOnDrive_ShouldLoadRequest()
        {
            FdeRequest fdeRequest = _fdeRequestBuilder.LoadFromLusidDrive(CiTestFdePricesRequestLusidDriveId);
            Console.WriteLine(fdeRequest);
        }
        
        [Test]
        public void LoadFromLusidDrive_OnNonExistingFdeRequestOnDrive_ShouldLoadRequest()
        {
            FdeRequest fdeRequest = _fdeRequestBuilder.LoadFromLusidDrive("this_id_does_not_exist_012345");
            Console.WriteLine(fdeRequest);
        }
    }
}