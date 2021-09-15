using Lusid.FinDataEx.Data;
using Lusid.FinDataEx.Data.DataRecord;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Call;
using Lusid.FinDataEx.DataLicense.Service.Transform;
using Lusid.FinDataEx.DataLicense.Vendor;
using Moq;
using NUnit.Framework;
using PerSecurity_Dotnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx.Tests.Unit.Input
{
    [TestFixture]
    public class DataLicenseInputReaderTests
    {
        private IDataLicenseService _mockDataLicenseService;
        private IPerSecurityWsFactory _mockPerSecurityWsFactory;
        private ITransformerFactory _mockTransformerFactory;
        private IResponseTransformer _mockTransformer;

        [SetUp]
        public void SetUp()
        {
            _mockDataLicenseService = Mock.Of<IDataLicenseService>();
            _mockPerSecurityWsFactory = Mock.Of<IPerSecurityWsFactory>();

            Mock.Get(_mockPerSecurityWsFactory)
                .Setup(mock => mock.CreateDefault())
                .Returns(new PerSecurityWSClient(new BasicHttpBinding(), new EndpointAddress("http://example.test/not/real/endpoint")));

            _mockTransformerFactory = Mock.Of<ITransformerFactory>();
            _mockTransformer = Mock.Of<IResponseTransformer>();
        }

        [Test]
        public void ReadProducesValidOutput()
        {
            var fakeOptions = new GetDataOptions
            {
                DataFields = new List<string> { "field" }
            };

            var fakeInstrument = new Instrument
            {
                id = "fakeinstrument"
            };

            var fakeInstruments = new Instruments
            {
                instrument = new Instrument[] { fakeInstrument }
            };

            var fakeResponse = new RetrieveGetDataResponse
            {
                requestId = "output"
            };

            var fakeDictionaryResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "someHeader", "someValue" }
                }
            };

            Mock.Get(_mockDataLicenseService)
                .Setup(mock => mock.Get(
                    It.Is<IDataLicenseCall<PerSecurityResponse>>(c => c is GetDataLicenseCall),
                    It.Is<Instruments>(i => i.instrument.Contains(fakeInstrument)),
                    It.IsAny<ProgramTypes>(),
                    It.IsAny<bool>()))
                .Returns(fakeResponse);

            Mock.Get(_mockTransformerFactory)
                .Setup(mock => mock.Build(It.Is<DataTypes>(d => d.Equals(DataTypes.GetData))))
                .Returns(_mockTransformer);

            Mock.Get(_mockTransformer)
                .Setup(mock => mock.Transform(It.IsAny<PerSecurityResponse>()))
                .Returns(fakeDictionaryResponse);

            var output = new DataLicenseInputReader(fakeOptions, fakeInstruments, _mockDataLicenseService, _mockPerSecurityWsFactory, _mockTransformerFactory).Read();

            var fakeInstrumentResponse = new List<IRecord>
            {
                new InstrumentDataRecord(fakeDictionaryResponse.Single())
            };

            var fakeOutput = new DataLicenseOutput("output", fakeInstrumentResponse);

            Assert.That(output.Id, Is.EqualTo(fakeOutput.Id));
            Assert.That(output.DataRecords.Count, Is.EqualTo(fakeOutput.DataRecords.Count));
            Assert.That(output.CorporateActionRecords, Is.Empty);
        }

        [Test]
        public void ThrowIfNoDataFields ()
        {
            var fakeOptions = new GetDataOptions();

            var fakeInstrument = new Instrument
            {
                id = "fakeinstrument"
            };
            var fakeInstruments = new Instruments
            {
                instrument = new Instrument[] { fakeInstrument }
            };

            var fakeResponse = new PerSecurityResponse
            {
                requestId = "output"
            };

            Mock.Get(_mockDataLicenseService)
                .Setup(mock => mock.Get(
                    It.Is<IDataLicenseCall<PerSecurityResponse>>(c => c is GetDataLicenseCall),
                    It.Is<Instruments>(i => i.instrument.Contains(fakeInstrument)),
                    It.IsAny<ProgramTypes>(),
                    It.IsAny<bool>()))
                .Returns(fakeResponse);

            Assert.Throws<ArgumentNullException>(() => new DataLicenseInputReader(fakeOptions, fakeInstruments, _mockDataLicenseService, _mockPerSecurityWsFactory, _mockTransformerFactory).Read());
        }

        [Test]
        public void ThrowsIfNoCorpActions()
        {
            var fakeOptions = new GetActionsOptions();

            var fakeInstrument = new Instrument
            {
                id = "fakeinstrument"
            };
            var fakeInstruments = new Instruments
            {
                instrument = new Instrument[] { fakeInstrument }
            };

            var fakeInstrumentResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "someHeader", "someValue" }
                }
            };

            var fakeResponse = new PerSecurityResponse
            {
                requestId = "output"
            };

            Mock.Get(_mockDataLicenseService)
                .Setup(mock => mock.Get(
                    It.Is<IDataLicenseCall<PerSecurityResponse>>(c => c is GetActionsDataLicenseCall),
                    It.Is<Instruments>(i => i.instrument.Contains(fakeInstrument)),
                    It.IsAny<ProgramTypes>(),
                    It.IsAny<bool>()))
                .Returns(fakeResponse);

            Assert.Throws<ArgumentNullException>(() => new DataLicenseInputReader(fakeOptions, fakeInstruments, _mockDataLicenseService, _mockPerSecurityWsFactory, _mockTransformerFactory).Read());
        }

        [Test]
        public void SafetyOffIsPassedThrough()
        {
            var fakeOptions = new GetDataOptions
            {
                DataFields = new List<string>(),
                EnableLiveRequests = false
            };

            var fakeInstrument = new Instrument
            {
                id = "fakeinstrument"
            };
            var fakeInstruments = new Instruments
            {
                instrument = new Instrument[] { fakeInstrument }
            };

            var fakeResponse = new PerSecurityResponse
            {
                requestId = "output"
            };

            var fakeDictionaryResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "someHeader", "someValue" }
                }
            };

            Mock.Get(_mockDataLicenseService)
                .Setup(mock => mock.Get(
                    It.Is<IDataLicenseCall<PerSecurityResponse>>(c => c is GetDataLicenseCall),
                    It.Is<Instruments>(i => i.instrument.Contains(fakeInstrument)),
                    It.IsAny<ProgramTypes>(),
                    It.Is<bool>(b => !b)))
                .Returns(fakeResponse);

            Mock.Get(_mockTransformerFactory)
                .Setup(mock => mock.Build(It.Is<DataTypes>(d => d.Equals(DataTypes.GetData))))
                .Returns(_mockTransformer);

            Mock.Get(_mockTransformer)
                .Setup(mock => mock.Transform(It.IsAny<PerSecurityResponse>()))
                .Returns(fakeDictionaryResponse);

            var output = new DataLicenseInputReader(fakeOptions, fakeInstruments, _mockDataLicenseService, _mockPerSecurityWsFactory, _mockTransformerFactory).Read();

            var fakeInstrumentResponse = new List<IRecord>
            {
                new InstrumentDataRecord(
                    new Dictionary<string, string>
                    {
                        { "someHeader", "someValue" }
                    }
                )
            };

            var fakeOutput = new DataLicenseOutput("output", fakeInstrumentResponse);

            Assert.That(output.Id, Is.EqualTo(fakeOutput.Id));
            Assert.That(output.DataRecords.Count, Is.EqualTo(fakeOutput.DataRecords.Count));
            Assert.That(output.CorporateActionRecords, Is.Empty);
        }

        [Test]
        public void SafetyOnIsPassedThrough()
        {
            var fakeOptions = new GetDataOptions
            {
                DataFields = new List<string>(),
                EnableLiveRequests = true
            };

            var fakeInstrument = new Instrument
            {
                id = "fakeinstrument"
            };
            var fakeInstruments = new Instruments
            {
                instrument = new Instrument[] { fakeInstrument }
            };

            var fakeResponse = new PerSecurityResponse
            {
                requestId = "output"
            };

            var fakeDictionaryResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "someHeader", "someValue" }
                }
            };

            Mock.Get(_mockDataLicenseService)
                .Setup(mock => mock.Get(
                    It.Is<IDataLicenseCall<PerSecurityResponse>>(c => c is GetDataLicenseCall),
                    It.Is<Instruments>(i => i.instrument.Contains(fakeInstrument)),
                    It.IsAny<ProgramTypes>(),
                    It.Is<bool>(b => b)))
                .Returns(fakeResponse);

            Mock.Get(_mockTransformerFactory)
                .Setup(mock => mock.Build(It.Is<DataTypes>(d => d.Equals(DataTypes.GetData))))
                .Returns(_mockTransformer);

            Mock.Get(_mockTransformer)
                .Setup(mock => mock.Transform(It.IsAny<PerSecurityResponse>()))
                .Returns(fakeDictionaryResponse);

            var output = new DataLicenseInputReader(fakeOptions, fakeInstruments, _mockDataLicenseService, _mockPerSecurityWsFactory, _mockTransformerFactory).Read();

            var fakeInstrumentResponse = new List<IRecord>
            {
                new InstrumentDataRecord(
                    new Dictionary<string, string>
                    {
                        { "someHeader", "someValue" }
                    }
                )
            };

            var fakeOutput = new DataLicenseOutput("output", fakeInstrumentResponse);

            Assert.That(output.Id, Is.EqualTo(fakeOutput.Id));
            Assert.That(output.DataRecords.Count, Is.EqualTo(fakeOutput.DataRecords.Count));
            Assert.That(output.CorporateActionRecords, Is.Empty);
        }
    }
}
