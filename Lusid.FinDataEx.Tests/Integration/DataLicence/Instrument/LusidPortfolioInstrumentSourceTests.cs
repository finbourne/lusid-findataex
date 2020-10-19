using System;
using System.Collections.Generic;
using Lusid.FinDataEx.DataLicense.Service.Instrument;
using Lusid.Sdk.Api;
using Lusid.Sdk.Model;
using Lusid.Sdk.Utilities;
using NUnit.Framework;
using InstrumentType = PerSecurity_Dotnet.InstrumentType;

namespace Lusid.FinDataEx.Tests.Integration.DataLicence.Instrument
{
    [TestFixture]
    public class LusidPortfolioInstrumentSourceTests
    {
        
        private ILusidApiFactory lusidApiFactory;
        private string scope = "findataex-dl-instrument-source-test";
        private string portfolio = "port_01";
        private string portfolio2 = "port_02";
        private string portfolioNoHoldings = "port_no_holding";
        private string portfolioSameHoldingAsP1 = "port_same_holdings_as_p1";
        private string amznFigi = "BBG000BVPV84";
        private string micFigi = "BBG000BPHFS9";
        private DateTimeOffset effectiveAt = new DateTimeOffset(2020, 06, 30, 0, 0, 0, TimeSpan.Zero);
        private DateTimeOffset effectiveAtTMinus1 = new DateTimeOffset(2020, 06, 29, 0, 0, 0, TimeSpan.Zero);

        [SetUp]
        public void SetUp()
        {
            lusidApiFactory = LusidApiFactoryBuilder.Build("secrets_api.json");
            var transactionPortfoliosApi = lusidApiFactory.Api<TransactionPortfoliosApi>();
            
            // Create Portfolio with AMZN
            CreateTransactionPortfolio(transactionPortfoliosApi, portfolio);
            UpsertTestTransactionsAmzn(transactionPortfoliosApi, portfolio);
            CreateTransactionPortfolio(transactionPortfoliosApi, portfolioSameHoldingAsP1);
            UpsertTestTransactionsAmzn(transactionPortfoliosApi, portfolioSameHoldingAsP1);
            // Create Portfolio with MSFT
            CreateTransactionPortfolio(transactionPortfoliosApi, portfolio2);
            UpsertTestTransactionsMSFT(transactionPortfoliosApi, portfolio2);

            // Create a portfolio with no holdings
            CreateTransactionPortfolio(transactionPortfoliosApi, portfolioNoHoldings);
        }
        
        [TearDown]
        public void TearDown()
        {
            lusidApiFactory.Api<PortfoliosApi>().DeletePortfolio(scope, portfolio);
            lusidApiFactory.Api<PortfoliosApi>().DeletePortfolio(scope, portfolioSameHoldingAsP1);
            lusidApiFactory.Api<PortfoliosApi>().DeletePortfolio(scope, portfolio2);
            lusidApiFactory.Api<PortfoliosApi>().DeletePortfolio(scope, portfolioNoHoldings);
        }

