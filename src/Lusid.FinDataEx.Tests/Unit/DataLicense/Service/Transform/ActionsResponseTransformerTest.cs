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
        public void OnValidCorpActionsShouldProduceOutput()
        {
            var responseId = "ValidActions";
            var retrieveGetActionResponse = TestUtils.LoadGetActionsResponseFromFile(responseId);

            var getActionOutput = _transformer.Transform(retrieveGetActionResponse);

            // Verify data field headers from corp action (specific to corp action type)
            CollectionAssert.IsSupersetOf(getActionOutput[0].Keys, new List<string>{"CP_RECORD_DT","CP_PAY_DT","CP_FREQ",
                "CP_NET_AMT","CP_TAX_AMT","CP_GROSS_AMT","CP_FRANKED_AMT", "CP_DVD_CRNCY", "CP_DVD_TYP","CP_ELECTION_DT", 
                "CP_ACTION_STATUS"});

            // Verify standard corp action field headers from corp action ( general to any corp action type)
            CollectionAssert.IsSupersetOf(getActionOutput[0].Keys, new List<string>{"announceDate","bbGlobal","companyName",
                "currency","effectiveDate","mnemonic","CP_FRANKED_AMT", "CP_DVD_CRNCY", "CP_DVD_TYP","CP_ELECTION_DT", 
                "CP_ACTION_STATUS"});

            Assert.That(getActionOutput.Count, Is.EqualTo(1));

            var corpActionRecord = getActionOutput[0];
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
            Assert.That(corpActionRecord["timeStarted"], Is.EqualTo("27/10/2020 18:06:20 +00:00"));
            Assert.That(corpActionRecord["timeFinished"], Is.EqualTo("27/10/2020 18:06:22 +00:00"));
        }

        [Test]
        public void OnNoCorpActionsDataForInstrumentShouldBeEmptyOutput()
        {
            var responseId = "NoActions";
            var retrieveGetActionsResponse = TestUtils.LoadGetActionsResponseFromFile(responseId);

            var getActionsOutput = _transformer.Transform(retrieveGetActionsResponse);
            Assert.That(getActionsOutput, Is.Empty);
        }
    }
}