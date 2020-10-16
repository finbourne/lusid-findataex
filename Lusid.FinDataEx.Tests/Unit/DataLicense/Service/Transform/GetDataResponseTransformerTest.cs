using System.Collections.Generic;
using Lusid.FinDataEx.DataLicense.Service.Transform;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Unit.DataLicense.Service.Transform
{
    [TestFixture]
    public class GetDataResponseTransformerTest
    {
        private GetDataResponseTransformer _transformer;

        [SetUp]
        public void SetUp()
        {
            _transformer = new GetDataResponseTransformer();
        }

        [Test]
        public void transform_OnAllValidInstrumentsResponse_IsWellFormed()
        {
            //when
            var responseId = "1602149495-71386027_ValidInstruments";
            var retrieveGetDataResponse =  TestUtils.LoadResponseFromFile(responseId);
            
            //execute
            var getDataOutputs = _transformer.Transform(retrieveGetDataResponse);
            var getDataOutput = getDataOutputs[0];
            
            //verify
            Assert.That(getDataOutputs.Count, Is.EqualTo(1));
            CollectionAssert.AreEqual(getDataOutput.Header, new List<string>{"ID_BB_GLOBAL","PX_LAST"});
            
            Assert.That(getDataOutput.Records.Count, Is.EqualTo(2));
            CollectionAssert.AreEquivalent(getDataOutput.Records[0], new Dictionary<string,string>
            {
                {"ID_BB_GLOBAL" , "BBG000BPHFS9"},
                {"PX_LAST", "209.830000"}
            });
            CollectionAssert.AreEquivalent(getDataOutput.Records[1], new Dictionary<string,string>
            {
                {"ID_BB_GLOBAL" , "BBG000BVPV84"},
                {"PX_LAST", "3195.690000"}
            });
        }
        
        [Test]
        public void transform_OnOneBadInstrumentsResponse_IsWellFormedWithOnlyValidInstrument()
        {
            //when
            var responseId = "1602161569-1051504982_OneBadInstrument";
            var retrieveGetDataResponse =  TestUtils.LoadResponseFromFile(responseId);
            
            //execute
            var getDataOutputs = _transformer.Transform(retrieveGetDataResponse);
            var getDataOutput = getDataOutputs[0];
            
            //verify
            Assert.That(getDataOutputs.Count, Is.EqualTo(1));
            CollectionAssert.AreEqual(getDataOutput.Header, new List<string>{"ID_BB_GLOBAL","PX_LAST"});
            
            Assert.That(getDataOutput.Records.Count, Is.EqualTo(1));
            CollectionAssert.AreEquivalent(getDataOutput.Records[0], new Dictionary<string,string>
            {
                {"ID_BB_GLOBAL" , "BBG000BVPV84"},
                {"PX_LAST", "3195.690000"}
            });
        }

    }
}