using Lusid.Sdk.Utilities;
using Lusid.FinDataEx.Operation;
using Lusid.FinDataEx.Util;
using Moq;
using NUnit.Framework;
using System;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Transform;
using Lusid.FinDataEx.DataLicense.Vendor;
using Lusid.FinDataEx.Util.FileUtils.Handler;
using Lusid.FinDataEx.Util.FileUtils;
using System.Collections.Generic;
using Lusid.FinDataEx.Data.DataRecord;
using Lusid.FinDataEx.Data;
using System.Linq;
using PerSecurity_Dotnet;
using System.ServiceModel;
using Lusid.FinDataEx.DataLicense.Service.Call;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;
using Lusid.Sdk.Api;
using Lusid.Sdk;
using Lusid.Sdk.Model;

namespace Lusid.FinDataEx.Tests.Unit.Operation
{
    [TestFixture]
    public class DataLicenseRequestExecutorTest
    {
        private ILusidApiFactory _mockLusidApiFactory;
        private ITransactionPortfoliosApi _mockTransactionPortfoliosApi;
        private IFileHandlerFactory _mockFileHandlerFactory;
        private IFileHandler _mockFileHandler;
        private IDataLicenseService _mockDataLicenseService;
        private IPerSecurityWsFactory _mockPerSecurityWsFactory;
        private ITransformerFactory _mockTransformerFactory;
        private IResponseTransformer _mockTransformer;

