using Lusid.Sdk.Utilities;
using Lusid.FinDataEx.Output;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;
using Lusid.FinDataEx.Output.OutputInterpreter;
using Lusid.Sdk.Api;
using Lusid.FinDataEx.Util.InterpreterUtils;
using Lusid.Sdk.Model;

namespace Lusid.FinDataEx.Tests.Unit.Output
{
    [TestFixture]
    public class LusidTenantOutputWriterTests
    {
        private IInterpreterFactory _mockInterpreterFactory;
        private IOutputInterpreter _mockInterpreter;
        private ILusidApiFactory _mockLusidApiFactory;
        private ICorporateActionSourcesApi _mockCorporateActionSourcesApi;

        [SetUp]
        public void SetUp()
        {
            _mockInterpreterFactory = Mock.Of<IInterpreterFactory>();
            _mockInterpreter = Mock.Of<IOutputInterpreter>();
            _mockLusidApiFactory = Mock.Of<ILusidApiFactory>();
            _mockCorporateActionSourcesApi = Mock.Of<ICorporateActionSourcesApi>();

            Mock.Get(_mockInterpreterFactory)
                .Setup(mock => mock.Build(It.IsAny<InterpreterType>(), It.IsAny<GetActionsOptions>()))
                .Returns(_mockInterpreter);

            Mock.Get(_mockInterpreter)
                .Setup(mock => mock.Interpret(It.IsAny<DataLicenseOutput>()))
                .Returns(Mock.Of<List<UpsertCorporateActionRequest>>());

            Mock.Get(_mockLusidApiFactory)
                .Setup(mock => mock.Api<ICorporateActionSourcesApi>())
                .Returns(_mockCorporateActionSourcesApi);
        }

        [Test]
        public void ValidDataConstructsValidRequest()
        {
            Mock.Get(_mockCorporateActionSourcesApi)
                .Setup(mock => mock.BatchUpsertCorporateActions(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<UpsertCorporateActionRequest>>()))
                .Returns(new UpsertCorporateActionsResponse { Failed = new Dictionary<string, ErrorDetail>() });

            var id = "unique id";
            var headers = new List<string> { "h1", "h2", "h3" };
            var records = new List<Dictionary<string, string>>();
            var fakeData = new DataLicenseOutput(id, headers, records);

            var fakeOptions = new GetActionsOptions
            {
                OutputPath = "scope:code",
                CorpActionTypes = new List<CorpActionType> { CorpActionType.DVD_CASH }
            };

            var writeResult = new LusidTenantOutputWriter(fakeOptions, _mockLusidApiFactory, _mockInterpreterFactory).Write(fakeData);

            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            Assert.That(writeResult.FileOutputPath, Is.EqualTo("scope:code"));
        }

        [Test]
        public void FailsWhenNotProcessingActions()
        {
            var id = "unique id";
            var headers = new List<string> { "h1", "h2", "h3" };
            var records = new List<Dictionary<string, string>>();
            var fakeData = new DataLicenseOutput(id, headers, records);

            var fakeOptions = new GetDataOptions
            {
                DataFields = new List<string>()
            };

            var fakeLusidApiFactory = Mock.Of<ILusidApiFactory>();
            var fakeInterpreterFactory = Mock.Of<IInterpreterFactory>();
            var writeResult = new LusidTenantOutputWriter(fakeOptions, fakeLusidApiFactory, fakeInterpreterFactory).Write(fakeData);

            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.NotRun));
        }

        [Test]
        public void FailsWhenActionTypeNotCashDividend()
        {
            var id = "unique id";
            var headers = new List<string> { "h1", "h2", "h3" };
            var records = new List<Dictionary<string, string>>();
            var fakeData = new DataLicenseOutput(id, headers, records);

            var fakeOptions = new GetActionsOptions
            {
                CorpActionTypes = new List<CorpActionType> { CorpActionType.STOCK_SPLT }
            };

            var fakeLusidApiFactory = Mock.Of<ILusidApiFactory>();
            var fakeInterpreterFactory = Mock.Of<IInterpreterFactory>();
            var writeResult = new LusidTenantOutputWriter(fakeOptions, fakeLusidApiFactory, fakeInterpreterFactory).Write(fakeData);

            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.NotRun));
        }

        [Test]
        public void FailsWhenActionTypeNotUniquelyCashDividend()
        {
            var id = "unique id";
            var headers = new List<string> { "h1", "h2", "h3" };
            var records = new List<Dictionary<string, string>>();
            var fakeData = new DataLicenseOutput(id, headers, records);

            var fakeOptions = new GetActionsOptions
            {
                CorpActionTypes = new List<CorpActionType> { CorpActionType.DVD_CASH, CorpActionType.STOCK_SPLT }
            };

            var fakeLusidApiFactory = Mock.Of<ILusidApiFactory>();
            var fakeInterpreterFactory = Mock.Of<IInterpreterFactory>();
            var writeResult = new LusidTenantOutputWriter(fakeOptions, fakeLusidApiFactory, fakeInterpreterFactory).Write(fakeData);

            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.NotRun));
        }

        [Test]
        public void FailsWhenDataRecordsAreEmpty()
        {
            var fakeData = DataLicenseOutput.Empty("request returned no data");

            var fakeOptions = new GetActionsOptions
            {
                CorpActionTypes = new List<CorpActionType> { CorpActionType.DVD_CASH }
            };

            var fakeLusidApiFactory = Mock.Of<ILusidApiFactory>();
            var fakeInterpreterFactory = Mock.Of<IInterpreterFactory>();
            var writeResult = new LusidTenantOutputWriter(fakeOptions, fakeLusidApiFactory, fakeInterpreterFactory).Write(fakeData);

            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.NotRun));
        }

        [Test]
        public void FailsOnInvalidData()
        {
            Mock.Get(_mockCorporateActionSourcesApi)
                .Setup(mock => mock.BatchUpsertCorporateActions(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<UpsertCorporateActionRequest>>()))
                .Returns(new UpsertCorporateActionsResponse { Failed = new Dictionary<string, ErrorDetail> { { "thing", new ErrorDetail() } } });

            var id = "unique id";
            var headers = new List<string> { "h1", "h2", "h3" };
            var records = new List<Dictionary<string, string>>();
            var fakeData = new DataLicenseOutput(id, headers, records);

            var fakeOptions = new GetActionsOptions
            {
                OutputPath = "scope:code",
                CorpActionTypes = new List<CorpActionType> { CorpActionType.DVD_CASH }
            };

            var writeResult = new LusidTenantOutputWriter(fakeOptions, _mockLusidApiFactory, _mockInterpreterFactory).Write(fakeData);

            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Fail));
        }
    }
}
