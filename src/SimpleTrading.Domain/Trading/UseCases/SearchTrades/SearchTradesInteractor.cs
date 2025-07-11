﻿using System.Linq.Expressions;
using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.DataAccess;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Infrastructure.Filter;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.Domain.User.DataAccess;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades;

[UsedImplicitly]
public class SearchTradesInteractor(
    ITradeRepository tradeRepository,
    IUserSettingsRepository userSettingsRepository,
    IEnumerable<IFilterPredicate<Trade>> filterPredicates,
    IReadOnlyDictionary<string, Func<Order, ISort<Trade>>> sorterByName)
    : InteractorBase, IInteractor<SearchTradesRequestModel, OneOf<PagedList<TradeResponseModel>, BadInput>>
{
    private static readonly Expression<Func<Trade, bool>> Id = x => true;

    public async Task<OneOf<PagedList<TradeResponseModel>, BadInput>> Execute(SearchTradesRequestModel model)
    {
        var sortingConfig = model.Sort
            .DefaultIfEmpty(new SortModel(nameof(Trade.Opened), false))
            .Select(x => sorterByName[x.Property](x.Ascending ? Order.Ascending : Order.Descending));

        var filter = model.Filter
            .Select(x => new { Model = x, Filter = filterPredicates.Single(p => p.Match(x.PropertyName, x.Operator)) })
            .Select(x => x.Filter.GetPredicate(x.Model.ComparisonValue, x.Model.IsLiteral))
            .Aggregate(Id, Add);

        var filterIncludingProfile = Add(filter, trade => trade.ProfileId == model.ProfileId);
        var paginationConfig = new PaginationConfiguration(model.Page, model.PageSize);
        var trades = await tradeRepository.Find(filterIncludingProfile, paginationConfig, sortingConfig);

        var userSettings = await userSettingsRepository.GetUserSettings();

        return trades
            .Select(x => TradeResponseModel.From(x, userSettings.TimeZone));
    }

    private static Expression<Func<Trade, bool>> Add(Expression<Func<Trade, bool>> acc,
        Expression<Func<Trade, bool>> next)
    {
        var tradeParameter = Expression.Parameter(typeof(Trade));
        return Expression.Lambda<Func<Trade, bool>>(
            Expression.AndAlso(
                Expression.Invoke(acc, tradeParameter),
                Expression.Invoke(next, tradeParameter)),
            tradeParameter);
    }
}