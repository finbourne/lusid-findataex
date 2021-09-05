using System;
using System.Linq;
using Lusid.FinDataEx.Output.OutputInterpreter;
using Lusid.Sdk.Api;
using Lusid.Sdk.Model;
using Lusid.Sdk.Utilities;

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
            if (!(_getOptions is GetActionsOptions))
            {
                Console.WriteLine($"LusidTenantOutputWriter does not support requests other than GetActions. Skipping...");
                return WriteResult.NotRun();
            }

            if (dataLicenseOutput.IsEmpty())
            {
                Console.WriteLine($"Attempting to write empty data license output : {dataLicenseOutput}. Skipping...");
                return WriteResult.NotRun();
            }

            var corporateActionSourceComponents = _getOptions.OutputLusid.Split(":");
            var scope = corporateActionSourceComponents.First();
            var code = corporateActionSourceComponents.Last();

            var interpreter = CreateInterpreter(_getOptions);

            var actions = interpreter.Interpret(dataLicenseOutput);

            try
            {
                // Upsert corporate actions
                UpsertCorporateActionsResponse result = _corporateActionSourcesApi.BatchUpsertCorporateActions(scope, code, actions);
                Console.WriteLine(result);
                return WriteResult.Ok(_getOptions.OutputLusid);
            }
            catch (Exception e)
            {
                return WriteResult.Fail($"FAILURE : Did not write {dataLicenseOutput.Id} to scope {scope}, code {code} due to an exception. Cause of failure: {e.Message}");
            }
        }

        private static IOutputInterpreter CreateInterpreter(DataLicenseOptions getOptions)
        {
            if (!string.IsNullOrWhiteSpace(getOptions.BBGSource))
            {
                return new FileInterpreter();
            }
            else if (!string.IsNullOrWhiteSpace(getOptions.InstrumentSource))
            {
                return new ServiceInterpreter();
            }
            else
            {
                throw new ArgumentNullException("No available interpreters");
            }
        }
    }
}