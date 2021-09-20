using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Transform;
using NUnit.Framework;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx.Tests.Unit.DataLicense.Service
{
    [TestFixture]
    public class TransformerFactoryTests
    {
        [Test]
        public void ProducesDriveHandler()
        {
            var factory = new TransformerFactory();

            var handler = factory.Build(DataTypes.GetData);

            Assert.That(handler, Is.TypeOf<GetDataResponseTransformer>());
        }

        [Test]
        public void ProducesLocalHandler()
        {
            var factory = new TransformerFactory();

            var handler = factory.Build(DataTypes.GetActions);

            Assert.That(handler, Is.TypeOf<GetActionResponseTransformer>());
        }
    }
}