using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Lusid.FinDataEx.DataLicense.Service.Transform;
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.DataLicense.BbgDlTypes;

namespace Lusid.FinDataEx.DataLicense.Service
{
    public class DLDataService
    {
        /* DO NOT change PollInterval except for testing with Mocks. BBG DL will throttle
         or worse if poll interval against actual servers*/
        public const int PollInterval = 30000;
        public const int DataNotAvailable = 100;
        public const int Success = 0;
        public const int RequestError = 200;
        public const string InstrumentSuccessCode = "0";
        public const int NoCorpActionCode = 300;

        private readonly PerSecurityWS _perSecurityWs;

        public DLDataService(PerSecurityWS perSecurityWs)
        {
            _perSecurityWs = perSecurityWs;
        }

        public List<FinDataOutput> Get(List<string> bbgIds, ProgramTypes programType, DataTypes dataType)
        {
            Instruments instruments = CreateInstruments(bbgIds);
            IBbgCall<PerSecurityResponse> bbgCall = CreateBbgCall(instruments, programType, dataType);
            PerSecurityResponse perSecurityResponse = bbgCall.Get(instruments);
            List<FinDataOutput> finDataOutputs = TransformBbgResponse(perSecurityResponse, dataType);
            return finDataOutputs;
        }

        List<FinDataOutput> TransformBbgResponse(PerSecurityResponse perSecurityResponse, DataTypes dataType)
        {
            switch (dataType)
            {
                case DataTypes.GetData:
                    return new GetDataResponseTransformer().Transform((RetrieveGetDataResponse) perSecurityResponse);
                default:
                    throw new InvalidEnumArgumentException($"{dataType} is not a currently supported BBG Data type.");
            }
        }

        // Ask Riz why this won't work with generics and extension?? Maybe think again!
        //IBbgCall<T> CreateBbgCall<T>(Instruments instruments, ProgramTypes programType, DataTypes dataType) where T : extends PerSecurityResponse
        IBbgCall<PerSecurityResponse> CreateBbgCall(Instruments instruments, ProgramTypes programType, DataTypes dataType)
        {
            switch (dataType)
            {
                case DataTypes.GetData:
                    return new GetDataBbgCall(_perSecurityWs);
                default:
                    throw new InvalidEnumArgumentException($"{dataType} is not a currently supported BBG Data type.");
            }
        }

        Instruments CreateInstruments(List<string> bbgIds)
        {
            var instruments = bbgIds.Select(id => new Instrument()
            {
                id = id,
                type = InstrumentType.BB_GLOBAL
            }).ToArray();
            return new Instruments()
            {
                instrument = instruments
            };
        }

    }
}