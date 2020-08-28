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
        private Mock<FdeBaseExtractor<IVendorRequest, IVendorResponse>> _fdeBaseExtractorMock;
        private readonly IVendorRequest _vendorRequest = Mock.Of<IVendorRequest>();
        private readonly IVendorResponse _vendorResponse = Mock.Of<IVendorResponse>();
        private readonly FdeRequest _fdeRequest = Mock.Of<FdeRequest>();

        [SetUp]
        public void SetUp()
        {
            _vendorClient = Mock.Of<IVendorClient<IVendorRequest, IVendorResponse>>();
            Mock.Get(_vendorClient).Setup(m => m.Submit(_vendorRequest)).Returns(_vendorResponse);
    
            _fdeBaseExtractorMock =
                new Mock<FdeBaseExtractor<IVendorRequest, IVendorResponse>>(_vendorClient) {CallBase = true};
            _fdeBaseExtractorMock.Setup(m => m.ToVendorRequest(_fdeRequest)).Returns(_vendorRequest);

            _fdeBaseExtractor = _fdeBaseExtractorMock.Object;
        }

        [Test]
        public void Extract_OnFdeRequest_ShouldCallVendorClientAndReturnResponse()
        {
            IVendorResponse response = _fdeBaseExtractor.Extract(_fdeRequest);
            
            Assert.That(_vendorResponse, Is.EqualTo(response));
            _fdeBaseExtractorMock.Verify(m => m.ToVendorRequest(_fdeRequest), Times.Once);
            Mock.Get(_vendorClient).Verify(m => m.Submit(_vendorRequest), Times.Once);
        }
        
        
    }
}