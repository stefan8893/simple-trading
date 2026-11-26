using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.DataAccess;

public interface ITradeQueries
{
    Task<TradeResponseModel?> FindTradeModel(Guid tradeId);
    Task<PagedList<TradeResponseModel>> SearchTrades(SearchTradesRequestModel model,
        CancellationToken cancellationToken = default);
}