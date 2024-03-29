﻿using System.Collections.Generic;
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
            var responseId = "ValidInstruments";
            var retrieveGetDataResponse = TestUtils.LoadGetDataResponseFromFile(responseId);

            var getDataOutput = _transformer.Transform(retrieveGetDataResponse);

            CollectionAssert.AreEquivalent(getDataOutput[0].Keys, new List<string>{"timeStarted","timeFinished","ID_BB_GLOBAL","PX_LAST"});

            Assert.That(getDataOutput.Count, Is.EqualTo(2));
            CollectionAssert.AreEquivalent(getDataOutput[0], new Dictionary<string,string>
            {
                {"ID_BB_GLOBAL" , "BBG000BPHFS9"},
                {"PX_LAST", "209.830000"},
                {"timeStarted", "08/10/2020 09:31:52 +00:00"},
                {"timeFinished", "08/10/2020 09:32:09 +00:00"}
            });
            CollectionAssert.AreEquivalent(getDataOutput[1], new Dictionary<string,string>
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
            var responseId = "OneBadInstrument";
            var retrieveGetDataResponse = TestUtils.LoadGetDataResponseFromFile(responseId);

            var getDataOutput = _transformer.Transform(retrieveGetDataResponse);

            CollectionAssert.AreEquivalent(getDataOutput[0].Keys, new List<string>{"timeStarted","timeFinished","ID_BB_GLOBAL","PX_LAST"});

            Assert.That(getDataOutput.Count, Is.EqualTo(1));
            CollectionAssert.AreEquivalent(getDataOutput[0], new Dictionary<string,string>
            {
                {"ID_BB_GLOBAL" , "BBG000BVPV84"},
                {"PX_LAST", "3195.690000"},
                {"timeStarted", "08/10/2020 12:53:06 +00:00"},
                {"timeFinished", "08/10/2020 12:53:24 +00:00"}
            });
        }
    }
}