using Lusid.FinDataEx.DataLicense.Service.Instrument;
using Lusid.FinDataEx.Util.FileUtils.Handler;
using Moq;
using NUnit.Framework;
using PerSecurity_Dotnet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lusid.FinDataEx.Tests.Unit.Input.InputReader.InstrumentSource
{
    [TestFixture]
    public class FileInstrumentSourceTests
    {
        private IFileHandler _mockFileHandler;

        [SetUp]
        public void SetUp()
        {
            _mockFileHandler = Mock.Of<IFileHandler>();
        }

        [Test]
        public void SingleInstrument()
        {
            var fakeData = new List<string> { "field1,field2,instrumentName,field4", "value1,value2,instrument1,value4" };

            var goodPath = "path";
            var goodDelimeter = ",";
            var goodColIndex = "2";

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.ValidatePath(It.Is<string>(s => s.Equals(goodPath))))
                .Returns(goodPath);

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.Read(It.Is<string>(s => s.Equals(goodPath)), It.IsAny<char>()))
                .Returns(fakeData);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            var output = FileInstrumentSource.Create(_mockFileHandler, instrumentArgs, new List<string> { goodPath, goodDelimeter, goodColIndex }).Get();
            var outputInstrument = output.instrument.Single();

            Assert.That(outputInstrument.id, Is.EqualTo("instrument1"));
            Assert.That(outputInstrument.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument.typeSpecified, Is.True);
            Assert.That(outputInstrument.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument.yellowkey, Is.EqualTo(MarketSector.Govt));
        }

        [Test]
        public void MultipleInstrument()
        {
            var fakeData = new List<string> { "field1,field2,instrumentName,field4", "value1,value2,instrument1,value4", "value1,value2,instrument2,value4" };

            var goodPath = "path";
            var goodDelimeter = ",";
            var goodColIndex = "2";

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.ValidatePath(It.Is<string>(s => s.Equals(goodPath))))
                .Returns(goodPath);

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.Read(It.Is<string>(s => s.Equals(goodPath)), It.IsAny<char>()))
                .Returns(fakeData);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            var output = FileInstrumentSource.Create(_mockFileHandler, instrumentArgs, new List<string> { goodPath, goodDelimeter, goodColIndex }).Get();
            var outputInstrument1 = output.instrument.First();

            Assert.That(outputInstrument1.id, Is.EqualTo("instrument1"));
            Assert.That(outputInstrument1.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument1.typeSpecified, Is.True);
            Assert.That(outputInstrument1.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument1.yellowkey, Is.EqualTo(MarketSector.Govt));

            var outputInstrument2 = output.instrument.Last();

            Assert.That(outputInstrument2.id, Is.EqualTo("instrument2"));
            Assert.That(outputInstrument2.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument2.typeSpecified, Is.True);
            Assert.That(outputInstrument2.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument2.yellowkey, Is.EqualTo(MarketSector.Govt));
        }

        [Test]
        public void DuplicateInstrumentsAreIgnored()
        {
            var fakeData = new List<string>
            { 
                "field1,field2,instrumentName,field4",
                "value1,value2,instrument1,value4",
                "value1,value2,instrument2,value4",
                "value1,value2,instrument1,value4"
            };

            var goodPath = "path";
            var goodDelimeter = ",";
            var goodColIndex = "2";

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.ValidatePath(It.Is<string>(s => s.Equals(goodPath))))
                .Returns(goodPath);

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.Read(It.Is<string>(s => s.Equals(goodPath)), It.IsAny<char>()))
                .Returns(fakeData);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            var output = FileInstrumentSource.Create(_mockFileHandler, instrumentArgs, new List<string> { goodPath, goodDelimeter, goodColIndex }).Get();

            Assert.That(output.instrument.Length, Is.EqualTo(2));

            var outputInstrument1 = output.instrument.First();

            Assert.That(outputInstrument1.id, Is.EqualTo("instrument1"));
            Assert.That(outputInstrument1.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument1.typeSpecified, Is.True);
            Assert.That(outputInstrument1.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument1.yellowkey, Is.EqualTo(MarketSector.Govt));

            var outputInstrument2 = output.instrument.Last();

            Assert.That(outputInstrument2.id, Is.EqualTo("instrument2"));
            Assert.That(outputInstrument2.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument2.typeSpecified, Is.True);
            Assert.That(outputInstrument2.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument2.yellowkey, Is.EqualTo(MarketSector.Govt));
        }

        [Test]
        public void ColIndexDefaultsToZero()
        {
            var fakeData = new List<string> { "instrumentName,field2,field3,field4", "instrument1,value2,value3,value4" };

            var goodPath = "path";
            var goodDelimeter = ",";

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.ValidatePath(It.Is<string>(s => s.Equals(goodPath))))
                .Returns(goodPath);

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.Read(It.Is<string>(s => s.Equals(goodPath)), It.IsAny<char>()))
                .Returns(fakeData);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            var output = FileInstrumentSource.Create(_mockFileHandler, instrumentArgs, new List<string> { goodPath, goodDelimeter }).Get();
            var outputInstrument = output.instrument.Single();

            Assert.That(outputInstrument.id, Is.EqualTo("instrument1"));
            Assert.That(outputInstrument.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument.typeSpecified, Is.True);
            Assert.That(outputInstrument.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument.yellowkey, Is.EqualTo(MarketSector.Govt));
        }

        [Test]
        public void DelimeterDefaultsToComma()
        {
            var fakeData = new List<string> { "instrumentName,field2,field3,field4", "instrument1,value2,value3,value4" };

            var goodPath = "path";

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.ValidatePath(It.Is<string>(s => s.Equals(goodPath))))
                .Returns(goodPath);

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.Read(It.Is<string>(s => s.Equals(goodPath)), It.IsAny<char>()))
                .Returns(fakeData);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            var output = FileInstrumentSource.Create(_mockFileHandler, instrumentArgs, new List<string> { goodPath }).Get();
            var outputInstrument = output.instrument.Single();

            Assert.That(outputInstrument.id, Is.EqualTo("instrument1"));
            Assert.That(outputInstrument.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument.typeSpecified, Is.True);
            Assert.That(outputInstrument.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument.yellowkey, Is.EqualTo(MarketSector.Govt));
        }

        [Test]
        public void ThrowOnMissingSourceArgs()
        {
            var fakeData = new List<string> { "field1,field2,instrumentName,field4", "value1,value2,instrument1,value4" };

            var goodPath = "path";

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.ValidatePath(It.Is<string>(s => s.Equals(goodPath))))
                .Returns(goodPath);

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.Read(It.Is<string>(s => s.Equals(goodPath)), It.IsAny<char>()))
                .Returns(fakeData);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            Assert.Throws<ArgumentException>(() => FileInstrumentSource.Create(_mockFileHandler, instrumentArgs, new List<string>()).Get());
        }

        [Test]
        public void DoesNotThrowOnMissingInstrumentIds()
        {
            var fakeData = new List<string> { "field1,field2,instrumentName,field4" };

            var goodPath = "path";
            var goodDelimeter = ",";
            var goodColIndex = "2";

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.ValidatePath(It.Is<string>(s => s.Equals(goodPath))))
                .Returns(goodPath);

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.Read(It.Is<string>(s => s.Equals(goodPath)), It.IsAny<char>()))
                .Returns(fakeData);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            var output = FileInstrumentSource.Create(_mockFileHandler, instrumentArgs, new List<string> { goodPath, goodDelimeter, goodColIndex }).Get();

            Assert.That(output.instrument, Is.Empty);
        }
    }
}