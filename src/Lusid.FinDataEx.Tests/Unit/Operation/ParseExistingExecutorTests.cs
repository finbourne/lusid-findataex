using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Operation;
using Lusid.FinDataEx.Util;
using Lusid.FinDataEx.Util.FileUtils;
using Lusid.FinDataEx.Util.FileUtils.Handler;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lusid.FinDataEx.Tests.Unit.Operation
{
    [TestFixture]
    public class ParseExistingExecutorTest
    {
        private ILusidApiFactory _mockDriveApiFactory;
        private IFileHandlerFactory _mockFileHandlerFactory;

        [SetUp]
        public void SetUp()
        {
            _mockDriveApiFactory = Mock.Of<ILusidApiFactory>();
            _mockFileHandlerFactory = Mock.Of<IFileHandlerFactory>();
        }

        [Test]
        public void HandleInstrumentTypeLocal()
        {
            var fakeInstrumentResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "someHeader", "someValue" }
                }
            };

            var fakeOptions = new DataLicenseOptions
            {
                InputPath = "myPath",
                InputSource = InputType.Local
            };

            var fakeData = new List<string> { "field1,field2,field3", "value1,value2,value3" };

            var fakeFileHandler = Mock.Of<IFileHandler>();

            Mock.Get(fakeFileHandler)
                .Setup(mock => mock.ValidatePath(It.Is<string>(s => s.Equals("myPath"))))
                .Returns("myPath");

            Mock.Get(fakeFileHandler)
                .Setup(mock => mock.Read(It.Is<string>(s => s.Equals("myPath")), It.IsAny<char>()))
                .Returns(fakeData);

            Mock.Get(_mockFileHandlerFactory)
                .Setup(mock => mock.Build(It.Is<FileHandlerType>(f => f.Equals(FileHandlerType.Local)), It.IsAny<ILusidApiFactory>()))
                .Returns(fakeFileHandler);

            var fakeOutput = new DataLicenseOutput("output", new List<string>(), fakeInstrumentResponse);

            var output = new ParseExistingDataExecutor(fakeOptions, _mockDriveApiFactory, _mockFileHandlerFactory).Execute();

            Assert.That(output.Header.Length, Is.EqualTo(3));
            Assert.That(output.Header.ToList()[0], Is.EqualTo("0-field1"));
            Assert.That(output.Header.ToList()[1], Is.EqualTo("1-field2"));
            Assert.That(output.Header.ToList()[2], Is.EqualTo("2-field3"));

            Assert.That(output.Records.Length, Is.EqualTo(1));
            Assert.That(output.Records.Single().Length, Is.EqualTo(3));
            Assert.That(output.Records.Single()["0-field1"], Is.EqualTo("value1"));
            Assert.That(output.Records.Single()["1-field2"], Is.EqualTo("value2"));
            Assert.That(output.Records.Single()["2-field3"], Is.EqualTo("value3"));
        }

        [Test]
        public void HandleInstrumentTypeLusidDrive()
        {
            var fakeInstrumentResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "someHeader", "someValue" }
                }
            };

            var fakeOptions = new DataLicenseOptions
            {
                InputPath = "myPath",
                InputSource = InputType.Drive
            };

            var fakeData = new List<string> { "field1,field2,field3", "value1,value2,value3" };

            var fakeFileHandler = Mock.Of<IFileHandler>();

            Mock.Get(fakeFileHandler)
                .Setup(mock => mock.ValidatePath(It.Is<string>(s => s.Equals("myPath"))))
                .Returns("myPath");

            Mock.Get(fakeFileHandler)
                .Setup(mock => mock.Read(It.Is<string>(s => s.Equals("myPath")), It.IsAny<char>()))
                .Returns(fakeData);

            Mock.Get(_mockFileHandlerFactory)
                .Setup(mock => mock.Build(It.Is<FileHandlerType>(f => f.Equals(FileHandlerType.Lusid)), It.IsAny<ILusidApiFactory>()))
                .Returns(fakeFileHandler);

            var fakeOutput = new DataLicenseOutput("output", new List<string>(), fakeInstrumentResponse);

            var output = new ParseExistingDataExecutor(fakeOptions, _mockDriveApiFactory, _mockFileHandlerFactory).Execute();

            Assert.That(output.Header.Length, Is.EqualTo(3));
            Assert.That(output.Header.ToList()[0], Is.EqualTo("0-field1"));
            Assert.That(output.Header.ToList()[1], Is.EqualTo("1-field2"));
            Assert.That(output.Header.ToList()[2], Is.EqualTo("2-field3"));

            Assert.That(output.Records.Length, Is.EqualTo(1));
            Assert.That(output.Records.Single().Length, Is.EqualTo(3));
            Assert.That(output.Records.Single()["0-field1"], Is.EqualTo("value1"));
            Assert.That(output.Records.Single()["1-field2"], Is.EqualTo("value2"));
            Assert.That(output.Records.Single()["2-field3"], Is.EqualTo("value3"));
        }

        [Test]
        public void ThrowOnInstrumentTypeCli()
        {
            var fakeOptions = new DataLicenseOptions
            {
                InputSource = InputType.CLI
            };

            Assert.Throws<ArgumentNullException>(() => new ParseExistingDataExecutor(fakeOptions, _mockDriveApiFactory, _mockFileHandlerFactory).Execute());
        }

        [Test]
        public void ThrowOnInstrumentTypeLusid()
        {
            var fakeOptions = new DataLicenseOptions
            {
                InputSource = InputType.Lusid
            };

            Assert.Throws<ArgumentNullException>(() => new ParseExistingDataExecutor(fakeOptions, _mockDriveApiFactory, _mockFileHandlerFactory).Execute());
        }
    }
}