using System;
using System.Collections.Generic;
using System.Linq;
using Lusid.Sdk.Api;
using Lusid.Sdk.Utilities;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    public class LusidPortfolioInstrumentSource : IInstrumentSource
    {
        private const string FigiInstrumentProperty = "Instrument/default/Figi";
        
        private readonly string scope;
        private readonly string portfolio;
        private readonly DateTimeOffset effectiveAt;

        public LusidPortfolioInstrumentSource(string scope, string portfolio, DateTimeOffset effectiveAt)
        {
            this.scope = scope;
            this.portfolio = portfolio;
            this.effectiveAt = effectiveAt;
        }

        public Instruments Get()
        {
            var lusidApiFactory = LusidApiFactoryBuilder.Build("secrets_api.json");
            var figis = getHoldingsInstrumentFigis(lusidApiFactory, scope, portfolio, effectiveAt);
            return ToBbgDlInstruments(figis);
        }

        private Instruments ToBbgDlInstruments(ISet<string> figis)
        {
            var instruments = figis.Select(figi => new PerSecurity_Dotnet.Instrument()
            {
                id = figi,
                type = InstrumentType.BB_GLOBAL,
                typeSpecified = true
            }).ToArray();
            return new Instruments()
            {
                instrument = instruments
            };
        }

        private ISet<string> getHoldingsInstrumentFigis(ILusidApiFactory lusidApiFactory, string scope, string portfolio, DateTimeOffset effectiveAt)
        {
            var transactionPortfoliosApi = lusidApiFactory.Api<TransactionPortfoliosApi>();
            var holdings = transactionPortfoliosApi.GetHoldings(scope, portfolio, effectiveAt, propertyKeys: new List<string>(){FigiInstrumentProperty});

            ISet<string> figis = holdings.Values
                .Where(h => h.Properties.ContainsKey(FigiInstrumentProperty))
                .Select(h => h.Properties[FigiInstrumentProperty].Value.LabelValue)
                .ToHashSet();
            
            Console.WriteLine($"Retrieving figis for instruments of positions for scope={scope}, " +
                              $"portfolio={portfolio}, effectiveAt={effectiveAt}. Figis={figis}");
            return figis;
        }
    }
}