        [SetUp]
        public void SetUp()
        {
            _mockLusidApiFactory = Mock.Of<ILusidApiFactory>();
            _mockTransactionPortfoliosApi = Mock.Of<ITransactionPortfoliosApi>();
            _mockFileHandlerFactory = Mock.Of<IFileHandlerFactory>();
            _mockFileHandler = Mock.Of<IFileHandler>();
            _mockDataLicenseService = Mock.Of<IDataLicenseService>();
            _mockPerSecurityWsFactory = Mock.Of<IPerSecurityWsFactory>();
            _mockTransformerFactory = Mock.Of<ITransformerFactory>();
            _mockTransformer = Mock.Of<IResponseTransformer>();

            var fakeResponse = new RetrieveGetDataResponse
            {
                requestId = "output"
            };

            var fakeInstrumentList = new List<string> { "myInstrumentHeader,dummyHeader", "myInstrument,dummyValue" };

            var fakeInstrument = new PerSecurity_Dotnet.Instrument
            {
                id = "myInstrument"
            };

            var fakeHoldings = new List<PortfolioHolding>
            {
                new PortfolioHolding(
                    "instrument1Luid",
                    null,
                    new Dictionary<string, Property>
                    {
                        {
                            "Instrument/default/Isin",
                            new Property(
                                "Instrument/default/Isin",
                                new PropertyValue("myInstrument")
                            )
                        }
                    },
                    "dummyHolding",
                    0,
                    0,
                    new CurrencyAndAmount(0, "USD"),
                    new CurrencyAndAmount(0, "CCY_USD")
               )
            };
            var fakeHoldingsData = new VersionedResourceListOfPortfolioHolding(new Sdk.Model.Version(DateTimeOffset.MinValue, DateTimeOffset.MaxValue), fakeHoldings, null, null);

            var fakeInstruments = new Instruments
            {
                instrument = new PerSecurity_Dotnet.Instrument[] { fakeInstrument }
            };

            var fakeDataResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "field1", "value1" },
                    { "field2", "value2" },
                    { "field3", "value3" }
                }
            };

            Mock.Get(_mockLusidApiFactory)
                .Setup(mock => mock.Api<ITransactionPortfoliosApi>())
                .Returns(_mockTransactionPortfoliosApi);

            Mock.Get(_mockTransactionPortfoliosApi)
                .Setup(mock => mock.GetHoldings(
                    It.Is<string>(s => s.Equals("myScope")),
                    It.Is<string>(s => s.Equals("myPortfolio")),
                    It.IsAny<DateTimeOrCutLabel>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<bool?>()))
                .Returns(fakeHoldingsData);

            Mock.Get(_mockFileHandlerFactory)
                .Setup(mock => mock.Build(It.IsAny<FileHandlerType>()))
                .Returns(_mockFileHandler);

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.ValidatePath(It.Is<string>(s => s.Equals("myPath"))))
                .Returns("myPath");

            Mock.Get(_mockFileHandler)
                .Setup(mock => mock.Read(It.Is<string>(s => s.Equals("myPath")), It.IsAny<char>()))
                .Returns(fakeInstrumentList);

            Mock.Get(_mockPerSecurityWsFactory)
                .Setup(mock => mock.CreateDefault())
                .Returns(new PerSecurityWSClient(new BasicHttpBinding(), new EndpointAddress("http://example.test/not/real/endpoint")));

            Mock.Get(_mockDataLicenseService)
                .Setup(mock => mock.Get(
                    It.IsAny<IDataLicenseCall<PerSecurityResponse>>(),
                    It.Is<Instruments>(i => i.instrument.Any(Instrument => Instrument.id.Equals("myInstrument"))),
                    It.IsAny<ProgramTypes>(),
                    It.IsAny<bool>()))
                .Returns(fakeResponse);

            Mock.Get(_mockTransformerFactory)
                .Setup(mock => mock.Build(It.IsAny<DataTypes>()))
                .Returns(_mockTransformer);

            Mock.Get(_mockTransformer)
                .Setup(mock => mock.Transform(It.IsAny<PerSecurityResponse>()))
                .Returns(fakeDataResponse);
        }

        [Test]
        public void HandleInstrumentTypeLocal()
        {
            var fakeOptions = new GetDataOptions
            {
                InputPath = "myPath",
                InputSource = InputType.Local,
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
                InstrumentSourceArguments = new List<string> { "myPath" },
                MaxInstruments = 5,
                DataFields = new List<string> { "dummyField" }
            };

            var output = new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockFileHandlerFactory, _mockDataLicenseService, _mockPerSecurityWsFactory, _mockTransformerFactory).Execute();

            Assert.That(output.DataRecords.Length, Is.EqualTo(1));
            Assert.That(output.DataRecords.Single().RawData.Length(), Is.EqualTo(3));
            Assert.That(output.DataRecords[0].RawData["field1"], Is.EqualTo("value1"));
            Assert.That(output.DataRecords[0].RawData["field2"], Is.EqualTo("value2"));
            Assert.That(output.DataRecords[0].RawData["field3"], Is.EqualTo("value3"));
        }

        [Test]
        public void HandleInstrumentTypeLusidDrive()
        {
            var fakeOptions = new GetDataOptions
            {
                InputPath = "myPath",
                InputSource = InputType.Drive,
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
                InstrumentSourceArguments = new List<string> { "myPath" },
                MaxInstruments = 5,
                DataFields = new List<string> { "dummyField" }
            };

            var output = new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockFileHandlerFactory, _mockDataLicenseService, _mockPerSecurityWsFactory, _mockTransformerFactory).Execute();

            Assert.That(output.DataRecords.Length, Is.EqualTo(1));
            Assert.That(output.DataRecords.Single().RawData.Length(), Is.EqualTo(3));
            Assert.That(output.DataRecords[0].RawData["field1"], Is.EqualTo("value1"));
            Assert.That(output.DataRecords[0].RawData["field2"], Is.EqualTo("value2"));
            Assert.That(output.DataRecords[0].RawData["field3"], Is.EqualTo("value3"));
        }

        [Test]
        public void HandleInstrumentTypeCli()
        {
            var fakeOptions = new GetDataOptions
            {
                InputPath = "myPath",
                InputSource = InputType.CLI,
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
                InstrumentSourceArguments = new List<string> { "myInstrument" },
                MaxInstruments = 5,
                DataFields = new List<string> { "dummyField" }
            };

            var output = new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockFileHandlerFactory, _mockDataLicenseService, _mockPerSecurityWsFactory, _mockTransformerFactory).Execute();

            Assert.That(output.DataRecords.Length, Is.EqualTo(1));
            Assert.That(output.DataRecords.Single().RawData.Length(), Is.EqualTo(3));
            Assert.That(output.DataRecords[0].RawData["field1"], Is.EqualTo("value1"));
            Assert.That(output.DataRecords[0].RawData["field2"], Is.EqualTo("value2"));
            Assert.That(output.DataRecords[0].RawData["field3"], Is.EqualTo("value3"));
        }

        [Test]
        public void HandleInstrumentTypeLusid()
        {
            var fakeOptions = new GetDataOptions
            {
                InputPath = "myPath",
                InputSource = InputType.Lusid,
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
                InstrumentSourceArguments = new List<string> { "myScope|myPortfolio" },
                MaxInstruments = 5,
                DataFields = new List<string> { "dummyField" }
            };

            var output = new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockFileHandlerFactory, _mockDataLicenseService, _mockPerSecurityWsFactory, _mockTransformerFactory).Execute();

            Assert.That(output.DataRecords.Length, Is.EqualTo(1));
            Assert.That(output.DataRecords.Single().RawData.Length(), Is.EqualTo(3));
            Assert.That(output.DataRecords[0].RawData["field1"], Is.EqualTo("value1"));
            Assert.That(output.DataRecords[0].RawData["field2"], Is.EqualTo("value2"));
            Assert.That(output.DataRecords[0].RawData["field3"], Is.EqualTo("value3"));
        }

        [Test]
        public void ThrowWhenNoInstrumentsFound()
        {
            var fakeOptions = new GetDataOptions
            {
                InputPath = "myPath",
                InputSource = InputType.CLI,
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
                InstrumentSourceArguments = new List<string> { },
                MaxInstruments = 5,
                DataFields = new List<string> { "dummyField" }
            };

            Assert.Throws<ArgumentException>(() => new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockFileHandlerFactory, _mockDataLicenseService, _mockPerSecurityWsFactory, _mockTransformerFactory).Execute());
        }

        [Test]
        public void ThrowWhenExceedingMaxInstruments()
        {
            var fakeOptions = new GetDataOptions
            {
                InputPath = "myPath",
                InputSource = InputType.CLI,
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
                InstrumentSourceArguments = new List<string> { "myInstrument" },
                MaxInstruments = 0,
                DataFields = new List<string> { "dummyField" }
            };

            Assert.Throws<ArgumentException>(() => new DataLicenseRequestExecutor(fakeOptions, _mockLusidApiFactory, _mockFileHandlerFactory, _mockDataLicenseService, _mockPerSecurityWsFactory, _mockTransformerFactory).Execute());
        }
    }
}