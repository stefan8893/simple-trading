using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

internal static class SearchTradesQueryExtensions
{
    public static async Task<SearchTradesQueryResult> Execute(this ITradeQueries tradeQueries,
        SearchTradesRequestModel model, CancellationToken cancellationToken)
    {
        var result = await tradeQueries.SearchTrades(model, cancellationToken);
        return new SearchTradesQueryResult(result);
    }
}

internal record SearchTradesQueryResult(PagedList<TradeResponseModel> Value);