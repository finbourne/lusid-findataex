using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Util;
using Lusid.FinDataEx.Util.FileUtils;
using Lusid.FinDataEx.Util.FileUtils.Handler;
using Moq;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Unit.Util.FileUtils
{
    [TestFixture]
    public class FileHandlerFactoryTests
    {
        [Test]
        public void ProducesDriveHandler()
        {
            var factory = new FileHandlerFactory();

            var handler = factory.Build(FileHandlerType.Lusid, Mock.Of<ILusidApiFactory>());

            Assert.That(handler, Is.TypeOf<LusidDriveFileHandler>());
        }

        [Test]
        public void ProducesLocalHandler()
        {
            var factory = new FileHandlerFactory();

            var handler = factory.Build(FileHandlerType.Local, Mock.Of<ILusidApiFactory>());

            Assert.That(handler, Is.TypeOf<LocalFileHandler>());
        }
    }
}