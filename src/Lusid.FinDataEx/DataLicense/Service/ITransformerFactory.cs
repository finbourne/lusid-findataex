using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx.DataLicense.Service.Transform
{
    public interface ITransformerFactory
    {
        IResponseTransformer Build(DataTypes dataType);
    }
}