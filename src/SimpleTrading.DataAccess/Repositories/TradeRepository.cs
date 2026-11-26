using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SimpleTrading.Domain;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.DataAccess;
using SimpleTrading.Domain.Infrastructure.Filter;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.DataAccess.Repositories;

[UsedImplicitly]
public class TradeRepository(
    TradingDbContext dbContext,
    IEnumerable<IFilterPredicate<Trade>> filterPredicates,
    IReadOnlyDictionary<string, Func<Order, ISort<Trade>>> sorterByName) : RepositoryBase<Trade>(dbContext),
    ITradeRepository, ITradeQueries
{
    public void AddReference(Reference reference) => dbContext.References.Add(reference);

    public void RemoveReferences(IEnumerable<Reference> references) => dbContext.References.RemoveRange(references);

    public async Task<TradeResponseModel?> FindTradeModel(Guid tradeId)
    {
        var userSettings = await dbContext.UserSettings.SingleAsync(x => x.Id == Constants.UserSettingsId);
        
        var tradeModelQuery = from trade in dbContext.Trades
                join profile in dbContext.Profiles on trade.ProfileId equals profile.Id
                join asset in dbContext.Assets on trade.AssetId equals asset.Id
                join currency in dbContext.Currencies on trade.CurrencyId equals currency.Id
                where trade.Id == tradeId
                select TradeResponseModel.From(trade, profile, asset, currency, userSettings.TimeZone);

        return await tradeModelQuery.FirstOrDefaultAsync();
    }

    public async Task<PagedList<TradeResponseModel>> SearchTrades(SearchTradesRequestModel model,
        CancellationToken cancellationToken = default)
    {
        var userSettings = await dbContext.UserSettings.SingleAsync(x => x.Id == Constants.UserSettingsId, cancellationToken);
        var trades = dbContext.Trades.Where(x => x.ProfileId == model.ProfileId);

        foreach (var filter in model.Filter)
        {
            var predicate = filterPredicates.Single(x => x.Match(filter.PropertyName, filter.Operator));
            trades = trades.Where(predicate.GetPredicate(filter.ComparisonValue, filter.IsLiteral));
        }

        var sorting = model.Sort
            .DefaultIfEmpty(new SortModel(nameof(Trade.Opened), false))
            .Select(x => sorterByName[x.Property](x.Ascending ? Order.Ascending : Order.Descending))
            .ToList();
        var orderedTrades = ApplySorting(trades, sorting);
        var totalCount = await trades.CountAsync(cancellationToken);

        if (totalCount == 0)
            return new PagedList<TradeResponseModel>([], 0, model.Page, model.PageSize);

        var tradeModelQuery = from trade in orderedTrades
            join profile in dbContext.Profiles on trade.ProfileId equals profile.Id
            join asset in dbContext.Assets on trade.AssetId equals asset.Id
            join currency in dbContext.Currencies on trade.CurrencyId equals currency.Id
            select TradeResponseModel.From(trade, profile, asset, currency, userSettings.TimeZone);

        var tradeModels = await tradeModelQuery
            .Skip((model.Page - 1) * model.PageSize)
            .Take(model.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<TradeResponseModel>(tradeModels, totalCount, model.Page, model.PageSize);
    }

    private static IOrderedQueryable<Trade> ApplySorting(IQueryable<Trade> trades, IReadOnlyList<ISort<Trade>> sorting)
    {
        var firstSorting = sorting.First();
        var first = firstSorting.IsAscendingOrder
            ? trades.OrderBy(firstSorting.Selector)
            : trades.OrderByDescending(firstSorting.Selector);

        return sorting
            .Skip(1)
            .Aggregate(first, (acc, next) => next.IsAscendingOrder
                ? acc.ThenBy(next.Selector)
                : acc.ThenByDescending(next.Selector));
    }
}