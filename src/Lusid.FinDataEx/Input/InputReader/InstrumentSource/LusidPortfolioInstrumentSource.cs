using System;
using System.Collections.Generic;
using System.Linq;
using Lusid.Sdk.Utilities;
using Lusid.Sdk.Api;
using Lusid.Sdk.Client;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    public class LusidPortfolioInstrumentSource : IInstrumentSource
    {
        private readonly DataLicenseOptions _dataOptions;
        private readonly InstrumentArgs _instrumentArgs;
        private readonly string _instrumentTypeLusidPropertyKey;
        private readonly ITransactionPortfoliosApi _transactionPortfoliosApi;

        public LusidPortfolioInstrumentSource(DataLicenseOptions dataOptions, ILusidApiFactory lusidApiFactory)
        {
            _dataOptions = dataOptions;
            _instrumentArgs = _instrumentArgs = InstrumentArgs.Create(dataOptions);
            _instrumentTypeLusidPropertyKey = GetLusidInstrumentIdPropertyAddress(_instrumentArgs.InstrumentType);
            _transactionPortfoliosApi = lusidApiFactory.Api<ITransactionPortfoliosApi>();
        }

        #nullable enable
        public Instruments? Get()
        {
            var scopesAndPortfolios = ConstructScopesAndPortfolios(_dataOptions.InstrumentSourceArguments);
            var holdingsInstrumentIds = GetHoldingsInstrumentIds(scopesAndPortfolios, DateTimeOffset.UtcNow);
            return IInstrumentSource.CreateInstruments(_instrumentArgs, holdingsInstrumentIds);
        }

        private ISet<Tuple<string, string>> ConstructScopesAndPortfolios(IEnumerable<string> instrumentSourceArgs)
        {
            // parse portfolio and scopes from request arguments (e.g. [Port1|Scope1, Port2|Scope2])
            var scopeAndPortfolioArgs = instrumentSourceArgs;

            Console.WriteLine($"Creating a portfolio and scope source for the portfolios and scopes: " +
                              $"{string.Join(',', scopeAndPortfolioArgs)}");

            // parse the input arguments into set of portfolio/scope pairs.
            ISet<Tuple<string,string>> scopesAndPortfolios = scopeAndPortfolioArgs
                .Select(p =>
                    {
                        var scopeAndPortfolio = p.Split("|");

                        if (scopeAndPortfolio.Length != 2)
                        {
                            throw new ArgumentException($"Unexpected scope and portfolio entry for {p}. Should be " +
                                                        $"in form TestScope|UK_EQUITY");
                        }

                        return new Tuple<string,string>(scopeAndPortfolio[0], scopeAndPortfolio[1]);
                    })
                .ToHashSet();

            return scopesAndPortfolios;
        }

        private ISet<string> GetHoldingsInstrumentIds(ISet<Tuple<string, string>> scopesAndPortfolios, DateTimeOffset effectiveAt)
        {
            var instrumentIds = new HashSet<string>();
            foreach (var (scope, portfolio) in scopesAndPortfolios)
            {
                try
                {
                    // retrieve holdings from LUSID with instrument id property attached.
                    var holdings = _transactionPortfoliosApi.GetHoldings(scope, portfolio, effectiveAt,
                        propertyKeys: new List<string>() {_instrumentTypeLusidPropertyKey});

                    // filter only entries with valid instrument ids and map to a set for unique instruments.
                    var validPortfolioInstrumentIds = holdings.Values
                        .Where(h => h.Properties.ContainsKey(_instrumentTypeLusidPropertyKey))
                        .Select(h => h.Properties[_instrumentTypeLusidPropertyKey].Value.LabelValue)
                        .ToHashSet();

                    Console.WriteLine($"Retrieving ids for instruments of positions for scope={scope}, " +
                                      $"portfolio={portfolio}, effectiveAt={effectiveAt}. InstrumentType={_instrumentArgs.InstrumentType}");
                    
                    // union with other portfolio holding instruments
                    instrumentIds.UnionWith(validPortfolioInstrumentIds);
                }
                catch (ApiException e)
                {
                    Console.WriteLine(
                        $"Could not retrieve holdings for scope={scope} and portfolio={portfolio} due to exception. " +
                        $"Ignoring and moving to next portfolio. Details: {e}");
                }
            }

            return instrumentIds;
        }

        private static string GetLusidInstrumentIdPropertyAddress(InstrumentType instrumentType) => instrumentType switch
        {
            InstrumentType.BB_GLOBAL => "Instrument/default/Figi",
            InstrumentType.ISIN => "Instrument/default/Isin",
            InstrumentType.CUSIP => "Instrument/default/Cusip",
            _ => throw new ArgumentException($"Only Figi, Isin and Cusips are currently supported. {instrumentType} not yet supported.")
        };
    }
}