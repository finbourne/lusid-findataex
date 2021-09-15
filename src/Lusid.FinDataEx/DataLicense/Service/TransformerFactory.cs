using Lusid.FinDataEx.DataLicense.Service.Transform;
using System;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx.DataLicense.Service
{
    public class TransformerFactory : ITransformerFactory
    {
        public IResponseTransformer Build(DataTypes dataType) => dataType switch
        {
            DataTypes.GetData => new GetDataResponseTransformer(),
            DataTypes.GetActions => new GetActionResponseTransformer(),
            _ => throw new NotSupportedException($"{dataType} is not a currently supported BBG Data type.")
        };
    }
}