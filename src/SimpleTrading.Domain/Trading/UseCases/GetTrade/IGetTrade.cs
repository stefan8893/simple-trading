using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.GetTrade;

public interface IGetTrade : IInteractor<GetTradeRequestModel, OneOf<TradeResponseModel, NotFound>>
{
}