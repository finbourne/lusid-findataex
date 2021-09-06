using System.Collections.Generic;
using Lusid.FinDataEx.DataLicense.Service.Transform;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Unit.DataLicense.Service.Transform
{
    [TestFixture]
    public class DataResponseTransformerTest
    {
        private GetDataResponseTransformer _transformer;

        [SetUp]
        public void SetUp()
        {
            _transformer = new GetDataResponseTransformer();
        }

        [Test]
        public void Transform_OnAllValidInstrumentsResponse_IsWellFormed()
        {
            //when
            var responseId = "1602149495-71386027_ValidInstruments";
            var retrieveGetDataResponse = TestUtils.LoadGetDataResponseFromFile(responseId);
            
            //execute
            var getDataOutput = _transformer.Transform(retrieveGetDataResponse);
            
            //verify
            CollectionAssert.AreEqual(getDataOutput.Header, new List<string>{"timeStarted","timeFinished","ID_BB_GLOBAL","PX_LAST"});
            
            Assert.That(getDataOutput.Records.Count, Is.EqualTo(2));
            CollectionAssert.AreEquivalent(getDataOutput.Records[0], new Dictionary<string,string>
            {
                {"ID_BB_GLOBAL" , "BBG000BPHFS9"},
                {"PX_LAST", "209.830000"},
                {"timeStarted", "08/10/2020 09:31:52 +00:00"},
                {"timeFinished", "08/10/2020 09:32:09 +00:00"}
            });
            CollectionAssert.AreEquivalent(getDataOutput.Records[1], new Dictionary<string,string>
            {
                {"ID_BB_GLOBAL" , "BBG000BVPV84"},
                {"PX_LAST", "3195.690000"},
                {"timeStarted", "08/10/2020 09:31:52 +00:00"},
                {"timeFinished", "08/10/2020 09:32:09 +00:00"}
            });
        }
        
        [Test]
        public void Transform_OnOneBadInstrumentsResponse_IsWellFormedWithOnlyValidInstrument()
        {
            //when
            var responseId = "1602161569-1051504982_OneBadInstrument";
            var retrieveGetDataResponse = TestUtils.LoadGetDataResponseFromFile(responseId);
            
            //execute
            var getDataOutput = _transformer.Transform(retrieveGetDataResponse);
            
            //verify
            CollectionAssert.AreEqual(getDataOutput.Header, new List<string>{"timeStarted","timeFinished","ID_BB_GLOBAL","PX_LAST"});
            
            Assert.That(getDataOutput.Records.Count, Is.EqualTo(1));
            CollectionAssert.AreEquivalent(getDataOutput.Records[0], new Dictionary<string,string>
            {
                {"ID_BB_GLOBAL" , "BBG000BVPV84"},
                {"PX_LAST", "3195.690000"},
                {"timeStarted", "08/10/2020 12:53:06 +00:00"},
                {"timeFinished", "08/10/2020 12:53:24 +00:00"}
            });
        }

    }
}