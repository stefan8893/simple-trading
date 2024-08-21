using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades;

public interface ISearchTrades : IInteractor<SearchTradesRequestModel, OneOf<PagedList<TradeResponseModel>, BadInput>>
{
}