using Lusid.FinDataEx.Core;
using Lusid.FinDataEx.Vendor;
using Moq;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Core
{
    [TestFixture]
    public class FdeBaseExtractorTest
    {
        private FdeBaseExtractor<IVendorRequest, IVendorResponse> _fdeBaseExtractor;
        private IVendorClient<IVendorRequest, IVendorResponse> _vendorClient;
        private IVendorRequest _vendorRequest = Mock.Of<IVendorRequest>();
        private IVendorResponse _vendorResponse = Mock.Of<IVendorResponse>();
        private FdeRequest _fdeRequest = Mock.Of<FdeRequest>();
        private Mock<FdeBaseExtractor<IVendorRequest, IVendorResponse>> fdeBaseExtractorMock;
        
        [SetUp]
        public void SetUp()
        {
            _vendorClient = Mock.Of<IVendorClient<IVendorRequest, IVendorResponse>>();
            Mock.Get(_vendorClient).Setup(m => m.Submit(_vendorRequest)).Returns(_vendorResponse);
            
            fdeBaseExtractorMock =new Mock<FdeBaseExtractor<IVendorRequest, IVendorResponse>>();
            fdeBaseExtractorMock.CallBase = true;
            fdeBaseExtractorMock.Setup(m => m.ToVendorRequest(_fdeRequest)).Returns(_vendorRequest);

            _fdeBaseExtractor = fdeBaseExtractorMock.Object;
        }

        [Test]
        public void Extract_OnFdeRequest_ShouldCallVendorClientAndReturnResponse()
        {
            IVendorResponse response = _fdeBaseExtractor.Extract(_fdeRequest);
            
            Assert.AreEqual(_vendorResponse, response);
            fdeBaseExtractorMock.Verify(m => m.ToVendorRequest(_fdeRequest), Times.Once);
            Mock.Get(_vendorClient).Verify(m => m.Submit(_vendorRequest), Times.Once);
        }
        
        
    }
}