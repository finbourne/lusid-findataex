using Lusid.FinDataEx.Input;
using Lusid.FinDataEx.Util.FileUtils.Handler;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lusid.FinDataEx.Tests.Unit.Input
{
    [TestFixture]
    public class FileInputReaderTests
    {
        private IFileHandler _mockFileHandler;

        [SetUp]
        public void SetUp()
        {
            _mockFileHandler = Mock.Of<IFileHandler>();

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.ValidatePath(It.Is<string>(s => s.Equals("myPath"))))
                .Returns("myPath");
        }

        [Test]
        public void SingleInstrument()
        {
            var fakeOptions = new DataLicenseOptions
            {
                InputPath = "myPath"
            };

            var fakeData = new List<string> { "field1,field2,field3", "value1,value2,value3" };

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.Read(It.Is<string>(s => s.Equals("myPath")), It.IsAny<char>()))
                .Returns(fakeData);

            var output = new FileInputReader(fakeOptions, _mockFileHandler).Read();

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
        public void MultipleInstrument()
        {
            var fakeOptions = new DataLicenseOptions
            {
                InputPath = "myPath"
            };

            var fakeData = new List<string> { "field1,field2,field3", "value11,value12,value13", "value21,value22,value23" };

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.Read(It.Is<string>(s => s.Equals("myPath")), It.IsAny<char>()))
                .Returns(fakeData);

            var output = new FileInputReader(fakeOptions, _mockFileHandler).Read();

            Assert.That(output.Header.Length, Is.EqualTo(3));
            Assert.That(output.Header.ToList()[0], Is.EqualTo("0-field1"));
            Assert.That(output.Header.ToList()[1], Is.EqualTo("1-field2"));
            Assert.That(output.Header.ToList()[2], Is.EqualTo("2-field3"));

            Assert.That(output.Records.Length, Is.EqualTo(2));
            Assert.That(output.Records[0].Length, Is.EqualTo(3));
            Assert.That(output.Records[0]["0-field1"], Is.EqualTo("value11"));
            Assert.That(output.Records[0]["1-field2"], Is.EqualTo("value12"));
            Assert.That(output.Records[0]["2-field3"], Is.EqualTo("value13"));

            Assert.That(output.Records[1].Length, Is.EqualTo(3));
            Assert.That(output.Records[1]["0-field1"], Is.EqualTo("value21"));
            Assert.That(output.Records[1]["1-field2"], Is.EqualTo("value22"));
            Assert.That(output.Records[1]["2-field3"], Is.EqualTo("value23"));
        }

        [Test]
        public void ThrowsOnEmptyInstrument()
        {
            var fakeOptions = new DataLicenseOptions
            {
                InputPath = "myPath"
            };

            var fakeData = new List<string> { };

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.Read(It.Is<string>(s => s.Equals("myPath")), It.IsAny<char>()))
                .Returns(fakeData);

            Assert.Throws<InvalidOperationException>(() => new FileInputReader(fakeOptions, _mockFileHandler).Read());
        }

        [Test]
        public void DuplicateColumnNamesAreDiambiguated()
        {
            var fakeOptions = new DataLicenseOptions
            {
                InputPath = "myPath"
            };

            var fakeData = new List<string> { "field1,field1,field1", "value1,value2,value3" };

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.Read(It.Is<string>(s => s.Equals("myPath")), It.IsAny<char>()))
                .Returns(fakeData);

            var output = new FileInputReader(fakeOptions, _mockFileHandler).Read();

            Assert.That(output.Header.Length, Is.EqualTo(3));
            Assert.That(output.Header.ToList()[0], Is.EqualTo("0-field1"));
            Assert.That(output.Header.ToList()[1], Is.EqualTo("1-field1"));
            Assert.That(output.Header.ToList()[2], Is.EqualTo("2-field1"));

            Assert.That(output.Records.Length, Is.EqualTo(1));
            Assert.That(output.Records.Single().Length, Is.EqualTo(3));
            Assert.That(output.Records.Single()["0-field1"], Is.EqualTo("value1"));
            Assert.That(output.Records.Single()["1-field1"], Is.EqualTo("value2"));
            Assert.That(output.Records.Single()["2-field1"], Is.EqualTo("value3"));
        }
    }
}