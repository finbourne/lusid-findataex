﻿using System.Collections.Generic;
using Lusid.FinDataEx.DataLicense.Service.Transform;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Unit.DataLicense.Service.Transform
{
    [TestFixture]
    public class ActionsResponseTransformerTest
    {
        private GetActionResponseTransformer _transformer;

        [SetUp]
        public void SetUp()
        {
            _transformer = new GetActionResponseTransformer();
        }

        [Test]
        public void transform_OnValidCorpActions_ShouldProduceOutput()
        {
            //when
            var responseId = "1603798418-1052073180_ValidActions";
            var retrieveGetActionResponse =  TestUtils.LoadGetActionsResponseFromFile(responseId);
            
            //execute
            var getDataOutput = _transformer.Transform(retrieveGetActionResponse);
            
            //verify
            CollectionAssert.IsSupersetOf(getDataOutput.Header, new List<string>{"CP_RECORD_DT","CP_PAY_DT","CP_FREQ",
                "CP_NET_AMT","CP_TAX_AMT","CP_GROSS_AMT","CP_FRANKED_AMT", "CP_DVD_CRNCY", "CP_DVD_TYP","CP_ELECTION_DT", 
                "CP_ACTION_STATUS"});

            Assert.That(getDataOutput.Records.Count, Is.EqualTo(1));
            var corpActionRecord = getDataOutput.Records[0];
            Assert.That(corpActionRecord["CP_RECORD_DT"], Is.EqualTo("11/06/2020"));
            Assert.That(corpActionRecord["CP_PAY_DT"], Is.EqualTo("12/04/2020"));
            Assert.That(corpActionRecord["CP_FREQ"], Is.EqualTo("4"));
            Assert.That(corpActionRecord["CP_NET_AMT"], Is.EqualTo("N.A."));
            Assert.That(corpActionRecord["CP_TAX_AMT"], Is.EqualTo(" "));
            Assert.That(corpActionRecord["CP_GROSS_AMT"], Is.EqualTo("3.51"));
            Assert.That(corpActionRecord["CP_FRANKED_AMT"], Is.EqualTo(" "));
            Assert.That(corpActionRecord["CP_DVD_CRNCY"], Is.EqualTo("USD"));
            Assert.That(corpActionRecord["CP_DVD_TYP"], Is.EqualTo("1000"));
            Assert.That(corpActionRecord["CP_ELECTION_DT"], Is.EqualTo("N.A."));
            Assert.That(corpActionRecord["CP_ACTION_STATUS"], Is.EqualTo("R"));
        }
        
        [Test]
        public void transform_OnNoCorpActionsDataForInstrument_ShouldBeEmptyOutput()
        {
            //when
            var responseId = "1603900538-71379765_NoActions";
            var retrieveGetActionsResponse =  TestUtils.LoadGetActionsResponseFromFile(responseId);
            
            //execute
            var getActionsOutput = _transformer.Transform(retrieveGetActionsResponse);
            Assert.That(getActionsOutput.IsEmpty, Is.True);
        }

    }
}