using System;
using System.Collections.Generic;
using Lusid.FinDataEx.DataLicense.Service.Instrument;
using Lusid.Sdk.Api;
using Lusid.Sdk.Model;
using Lusid.Sdk.Utilities;
using NUnit.Framework;
using PerSecurity_Dotnet;
using InstrumentType = PerSecurity_Dotnet.InstrumentType;

namespace Lusid.FinDataEx.Tests.Integration.DataLicence.Instrument
{
    [TestFixture]
    public class LusidPortfolioInstrumentSourceTests
    {

        private LusidPortfolioInstrumentSource _instrumentSource;

        private ILusidApiFactory lusidApiFactory;
        private string scope = "findataex-dl-instrument-source-test";
        private string portfolio = "port_01";
        private string amznFigi = "BBG000BVPV84";
        private string micFigi = "BBG000BPHFS9";
        private DateTimeOffset effectiveAt = new DateTimeOffset(2020, 06, 30, 0, 0, 0, TimeSpan.Zero);
        private DateTimeOffset effectiveAtTMinus1 = new DateTimeOffset(2020, 06, 29, 0, 0, 0, TimeSpan.Zero);

        [SetUp]
        public void SetUp()
        {
            // Create a portfolio and upsert transactions in 
            lusidApiFactory = LusidApiFactoryBuilder.Build("secrets_api.json");
            var transactionPortfoliosApi = lusidApiFactory.Api<TransactionPortfoliosApi>();
            CreateTransactionPortfolio(transactionPortfoliosApi);
            UpsertTestTransactions(transactionPortfoliosApi);
            
            _instrumentSource = new LusidPortfolioInstrumentSource(scope, portfolio, effectiveAt);
        }
        
        [TearDown]
        public void TearDown()
        {
            lusidApiFactory.Api<PortfoliosApi>().DeletePortfolio(scope, portfolio);
        }

        [Test]
        public void Get_OnValidScopeAndPortfolio_ShouldReturnHoldingInstrumentsAsDlInstruments()
        {
            Instruments instruments = _instrumentSource.Get();
            Assert.That(instruments.instrument[0].id, Is.EqualTo("BBG000BVPV84"));
            Assert.That(instruments.instrument[0].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
            Assert.That(instruments.instrument[1].id, Is.EqualTo("BBG000BPHFS9"));
            Assert.That(instruments.instrument[1].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
        }
        
        private string CreateTransactionPortfolio(TransactionPortfoliosApi transactionPortfoliosApi)
        {
            var portfolioEffectiveDate = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var request = new CreateTransactionPortfolioRequest(
                code: $"{portfolio}",
                displayName: $"Portfolio-{portfolio}",                 
                baseCurrency: "USD",
                created: portfolioEffectiveDate
            );
            var portfolioCreated = transactionPortfoliosApi.CreatePortfolio(scope, request);
            return portfolioCreated.Id.Code;
        }

        private void UpsertTestTransactions(TransactionPortfoliosApi transactionPortfoliosApi)
        {
            List<TransactionRequest> transactions = new List<TransactionRequest>()
            {
                BuildTransactionRequest(amznFigi, 100, 3292.01M, "USD", effectiveAtTMinus1, "Buy"),
                BuildTransactionRequest(micFigi, 500, 217.10M, "USD", effectiveAtTMinus1, "Buy")
            };
            transactionPortfoliosApi.UpsertTransactions(scope, portfolio, transactions);
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