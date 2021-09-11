using Lusid.FinDataEx.DataLicense.Service.Instrument;
using Lusid.Sdk;
using Lusid.Sdk.Api;
using Lusid.Sdk.Model;
using Lusid.Sdk.Utilities;
using Moq;
using NUnit.Framework;
using PerSecurity_Dotnet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lusid.FinDataEx.Tests.Unit.Input.InputReader.InstrumentSource
{
    [TestFixture]
    public class LusidPortoflioInstrumentSourceTests
    {
        private ILusidApiFactory _mockLusidApiFactory;
        private ITransactionPortfoliosApi _mockTransactionPortfoliosApi;

        [SetUp]
        public void SetUp()
        {
            _mockLusidApiFactory = Mock.Of<ILusidApiFactory>();

            _mockTransactionPortfoliosApi = Mock.Of<ITransactionPortfoliosApi>();

            Mock.Get(_mockLusidApiFactory)
                .Setup(mock => mock.Api<ITransactionPortfoliosApi>())
                .Returns(_mockTransactionPortfoliosApi);
        }

        [Test]
        public void SingleInstrumentSinglePortfolio()
        {
            var fakePortfolios = new List<string> { "portfolio1|scope1" };
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
                                new PropertyValue("instrument1")
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
            var fakeData = new VersionedResourceListOfPortfolioHolding(new Sdk.Model.Version(DateTimeOffset.MinValue, DateTimeOffset.MaxValue), fakeHoldings, null, null);

            Mock.Get(_mockTransactionPortfoliosApi)
                .Setup(mock => mock.GetHoldings(
                    It.Is<string>(s => s.Equals("scope1")),
                    It.Is<string>(s => s.Equals("portfolio1")),
                    It.IsAny<DateTimeOrCutLabel>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<bool?>()))
                .Returns(fakeData);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            var output = LusidPortfolioInstrumentSource.Create(_mockLusidApiFactory, instrumentArgs, fakePortfolios).Get();
            var outputInstrument = output.instrument.Single();

            Assert.That(outputInstrument.id, Is.EqualTo("instrument1"));
            Assert.That(outputInstrument.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument.typeSpecified, Is.True);
            Assert.That(outputInstrument.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument.yellowkey, Is.EqualTo(MarketSector.Govt));
        }

        [Test]
        public void InstrumentsFilteredByType()
        {
            var fakePortfolios = new List<string> { "portfolio1|scope1" };
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
                                new PropertyValue("instrument1")
                            )
                        }
                    },
                    "dummyHolding",
                    0,
                    0,
                    new CurrencyAndAmount(0, "USD"),
                    new CurrencyAndAmount(0, "CCY_USD")
               ),
               new PortfolioHolding(
                    "instrument2Luid",
                    null,
                    new Dictionary<string, Property>
                    {
                        {
                            "Instrument/default/Figi",
                            new Property(
                                "Instrument/default/Figi",
                                new PropertyValue("instrument2")
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
            var fakeData = new VersionedResourceListOfPortfolioHolding(new Sdk.Model.Version(DateTimeOffset.MinValue, DateTimeOffset.MaxValue), fakeHoldings, null, null);

            Mock.Get(_mockTransactionPortfoliosApi)
                .Setup(mock => mock.GetHoldings(
                    It.Is<string>(s => s.Equals("scope1")),
                    It.Is<string>(s => s.Equals("portfolio1")),
                    It.IsAny<DateTimeOrCutLabel>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<bool?>()))
                .Returns(fakeData);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            var output = LusidPortfolioInstrumentSource.Create(_mockLusidApiFactory, instrumentArgs, fakePortfolios).Get();
            var outputInstrument = output.instrument.Single();

            Assert.That(outputInstrument.id, Is.EqualTo("instrument1"));
            Assert.That(outputInstrument.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument.typeSpecified, Is.True);
            Assert.That(outputInstrument.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument.yellowkey, Is.EqualTo(MarketSector.Govt));
        }

        [Test]
        public void SingleInstrumentMultiplePortfolio()
        {
            var fakePortfolios = new List<string> { "portfolio1|scope1", "portfolio2|scope2" };
            var fakeHoldingsPortfolio1 = new List<PortfolioHolding>
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
                                new PropertyValue("instrument1")
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
            var fakeDataPortfolio1 = new VersionedResourceListOfPortfolioHolding(new Sdk.Model.Version(DateTimeOffset.MinValue, DateTimeOffset.MaxValue), fakeHoldingsPortfolio1, null, null);

            var fakeHoldingsPortfolio2 = new List<PortfolioHolding>
            {
                new PortfolioHolding(
                    "instrument2Luid",
                    null,
                    new Dictionary<string, Property>
                    {
                        {
                            "Instrument/default/Isin",
                            new Property(
                                "Instrument/default/Isin",
                                new PropertyValue("instrument2")
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
            var fakeDataPortfolio2 = new VersionedResourceListOfPortfolioHolding(new Sdk.Model.Version(DateTimeOffset.MinValue, DateTimeOffset.MaxValue), fakeHoldingsPortfolio2, null, null);

            Mock.Get(_mockTransactionPortfoliosApi)
                .Setup(mock => mock.GetHoldings(
                    It.Is<string>(s => s.Equals("scope1")),
                    It.Is<string>(s => s.Equals("portfolio1")),
                    It.IsAny<DateTimeOrCutLabel>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<bool?>()))
                .Returns(fakeDataPortfolio1);

            Mock.Get(_mockTransactionPortfoliosApi)
                .Setup(mock => mock.GetHoldings(
                    It.Is<string>(s => s.Equals("scope2")),
                    It.Is<string>(s => s.Equals("portfolio2")),
                    It.IsAny<DateTimeOrCutLabel>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<bool?>()))
                .Returns(fakeDataPortfolio2);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            var output = LusidPortfolioInstrumentSource.Create(_mockLusidApiFactory, instrumentArgs, fakePortfolios).Get();
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
        public void MultipleInstrumentSinglePortfolio()
        {
            var fakePortfolios = new List<string> { "portfolio1|scope1" };
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
                                new PropertyValue("instrument1")
                            )
                        }
                    },
                    "dummyHolding",
                    0,
                    0,
                    new CurrencyAndAmount(0, "USD"),
                    new CurrencyAndAmount(0, "CCY_USD")
               ),
                new PortfolioHolding(
                    "instrument2Luid",
                    null,
                    new Dictionary<string, Property>
                    {
                        {
                            "Instrument/default/Isin",
                            new Property(
                                "Instrument/default/Isin",
                                new PropertyValue("instrument2")
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
            var fakeData = new VersionedResourceListOfPortfolioHolding(new Sdk.Model.Version(DateTimeOffset.MinValue, DateTimeOffset.MaxValue), fakeHoldings, null, null);

            Mock.Get(_mockTransactionPortfoliosApi)
                .Setup(mock => mock.GetHoldings(
                    It.Is<string>(s => s.Equals("scope1")),
                    It.Is<string>(s => s.Equals("portfolio1")),
                    It.IsAny<DateTimeOrCutLabel>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<bool?>()))
                .Returns(fakeData);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            var output = LusidPortfolioInstrumentSource.Create(_mockLusidApiFactory, instrumentArgs, fakePortfolios).Get();
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
        public void MultipleInstrumentMultiplePortfolio()
        {
            var fakePortfolios = new List<string> { "portfolio1|scope1", "portfolio2|scope2" };
            var fakeHoldingsPortfolio1 = new List<PortfolioHolding>
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
                                new PropertyValue("instrument1")
                            )
                        }
                    },
                    "dummyHolding",
                    0,
                    0,
                    new CurrencyAndAmount(0, "USD"),
                    new CurrencyAndAmount(0, "CCY_USD")
               ),
               new PortfolioHolding(
                    "instrument2Luid",
                    null,
                    new Dictionary<string, Property>
                    {
                        {
                            "Instrument/default/Isin",
                            new Property(
                                "Instrument/default/Isin",
                                new PropertyValue("instrument2")
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
            var fakeDataPortfolio1 = new VersionedResourceListOfPortfolioHolding(new Sdk.Model.Version(DateTimeOffset.MinValue, DateTimeOffset.MaxValue), fakeHoldingsPortfolio1, null, null);

            var fakeHoldingsPortfolio2 = new List<PortfolioHolding>
            {
                new PortfolioHolding(
                    "instrument3Luid",
                    null,
                    new Dictionary<string, Property>
                    {
                        {
                            "Instrument/default/Isin",
                            new Property(
                                "Instrument/default/Isin",
                                new PropertyValue("instrument3")
                            )
                        }
                    },
                    "dummyHolding",
                    0,
                    0,
                    new CurrencyAndAmount(0, "USD"),
                    new CurrencyAndAmount(0, "CCY_USD")
               ),
               new PortfolioHolding(
                    "instrument4Luid",
                    null,
                    new Dictionary<string, Property>
                    {
                        {
                            "Instrument/default/Isin",
                            new Property(
                                "Instrument/default/Isin",
                                new PropertyValue("instrument4")
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
            var fakeDataPortfolio2 = new VersionedResourceListOfPortfolioHolding(new Sdk.Model.Version(DateTimeOffset.MinValue, DateTimeOffset.MaxValue), fakeHoldingsPortfolio2, null, null);

            Mock.Get(_mockTransactionPortfoliosApi)
                .Setup(mock => mock.GetHoldings(
                    It.Is<string>(s => s.Equals("scope1")),
                    It.Is<string>(s => s.Equals("portfolio1")),
                    It.IsAny<DateTimeOrCutLabel>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<bool?>()))
                .Returns(fakeDataPortfolio1);

            Mock.Get(_mockTransactionPortfoliosApi)
                .Setup(mock => mock.GetHoldings(
                    It.Is<string>(s => s.Equals("scope2")),
                    It.Is<string>(s => s.Equals("portfolio2")),
                    It.IsAny<DateTimeOrCutLabel>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<bool?>()))
                .Returns(fakeDataPortfolio2);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            var output = LusidPortfolioInstrumentSource.Create(_mockLusidApiFactory, instrumentArgs, fakePortfolios).Get();

            Assert.That(output.instrument.Length, Is.EqualTo(4));

            var outputInstrument1 = output.instrument[0];

            Assert.That(outputInstrument1.id, Is.EqualTo("instrument1"));
            Assert.That(outputInstrument1.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument1.typeSpecified, Is.True);
            Assert.That(outputInstrument1.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument1.yellowkey, Is.EqualTo(MarketSector.Govt));

            var outputInstrument2 = output.instrument[1];

            Assert.That(outputInstrument2.id, Is.EqualTo("instrument2"));
            Assert.That(outputInstrument2.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument2.typeSpecified, Is.True);
            Assert.That(outputInstrument2.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument2.yellowkey, Is.EqualTo(MarketSector.Govt));

            var outputInstrument3 = output.instrument[2];

            Assert.That(outputInstrument3.id, Is.EqualTo("instrument3"));
            Assert.That(outputInstrument3.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument3.typeSpecified, Is.True);
            Assert.That(outputInstrument3.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument3.yellowkey, Is.EqualTo(MarketSector.Govt));

            var outputInstrument4 = output.instrument[3];

            Assert.That(outputInstrument4.id, Is.EqualTo("instrument4"));
            Assert.That(outputInstrument4.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument4.typeSpecified, Is.True);
            Assert.That(outputInstrument4.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument4.yellowkey, Is.EqualTo(MarketSector.Govt));
        }

        [Test]
        public void DuplicatesInPortfolioIgnored()
        {
            var fakePortfolios = new List<string> { "portfolio1|scope1" };
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
                                new PropertyValue("instrument1")
                            )
                        }
                    },
                    "dummyHolding",
                    0,
                    0,
                    new CurrencyAndAmount(0, "USD"),
                    new CurrencyAndAmount(0, "CCY_USD")
               ),
               new PortfolioHolding(
                    "instrument2Luid",
                    null,
                    new Dictionary<string, Property>
                    {
                        {
                            "Instrument/default/Isin",
                            new Property(
                                "Instrument/default/Isin",
                                new PropertyValue("instrument2")
                            )
                        }
                    },
                    "dummyHolding",
                    0,
                    0,
                    new CurrencyAndAmount(0, "USD"),
                    new CurrencyAndAmount(0, "CCY_USD")
               ),
               new PortfolioHolding(
                    "instrument1Luid",
                    null,
                    new Dictionary<string, Property>
                    {
                        {
                            "Instrument/default/Isin",
                            new Property(
                                "Instrument/default/Isin",
                                new PropertyValue("instrument1")
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
            var fakeData = new VersionedResourceListOfPortfolioHolding(new Sdk.Model.Version(DateTimeOffset.MinValue, DateTimeOffset.MaxValue), fakeHoldings, null, null);

            Mock.Get(_mockTransactionPortfoliosApi)
                .Setup(mock => mock.GetHoldings(
                    It.Is<string>(s => s.Equals("scope1")),
                    It.Is<string>(s => s.Equals("portfolio1")),
                    It.IsAny<DateTimeOrCutLabel>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<bool?>()))
                .Returns(fakeData);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            var output = LusidPortfolioInstrumentSource.Create(_mockLusidApiFactory, instrumentArgs, fakePortfolios).Get();

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
        public void DuplicatesAcrossPortfoliosIgnored()
        {
            var fakePortfolios = new List<string> { "portfolio1|scope1", "portfolio2|scope2" };
            var fakeHoldingsPortfolio1 = new List<PortfolioHolding>
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
                                new PropertyValue("instrument1")
                            )
                        }
                    },
                    "dummyHolding",
                    0,
                    0,
                    new CurrencyAndAmount(0, "USD"),
                    new CurrencyAndAmount(0, "CCY_USD")
               ),
               new PortfolioHolding(
                    "instrument2Luid",
                    null,
                    new Dictionary<string, Property>
                    {
                        {
                            "Instrument/default/Isin",
                            new Property(
                                "Instrument/default/Isin",
                                new PropertyValue("instrument2")
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
            var fakeDataPortfolio1 = new VersionedResourceListOfPortfolioHolding(new Sdk.Model.Version(DateTimeOffset.MinValue, DateTimeOffset.MaxValue), fakeHoldingsPortfolio1, null, null);

            var fakeHoldingsPortfolio2 = new List<PortfolioHolding>
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
                                new PropertyValue("instrument1")
                            )
                        }
                    },
                    "dummyHolding",
                    0,
                    0,
                    new CurrencyAndAmount(0, "USD"),
                    new CurrencyAndAmount(0, "CCY_USD")
               ),
               new PortfolioHolding(
                    "instrument3Luid",
                    null,
                    new Dictionary<string, Property>
                    {
                        {
                            "Instrument/default/Isin",
                            new Property(
                                "Instrument/default/Isin",
                                new PropertyValue("instrument3")
                            )
                        }
                    },
                    "dummyHolding",
                    0,
                    0,
                    new CurrencyAndAmount(0, "USD"),
                    new CurrencyAndAmount(0, "CCY_USD")
               ),
               new PortfolioHolding(
                    "instrument4Luid",
                    null,
                    new Dictionary<string, Property>
                    {
                        {
                            "Instrument/default/Isin",
                            new Property(
                                "Instrument/default/Isin",
                                new PropertyValue("instrument4")
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
            var fakeDataPortfolio2 = new VersionedResourceListOfPortfolioHolding(new Sdk.Model.Version(DateTimeOffset.MinValue, DateTimeOffset.MaxValue), fakeHoldingsPortfolio2, null, null);

            Mock.Get(_mockTransactionPortfoliosApi)
                .Setup(mock => mock.GetHoldings(
                    It.Is<string>(s => s.Equals("scope1")),
                    It.Is<string>(s => s.Equals("portfolio1")),
                    It.IsAny<DateTimeOrCutLabel>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<bool?>()))
                .Returns(fakeDataPortfolio1);

            Mock.Get(_mockTransactionPortfoliosApi)
                .Setup(mock => mock.GetHoldings(
                    It.Is<string>(s => s.Equals("scope2")),
                    It.Is<string>(s => s.Equals("portfolio2")),
                    It.IsAny<DateTimeOrCutLabel>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<bool?>()))
                .Returns(fakeDataPortfolio2);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            var output = LusidPortfolioInstrumentSource.Create(_mockLusidApiFactory, instrumentArgs, fakePortfolios).Get();

            Assert.That(output.instrument.Length, Is.EqualTo(4));

            var outputInstrument1 = output.instrument[0];

            Assert.That(outputInstrument1.id, Is.EqualTo("instrument1"));
            Assert.That(outputInstrument1.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument1.typeSpecified, Is.True);
            Assert.That(outputInstrument1.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument1.yellowkey, Is.EqualTo(MarketSector.Govt));

            var outputInstrument2 = output.instrument[1];

            Assert.That(outputInstrument2.id, Is.EqualTo("instrument2"));
            Assert.That(outputInstrument2.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument2.typeSpecified, Is.True);
            Assert.That(outputInstrument2.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument2.yellowkey, Is.EqualTo(MarketSector.Govt));

            var outputInstrument3 = output.instrument[2];

            Assert.That(outputInstrument3.id, Is.EqualTo("instrument3"));
            Assert.That(outputInstrument3.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument3.typeSpecified, Is.True);
            Assert.That(outputInstrument3.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument3.yellowkey, Is.EqualTo(MarketSector.Govt));

            var outputInstrument4 = output.instrument[3];

            Assert.That(outputInstrument4.id, Is.EqualTo("instrument4"));
            Assert.That(outputInstrument4.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument4.typeSpecified, Is.True);
            Assert.That(outputInstrument4.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument4.yellowkey, Is.EqualTo(MarketSector.Govt));
        }

        [Test]
        public void FailsOnExcessiveDelimeters()
        {
            var fakePortfolios = new List<string> { "portfolio1|scope1|thing1" };
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
                                new PropertyValue("instrument1")
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
            var fakeData = new VersionedResourceListOfPortfolioHolding(new Sdk.Model.Version(DateTimeOffset.MinValue, DateTimeOffset.MaxValue), fakeHoldings, null, null);

            Mock.Get(_mockTransactionPortfoliosApi)
                .Setup(mock => mock.GetHoldings(
                    It.Is<string>(s => s.Equals("scope1")),
                    It.Is<string>(s => s.Equals("portfolio1")),
                    It.IsAny<DateTimeOrCutLabel>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<bool?>()))
                .Returns(fakeData);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            Assert.Throws<ArgumentException>(() => LusidPortfolioInstrumentSource.Create(_mockLusidApiFactory, instrumentArgs, fakePortfolios).Get());
        }

        [Test]
        public void FailsOnTooFewDelimeters()
        {
            var fakePortfolios = new List<string> { "portfolio1" };
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
                                new PropertyValue("instrument1")
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
            var fakeData = new VersionedResourceListOfPortfolioHolding(new Sdk.Model.Version(DateTimeOffset.MinValue, DateTimeOffset.MaxValue), fakeHoldings, null, null);

            Mock.Get(_mockTransactionPortfoliosApi)
                .Setup(mock => mock.GetHoldings(
                    It.Is<string>(s => s.Equals("scope1")),
                    It.Is<string>(s => s.Equals("portfolio1")),
                    It.IsAny<DateTimeOrCutLabel>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<bool?>()))
                .Returns(fakeData);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            Assert.Throws<ArgumentException>(() => LusidPortfolioInstrumentSource.Create(_mockLusidApiFactory, instrumentArgs, fakePortfolios).Get());
        }

        [Test]
        public void DoesNotThrowOnMissingPortfolio()
        {
            var fakePortfolios = new List<string> { };
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
                                new PropertyValue("instrument1")
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
            var fakeData = new VersionedResourceListOfPortfolioHolding(new Sdk.Model.Version(DateTimeOffset.MinValue, DateTimeOffset.MaxValue), fakeHoldings, null, null);

            Mock.Get(_mockTransactionPortfoliosApi)
                .Setup(mock => mock.GetHoldings(
                    It.Is<string>(s => s.Equals("scope1")),
                    It.Is<string>(s => s.Equals("portfolio1")),
                    It.IsAny<DateTimeOrCutLabel>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<string>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<bool?>()))
                .Returns(fakeData);

            var options = new DataLicenseOptions
            {
                InstrumentIdType = PerSecurity_Dotnet.InstrumentType.ISIN,
            };

            var instrumentArgs = InstrumentArgs.Create(options);

            var output = LusidPortfolioInstrumentSource.Create(_mockLusidApiFactory, instrumentArgs, fakePortfolios).Get();

            Assert.That(output.instrument, Is.Empty);
        }
    }
}