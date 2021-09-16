using Lusid.Sdk.Utilities;
using Lusid.FinDataEx.Operation;
using Lusid.FinDataEx.Util;
using Moq;
using NUnit.Framework;
using System;

namespace Lusid.FinDataEx.Tests.Unit.Operation
{
    [TestFixture]
    public class DataLicenseRequestExecutorTest
    {
        private ILusidApiFactory _mockLusidApiFactory;
        private IFileHandlerFactory _mockFileHandlerFactory;

        [SetUp]
        public void SetUp()
        {
            _mockLusidApiFactory = Mock.Of<ILusidApiFactory>();
            _mockFileHandlerFactory = Mock.Of<IFileHandlerFactory>();
        }

        [Test]
        public void HandleInstrumentTypeCli()
        {
            var fakeOptions = new DataLicenseOptions();

            Assert.Throws<NotImplementedException>(() => new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockFileHandlerFactory).Execute());
        }

        [Test]
        public void HandleInstrumentTypeLusid()
        {
            var fakeOptions = new DataLicenseOptions();

            Assert.Throws<NotImplementedException>(() => new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockFileHandlerFactory).Execute());
        }

        [Test]
        public void HandleInstrumentTypeLocal()
        {
            var fakeOptions = new DataLicenseOptions();

            Assert.Throws<NotImplementedException>(() => new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockFileHandlerFactory).Execute());
        }

        [Test]
        public void HandleInstrumentTypeLusidDrive()
        {
            var fakeOptions = new DataLicenseOptions();

            Assert.Throws<NotImplementedException>(() => new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockFileHandlerFactory).Execute());
        }

        [Test]
        public void ThrowWhenNoInstrumentsFound()
        {
            var fakeOptions = new DataLicenseOptions();

            Assert.Throws<NotImplementedException>(() => new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockFileHandlerFactory).Execute());
        }

        [Test]
        public void ThrowWhenExceedingMaxInstruments()
        {
            var fakeOptions = new DataLicenseOptions();

            Assert.Throws<NotImplementedException>(() => new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockFileHandlerFactory).Execute());
        }
    }
}
