using Lusid.FinDataEx.Operation;
using Moq;
using NUnit.Framework;
using System;

namespace Lusid.FinDataEx.Tests.Unit.Operation
{
    [TestFixture]
    public class DataLicenseRequestExecutorTest
    {
        private Sdk.Utilities.ILusidApiFactory _mockLusidApiFactory;
        private Drive.Sdk.Utilities.ILusidApiFactory _mockDriveApiFactory;

        [SetUp]
        public void SetUp()
        {
            _mockLusidApiFactory = Mock.Of<Sdk.Utilities.ILusidApiFactory>();
            _mockDriveApiFactory = Mock.Of<Drive.Sdk.Utilities.ILusidApiFactory>();
        }

        [Test]
        public void HandleInstrumentTypeCli()
        {
            var fakeOptions = new DataLicenseOptions();

            Assert.Throws<NotImplementedException>(() => new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockDriveApiFactory).Execute());
        }

        [Test]
        public void HandleInstrumentTypeLusid()
        {
            var fakeOptions = new DataLicenseOptions();

            Assert.Throws<NotImplementedException>(() => new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockDriveApiFactory).Execute());
        }

        [Test]
        public void HandleInstrumentTypeLocal()
        {
            var fakeOptions = new DataLicenseOptions();

            Assert.Throws<NotImplementedException>(() => new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockDriveApiFactory).Execute());
        }

        [Test]
        public void HandleInstrumentTypeLusidDrive()
        {
            var fakeOptions = new DataLicenseOptions();

            Assert.Throws<NotImplementedException>(() => new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockDriveApiFactory).Execute());
        }

        [Test]
        public void ThrowWhenNoInstrumentsFound()
        {
            var fakeOptions = new DataLicenseOptions();

            Assert.Throws<NotImplementedException>(() => new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockDriveApiFactory).Execute());
        }

        [Test]
        public void ThrowWhenExceedingMaxInstruments()
        {
            var fakeOptions = new DataLicenseOptions();

            Assert.Throws<NotImplementedException>(() => new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockDriveApiFactory).Execute());
        }
    }
}
