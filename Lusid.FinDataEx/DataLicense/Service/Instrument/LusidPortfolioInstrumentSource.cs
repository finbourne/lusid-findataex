using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Lusid.Sdk.Api;
using Lusid.Sdk.Client;
using Lusid.Sdk.Utilities;
using PerSecurity_Dotnet;

[assembly: InternalsVisibleTo("Lusid.FinDataEx.Tests", AllInternalsVisible = true)]
namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    /// <summary>
    /// Instrument source based on holdings that exist at a given time for a given set of scopes and portfolios
    /// within a LUSID domain. 
    /// </summary>
    public class LusidPortfolioInstrumentSource : IInstrumentSource
    {
        private readonly ILusidApiFactory _lusidApiFactory;
        private readonly InstrumentType _instrumentType;
        private readonly string _instrumentTypeLusidPropertyKey;
        private readonly ISet<Tuple<string, string>> _scopesAndPortfolios;
        private readonly DateTimeOffset _effectiveAt;

        protected internal LusidPortfolioInstrumentSource(ILusidApiFactory lusidApiFactory, InstrumentType instrumentType, ISet<Tuple<string, string>> scopesAndPortfolios, DateTimeOffset effectiveAt)
        {
            _lusidApiFactory = lusidApiFactory;
            _instrumentType = instrumentType;
            _instrumentTypeLusidPropertyKey = GetLusidInstrumentIdPropertyAddress(instrumentType);
            _scopesAndPortfolios = scopesAndPortfolios;
            _effectiveAt = effectiveAt;
        }

        /// <summary>
        ///  Creates an instrument source for a given for instrument ids that are constructed from the holdings
        /// of a portfolio within a given scope.
        /// </summary>
        /// <param name="instrumentType">Instrument id types (e.g. BB_GLOBAL (FIGI), ISIN, etc...)</param>
        /// <param name="instrumentSourceArgs">Set of | delimited pair of portfolio and scope (e.g. [Port1|Scope1, Port2|Scope1, Port1|Scope2])</param>
        /// <returns>A LusidPortfolioInstrumentSource instance</returns>
        public static LusidPortfolioInstrumentSource Create(InstrumentType instrumentType, IEnumerable<string> instrumentSourceArgs)
        {   
            // parse portfolio and scopes from request arguments (e.g. [Port1|Scope1, Port2|Scope1, Port1|Scope2])
            var portfoliosAndScopes = instrumentSourceArgs as string[] ?? instrumentSourceArgs.ToArray();
            // LusidApiFactory to access LUSID to retrieve portfolio gholding data
            var lusidApiFactory = LusidApiFactoryBuilder.Build("secrets.json");
            
            Console.WriteLine($"Creating a portfolio and scope source using instrument id type {instrumentType} for the " +
                              $"portfolios and scopes: {string.Join(',',portfoliosAndScopes)}");

            // parse the input arguments into set of portfolio/scope pairs.
            ISet<Tuple<string,string>> scopesAndPortfolios = portfoliosAndScopes.Select(p =>
            {
                var scopeAndPortfolio = p.Split("|");
                if (scopeAndPortfolio.Length != 2)
                {
                    throw new ArgumentException($"Unexpected scope and portfolio entry for {p}. Should be " +
                                                $"in form TestScope|UK_EQUITY");
                }
                return new Tuple<string,string>(scopeAndPortfolio[0], scopeAndPortfolio[1]);
            }).ToHashSet();
            
            // currently only support holdings as at latest date. if required to support historical dates can modify
            // to include effectiveAt as part of instrumentSourceArgs.
            var effectiveAt = DateTimeOffset.UtcNow;
            return new LusidPortfolioInstrumentSource(lusidApiFactory, instrumentType, scopesAndPortfolios, effectiveAt);
        }

        /// <summary>
        ///  Retrieves a BBG DL representation of a set of instruments that make up the holdings for the given portfolios at the given
        ///  effectiveAt time. The LUSID domain used is configured in the ILusidApiFactory provided in the constructor.
        /// </summary>
        /// <returns>Set of BBG DLWS instruments</returns>
        #nullable enable
        public Instruments? Get()
        {
            var holdingsInstrumentIds = GetHoldingsInstrumentIds(_lusidApiFactory, _scopesAndPortfolios, _effectiveAt);
            return holdingsInstrumentIds.Any() ? IInstrumentSource.CreateInstruments(_instrumentType, holdingsInstrumentIds) : null;
        }

        /// <summary>
        ///  Calls LUSID to retrieve the instrument ids of holdings for the given portfolio and effectiveAt time.
        /// </summary>
        /// <returns></returns>
        private ISet<string> GetHoldingsInstrumentIds(ILusidApiFactory lusidApiFactory, ISet<Tuple<string, string>> scopesAndPortfolios, DateTimeOffset effectiveAt)
        {
            var transactionPortfoliosApi = lusidApiFactory.Api<TransactionPortfoliosApi>();
            var instrumentIds = new HashSet<string>();
            foreach (var (scope, portfolio) in scopesAndPortfolios)
            {
                try
                {
                    // retrieve holdings from LUSID with instrument id property attached.
                    var holdings = transactionPortfoliosApi.GetHoldings(scope, portfolio, effectiveAt,
                        propertyKeys: new List<string>() {_instrumentTypeLusidPropertyKey});
                    // filter only entries with valid instrument ids and map to a set for unique instruments.
                    var validPortfolioInstrumentIds = holdings.Values
                        .Where(h => h.Properties.ContainsKey(_instrumentTypeLusidPropertyKey))
                        .Select(h => h.Properties[_instrumentTypeLusidPropertyKey].Value.LabelValue)
                        .ToHashSet();

                    Console.WriteLine($"Retrieving ids for instruments of positions for scope={scope}, " +
                                      $"portfolio={portfolio}, effectiveAt={effectiveAt}. InstrumentId={_instrumentType}, instrument Ids={validPortfolioInstrumentIds}");
                    
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

        private static string GetLusidInstrumentIdPropertyAddress(InstrumentType instrumentType)
        {
            return instrumentType switch
            {
                InstrumentType.BB_GLOBAL => "Instrument/default/Figi",
                InstrumentType.ISIN => "Instrument/default/Isin",
                InstrumentType.CUSIP => "Instrument/default/Cusip",
                _ => throw new ArgumentException(
                    $"Only Figi, Isin and Cusips are currently supported. {instrumentType} not yet supported.")
            };
        }
    }
}