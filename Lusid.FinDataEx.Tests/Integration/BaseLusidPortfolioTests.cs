using System;
using System.Collections.Generic;
using Lusid.Sdk.Api;
using Lusid.Sdk.Model;
using Lusid.Sdk.Utilities;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Integration
{
    [TestFixture]
    public class BaseLusidPortfolioTests
    {
        private ILusidApiFactory _lusidApiFactory;
        
        // test portfolio params
        protected string Scope = "findataex-dl-instrument-source-test";
        protected string Portfolio = "port_01";
        protected string Portfolio2 = "port_02";
        protected string PortfolioNoHoldings = "port_no_holding";
        protected string PortfolioSameHoldingAsP1 = "port_same_holdings_as_p1";
        protected string PortfolioWithUnknownInstrument = "port_unknown_instrument";
        protected DateTimeOffset EffectiveAt = new DateTimeOffset(2020, 06, 30, 0, 0, 0, TimeSpan.Zero);
        protected DateTimeOffset EffectiveAtTMinus1 = new DateTimeOffset(2020, 06, 29, 0, 0, 0, TimeSpan.Zero);
        
        // bbg ids for remote bbg calls with test account
        private const string AmznFigi = "BBG000BVPV84";
        private const string MicFigi = "BBG000BPHFS9";
        // treasury bond unknown instrument (not real)
        private const string UnknownFigi = "BBG123AAA456";

        [SetUp]
        public virtual void SetUp()
        {
            _lusidApiFactory = LusidApiFactoryBuilder.Build("secrets_api.json");
            var transactionPortfoliosApi = _lusidApiFactory.Api<TransactionPortfoliosApi>();
            
            // Create Portfolio with AMZN
            CreateTransactionPortfolio(transactionPortfoliosApi, Portfolio);
            UpsertTestTransactionsAmzn(transactionPortfoliosApi, Portfolio);
            CreateTransactionPortfolio(transactionPortfoliosApi, PortfolioSameHoldingAsP1);
            UpsertTestTransactionsAmzn(transactionPortfoliosApi, PortfolioSameHoldingAsP1);
            // Create Portfolio with MSFT
            CreateTransactionPortfolio(transactionPortfoliosApi, Portfolio2);
            UpsertTestTransactionsMsft(transactionPortfoliosApi, Portfolio2);
            // Create a portfolio with no holdings
            CreateTransactionPortfolio(transactionPortfoliosApi, PortfolioNoHoldings);
            
            // Create a portfolio with holdings in a instrument that does not exist in LUSID
            CreateTransactionPortfolio(transactionPortfoliosApi, PortfolioWithUnknownInstrument);
            UpsertTestTransactionsUnknownInstrument(transactionPortfoliosApi, PortfolioWithUnknownInstrument);

        }
        
        [TearDown]
        public virtual void TearDown()
        {
            _lusidApiFactory.Api<PortfoliosApi>().DeletePortfolio(Scope, Portfolio);
            _lusidApiFactory.Api<PortfoliosApi>().DeletePortfolio(Scope, PortfolioSameHoldingAsP1);
            _lusidApiFactory.Api<PortfoliosApi>().DeletePortfolio(Scope, Portfolio2);
            _lusidApiFactory.Api<PortfoliosApi>().DeletePortfolio(Scope, PortfolioNoHoldings);
            _lusidApiFactory.Api<PortfoliosApi>().DeletePortfolio(Scope, PortfolioWithUnknownInstrument);
        }
        
        private string CreateTransactionPortfolio(TransactionPortfoliosApi transactionPortfoliosApi, string portfolioCode)
        {
            var portfolioEffectiveDate = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var request = new CreateTransactionPortfolioRequest(
                code: $"{portfolioCode}",
                displayName: $"Portfolio-{Portfolio}",                 
                baseCurrency: "USD",
                created: portfolioEffectiveDate
            );
            var portfolioCreated = transactionPortfoliosApi.CreatePortfolio(Scope, request);
            return portfolioCreated.Id.Code;
        }

        private void UpsertTestTransactionsMsft(TransactionPortfoliosApi transactionPortfoliosApi, string transactionPortfolio)
        {
            var transactions = new List<TransactionRequest>()
            {
                BuildTransactionRequest(MicFigi, 500, 217.10M, "USD", EffectiveAtTMinus1, "Buy")
            };
            transactionPortfoliosApi.UpsertTransactions(Scope, transactionPortfolio, transactions);
        }
        private void UpsertTestTransactionsAmzn(TransactionPortfoliosApi transactionPortfoliosApi, string transactionPortfolio)
        {
            var transactions = new List<TransactionRequest>()
            {
                BuildTransactionRequest(AmznFigi, 100, 3292.01M, "USD", EffectiveAtTMinus1, "Buy"),
            };
            transactionPortfoliosApi.UpsertTransactions(Scope, transactionPortfolio, transactions);
        }
        
        private void UpsertTestTransactionsUnknownInstrument(TransactionPortfoliosApi transactionPortfoliosApi, string transactionPortfolio)
        {
            var transactions = new List<TransactionRequest>()
            {
                BuildTransactionRequest(UnknownFigi, 10, 200.01M, "USD", EffectiveAtTMinus1, "Buy"),
            };
            transactionPortfoliosApi.UpsertTransactions(Scope, transactionPortfolio, transactions);
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