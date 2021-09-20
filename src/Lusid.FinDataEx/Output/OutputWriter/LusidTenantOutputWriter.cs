using System;
using System.Linq;
using Lusid.Sdk.Api;
using Lusid.Sdk.Utilities;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx.Output
{
    public class LusidTenantOutputWriter : IOutputWriter
    {
        private readonly ICorporateActionSourcesApi _corporateActionSourcesApi;
        private readonly DataLicenseOptions _getOptions;

        public LusidTenantOutputWriter(DataLicenseOptions getOptions, ILusidApiFactory lusidApiFactory)
        {
            _corporateActionSourcesApi = lusidApiFactory.Api<ICorporateActionSourcesApi>();
            _getOptions = getOptions;
        }

        public WriteResult Write(DataLicenseOutput dataLicenseOutput)
        {
            if (!(_getOptions is GetActionsOptions getActionsOptions))
            {
                Console.WriteLine($"LusidTenantOutputWriter does not support requests other than GetActions. Skipping...");
                return WriteResult.NotRun();
            }
            
            if (getActionsOptions.CorpActionTypes.Count() > 1 ||
                getActionsOptions.CorpActionTypes.Single() != CorpActionType.DVD_CASH)
            {
                Console.WriteLine($"LusidTenantOutputWriter does not Action types other than {CorpActionType.DVD_CASH}. Skipping...");
                return WriteResult.NotRun();
            }

            if (dataLicenseOutput.IsEmpty())
            {
                Console.WriteLine($"Attempting to write empty data license output : {dataLicenseOutput}. Skipping...");
                return WriteResult.NotRun();
            }

            var corporateActionSourceComponents = _getOptions.OutputPath.Split("|");
            var scope = corporateActionSourceComponents.First();
            var code = corporateActionSourceComponents.Last();

            var actions = dataLicenseOutput.CorporateActionRecords.Select(r => r.ConstructRequest(dataLicenseOutput.Id, dataLicenseOutput.GetHashCode().ToString())).ToList();

            try
            {
                var result = _corporateActionSourcesApi.BatchUpsertCorporateActions(scope, code, actions);

                if (result.Failed.Any())
                {
                    throw new AggregateException("One or more actions failed", result.Failed.Values.Select(e => new Exception(e.ToString())));
                }

                return WriteResult.Ok(_getOptions.OutputPath);
            }
            catch (Exception e)
            {
                return WriteResult.Fail($"FAILURE : Did not write {dataLicenseOutput.Id} to scope {scope}, code {code} due to an exception. Cause of failure: {e.Message}");
            }
        }
    }
}