        [Test]
        public void Get_OnPortfoliosWithAndWithoutHoldings_ShouldDlInstruments()
        {
            var scopesAndPortfolios = new HashSet<Tuple<string,string>>()
            {
                new Tuple<string, string>(scope, portfolio),
                new Tuple<string, string>(scope, portfolio2),
                new Tuple<string, string>(scope, portfolioNoHoldings),
            };
            
            var instrumentSource = new LusidPortfolioInstrumentSource(scopesAndPortfolios, effectiveAt);
            var instruments = instrumentSource.Get();
            
            Assert.That(instruments.instrument.Length, Is.EqualTo(2));
            Assert.That(instruments.instrument[0].id, Is.EqualTo("BBG000BVPV84"));
            Assert.That(instruments.instrument[0].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
            Assert.That(instruments.instrument[1].id, Is.EqualTo("BBG000BPHFS9"));
            Assert.That(instruments.instrument[1].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
        }
        
        [Test]
        public void Get_OnPortfoliosWithOneThatDoesNotExist_ShouldReturnDlInstrumentsForExistingPortfolio()
        {
            var scopesAndPortfolios = new HashSet<Tuple<string,string>>()
            {
                new Tuple<string, string>(scope, portfolio),
                new Tuple<string, string>(scope, "portfolio_does_not_exist"),
            };
            
            var instrumentSource = new LusidPortfolioInstrumentSource(scopesAndPortfolios, effectiveAt);
            var instruments = instrumentSource.Get();
            
            Assert.That(instruments.instrument.Length, Is.EqualTo(1));
            Assert.That(instruments.instrument[0].id, Is.EqualTo("BBG000BVPV84"));
            Assert.That(instruments.instrument[0].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
        }

        [Test]
        public void Get_OnPortfoliosWithSameInstrumentHoldings_ShouldReturnOnlyOneOfDlInstruments()
        {
            var scopesAndPortfolios = new HashSet<Tuple<string, string>>()
            {
                new Tuple<string, string>(scope, portfolio),
                new Tuple<string, string>(scope, portfolioSameHoldingAsP1),
            };

            var instrumentSource = new LusidPortfolioInstrumentSource(scopesAndPortfolios, effectiveAt);
            var instruments = instrumentSource.Get();

            Assert.That(instruments.instrument.Length, Is.EqualTo(1));
            Assert.That(instruments.instrument[0].id, Is.EqualTo("BBG000BVPV84"));
            Assert.That(instruments.instrument[0].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
        }

        [Test]
        public void Get_OnPortfoliosWithoutHoldings_ShouldReturnNull()
        {
            var scopesAndPortfolios = new HashSet<Tuple<string,string>>()
            {
                new Tuple<string, string>(scope, portfolioNoHoldings)
            };
            
            var instrumentSource = new LusidPortfolioInstrumentSource(scopesAndPortfolios, effectiveAt);
            var instruments = instrumentSource.Get();
            
            Assert.IsNull(instruments);
        }
        
        [Test]
        public void Get_OnPortfoliosDoNotExist_ShouldReturnNull()
        {
            var scopesAndPortfolios = new HashSet<Tuple<string,string>>()
            {
                new Tuple<string, string>(scope, "portfolio_does_not_exist"),
                new Tuple<string, string>(scope, "neither_does_this_portfolio")
            };
            
            var instrumentSource = new LusidPortfolioInstrumentSource(scopesAndPortfolios, effectiveAt);
            var instruments = instrumentSource.Get();
            
            Assert.IsNull(instruments);
        }
        
        private string CreateTransactionPortfolio(TransactionPortfoliosApi transactionPortfoliosApi, string portfolioCode)
        {
            var portfolioEffectiveDate = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var request = new CreateTransactionPortfolioRequest(
                code: $"{portfolioCode}",
                displayName: $"Portfolio-{portfolio}",                 
                baseCurrency: "USD",
                created: portfolioEffectiveDate
            );
            var portfolioCreated = transactionPortfoliosApi.CreatePortfolio(scope, request);
            return portfolioCreated.Id.Code;
        }

        private void UpsertTestTransactionsMSFT(TransactionPortfoliosApi transactionPortfoliosApi, string transactionPortfolio)
        {
            List<TransactionRequest> transactions = new List<TransactionRequest>()
            {
                BuildTransactionRequest(micFigi, 500, 217.10M, "USD", effectiveAtTMinus1, "Buy")
            };
            transactionPortfoliosApi.UpsertTransactions(scope, transactionPortfolio, transactions);
        }
        private void UpsertTestTransactionsAmzn(TransactionPortfoliosApi transactionPortfoliosApi, string transactionPortfolio)
        {
            List<TransactionRequest> transactions = new List<TransactionRequest>()
            {
                BuildTransactionRequest(amznFigi, 100, 3292.01M, "USD", effectiveAtTMinus1, "Buy"),
            };
            transactionPortfoliosApi.UpsertTransactions(scope, transactionPortfolio, transactions);
        }
        
        private TransactionRequest BuildTransactionRequest(
            string figiInstrumentId,
            decimal units, 
            decimal price,
            string currency,
            DateTimeOffset tradeDate, 
            string transactionType)
        {
            return new TransactionRequest(
                transactionId: Guid.NewGuid().ToString(),
                type: transactionType,
                instrumentIdentifiers: new Dictionary<string, string>
                {
                    ["Instrument/default/Figi"] = figiInstrumentId
                },
                transactionDate: tradeDate,
                settlementDate: tradeDate,
                units: units,
                transactionPrice: new TransactionPrice(price, TransactionPrice.TypeEnum.Price),
                totalConsideration: new CurrencyAndAmount(price*units, currency),
                source: "Broker");
        }
        
        

    }
}