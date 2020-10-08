using System.Collections.Generic;
using Lusid.FinDataEx.DataLicense.Service.Transform;
using NUnit.Framework;
using PerSecurity_Dotnet;

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
            string responseId = "1602149495-71386027_ValidInstruments";
            RetrieveGetDataResponse retrieveGetDataResponse =  TestUtils.LoadResponseFromFile(responseId);
            
            //execute
            List<FinDataOutput> getDataOutputs = _transformer.Transform(retrieveGetDataResponse);
            FinDataOutput getDataOutput = getDataOutputs[0];
            
            //verify
            Assert.That(getDataOutputs.Count, Is.EqualTo(1));
            CollectionAssert.AreEqual(getDataOutput.Header, new List<string>{"ID_BB_UNIQUE","PX_LAST"});
            
            Assert.That(getDataOutput.Records.Count, Is.EqualTo(2));
            CollectionAssert.AreEquivalent(getDataOutput.Records[0], new Dictionary<string,string>
            {
                {"ID_BB_UNIQUE" , "EQ0010174300001000"},
                {"PX_LAST", "209.830000"}
            });
            CollectionAssert.AreEquivalent(getDataOutput.Records[1], new Dictionary<string,string>
            {
                {"ID_BB_UNIQUE" , "EQ0021695200001000"},
                {"PX_LAST", "3195.690000"}
            });
        }
        
        [Test]
        public void transform_OnOneBadInstrumentsResponse_IsWellFormedWithOnlyValidInstrument()
        {
            //when
            string responseId = "1602161569-1051504982_OneBadInstrument";
            RetrieveGetDataResponse retrieveGetDataResponse =  TestUtils.LoadResponseFromFile(responseId);
            
            //execute
            List<FinDataOutput> getDataOutputs = _transformer.Transform(retrieveGetDataResponse);
            FinDataOutput getDataOutput = getDataOutputs[0];
            
            //verify
            Assert.That(getDataOutputs.Count, Is.EqualTo(1));
            CollectionAssert.AreEqual(getDataOutput.Header, new List<string>{"ID_BB_UNIQUE","PX_LAST"});
            
            Assert.That(getDataOutput.Records.Count, Is.EqualTo(1));
            CollectionAssert.AreEquivalent(getDataOutput.Records[0], new Dictionary<string,string>
            {
                {"ID_BB_UNIQUE" , "EQ0021695200001000"},
                {"PX_LAST", "3195.690000"}
            });
        }

    }
}