using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Call;
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

        [SetUp]
        public void SetUp()
        {
            _mockDataLicenseService = Mock.Of<IDataLicenseService>();
            _mockPerSecurityWsFactory = Mock.Of<IPerSecurityWsFactory>();

            Mock.Get(_mockPerSecurityWsFactory)
                .Setup(mock => mock.CreateDefault())
                .Returns(new PerSecurityWSClient(new BasicHttpBinding(), new EndpointAddress("http://example.test/not/real/endpoint")));
        }

        [Test]
        public void GetDataResponse()
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

            var fakeInstrumentResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "someHeader", "someValue" }
                }
            };

            var fakeOutput = new DataLicenseOutput("output", new List<string>(), fakeInstrumentResponse);

            Mock.Get(_mockDataLicenseService)
                .Setup(mock => mock.Get(
                    It.Is<IDataLicenseCall<PerSecurityResponse>>(c => c is GetDataLicenseCall),
                    It.Is<Instruments>(i => i.instrument.Contains(fakeInstrument)),
                    It.IsAny<ProgramTypes>(),
                    It.IsAny<bool>()))
                .Returns(fakeOutput);

            var output = new DataLicenseInputReader(fakeOptions, fakeInstruments, _mockDataLicenseService, _mockPerSecurityWsFactory).Read();

            Assert.That(output.Id, Is.EqualTo("output"));
            Assert.That(output.Records, Is.Not.Empty);
        }

        [Test]
        public void GetActionsResponse()
        {
            var fakeOptions = new GetActionsOptions
            {
                CorpActionTypes = new List<CorpActionType> { CorpActionType.DVD_CASH }
            };

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

            var fakeOutput = new DataLicenseOutput("output", new List<string>(), fakeInstrumentResponse);

            Mock.Get(_mockDataLicenseService)
                .Setup(mock => mock.Get(
                    It.Is<IDataLicenseCall<PerSecurityResponse>>(c => c is GetActionsDataLicenseCall),
                    It.Is<Instruments>(i => i.instrument.Contains(fakeInstrument)),
                    It.IsAny<ProgramTypes>(),
                    It.IsAny<bool>()))
                .Returns(fakeOutput);

            var output = new DataLicenseInputReader(fakeOptions, fakeInstruments, _mockDataLicenseService, _mockPerSecurityWsFactory).Read();

            Assert.That(output.Id, Is.EqualTo("output"));
            Assert.That(output.Records, Is.Not.Empty);
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

            var fakeInstrumentResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "someHeader", "someValue" }
                }
            };

            var fakeOutput = new DataLicenseOutput("output", new List<string>(), fakeInstrumentResponse);

            Mock.Get(_mockDataLicenseService)
                .Setup(mock => mock.Get(
                    It.Is<IDataLicenseCall<PerSecurityResponse>>(c => c is GetDataLicenseCall),
                    It.Is<Instruments>(i => i.instrument.Contains(fakeInstrument)),
                    It.IsAny<ProgramTypes>(),
                    It.IsAny<bool>()))
                .Returns(fakeOutput);

            Assert.Throws<ArgumentNullException>(() => new DataLicenseInputReader(fakeOptions, fakeInstruments, _mockDataLicenseService, _mockPerSecurityWsFactory).Read());
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

            var fakeOutput = new DataLicenseOutput("output", new List<string>(), fakeInstrumentResponse);

            Mock.Get(_mockDataLicenseService)
                .Setup(mock => mock.Get(
                    It.Is<IDataLicenseCall<PerSecurityResponse>>(c => c is GetActionsDataLicenseCall),
                    It.Is<Instruments>(i => i.instrument.Contains(fakeInstrument)),
                    It.IsAny<ProgramTypes>(),
                    It.IsAny<bool>()))
                .Returns(fakeOutput);

            Assert.Throws<ArgumentNullException>(() => new DataLicenseInputReader(fakeOptions, fakeInstruments, _mockDataLicenseService, _mockPerSecurityWsFactory).Read());
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

            var fakeInstrumentResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "someHeader", "someValue" }
                }
            };

            var fakeOutput = new DataLicenseOutput("output", new List<string>(), fakeInstrumentResponse);

            Mock.Get(_mockDataLicenseService)
                .Setup(mock => mock.Get(
                    It.Is<IDataLicenseCall<PerSecurityResponse>>(c => c is GetDataLicenseCall),
                    It.Is<Instruments>(i => i.instrument.Contains(fakeInstrument)),
                    It.IsAny<ProgramTypes>(),
                    It.Is<bool>(b => !b)))
                .Returns(fakeOutput);

            var output = new DataLicenseInputReader(fakeOptions, fakeInstruments, _mockDataLicenseService, _mockPerSecurityWsFactory).Read();

            Assert.That(output.Id, Is.EqualTo("output"));
            Assert.That(output.Records, Is.Not.Empty);
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

            var fakeInstrumentResponse = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "someHeader", "someValue" }
                }
            };

            var fakeOutput = new DataLicenseOutput("output", new List<string>(), fakeInstrumentResponse);

            Mock.Get(_mockDataLicenseService)
                .Setup(mock => mock.Get(
                    It.Is<IDataLicenseCall<PerSecurityResponse>>(c => c is GetDataLicenseCall),
                    It.Is<Instruments>(i => i.instrument.Contains(fakeInstrument)),
                    It.IsAny<ProgramTypes>(),
                    It.Is<bool>(b => b)))
                .Returns(fakeOutput);

            var output = new DataLicenseInputReader(fakeOptions, fakeInstruments, _mockDataLicenseService, _mockPerSecurityWsFactory).Read();

            Assert.That(output.Id, Is.EqualTo("output"));
            Assert.That(output.Records, Is.Not.Empty);
        }

        // throw when no datafield
        // throw when no action types
    }
}
