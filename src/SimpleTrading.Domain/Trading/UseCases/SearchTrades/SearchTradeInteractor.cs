using System.Linq.Expressions;
using FluentValidation;
using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertySorting;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades;

public class SearchTradeInteractor(
    IValidator<SearchTradesRequestModel> validator,
    ITradeRepository tradeRepository,
    IUserSettingsRepository userSettingsRepository,
    IReadOnlyDictionary<string, IPropertyFilterComparisonVisitor<Trade>> comparisonByOperator)
    : BaseInteractor, ISearchTrades
{
    private static readonly Expression<Func<Trade, bool>> Id = x => true;

    public async Task<OneOf<PagedList<TradeResponseModel>, BadInput>> Execute(SearchTradesRequestModel model)
    {
        var validation = await validator.ValidateAsync(model);
        if (!validation.IsValid)
            return BadInput(validation);

        var sorting = model.Sort
            .Select(x =>
                PropertySortingFactory.SortingByProperty[x.Property](x.Ascending
                    ? Order.Ascending
                    : Order.Descending));

        var filter = model.Filter
            .Select(x => PropertyFilterFactory.Create(x.PropertyName, x.Operator, x.ComparisonValue))
            .Aggregate(Id, Add);

        var paginationConfig = new PaginationConfiguration(model.Page, model.PageSize);
        var trades = await tradeRepository.Find(paginationConfig, filter, sorting);

        var userSettings = await userSettingsRepository.Get();

        return trades
            .Select(x => TradeResponseModel.From(x, userSettings.TimeZone));
    }

    private Expression<Func<Trade, bool>> Add(Expression<Func<Trade, bool>> acc, IPropertyFilter<Trade> propertyFilter)
    {
        var propertyFilterComparison = comparisonByOperator[propertyFilter.Operator];

        var tradeParameter = Expression.Parameter(typeof(Trade));
        return (Expression<Func<Trade, bool>>) Expression.Lambda(
            Expression.AndAlso(
                Expression.Invoke(acc, tradeParameter),
                Expression.Invoke(propertyFilter.GetPredicate(propertyFilterComparison), tradeParameter)),
            tradeParameter);
    }
}