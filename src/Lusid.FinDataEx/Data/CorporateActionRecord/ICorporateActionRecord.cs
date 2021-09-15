using Lusid.Sdk.Model;
using System;
using System.Collections.Generic;

namespace Lusid.FinDataEx.Data.CorporateActionRecord
{
    public interface ICorporateActionRecord : IRecord
    {
        string GetActionCode(string sourceId, string requestId);

        string GetDescription();

        DateTimeOffset? GetAnnouncementDate();

        DateTimeOffset? GetExecutionDate();

        DateTimeOffset? GetRecordDate();

        DateTimeOffset? GetPaymentDate();

        CorporateActionTransitionComponentRequest GetInputInstrument();

        List<CorporateActionTransitionComponentRequest> GetOutputInstruments();

        public UpsertCorporateActionRequest ConstructRequest(string sourceId, string requestId)
        {
            var corporateActionCode = GetActionCode(sourceId, requestId);
            var description = GetDescription();
            var announcementDate = GetAnnouncementDate();
            var executionDate = GetExecutionDate();
            var recordDate = GetRecordDate();
            var paymentDate = GetPaymentDate();
            var inputTransition = GetInputInstrument();
            var outputTransition = GetOutputInstruments();

            var transitions = new List<CorporateActionTransitionRequest> { new CorporateActionTransitionRequest(inputTransition, outputTransition) };

            return new UpsertCorporateActionRequest(corporateActionCode, description, announcementDate, executionDate, recordDate, paymentDate, transitions);
        }
    }
}