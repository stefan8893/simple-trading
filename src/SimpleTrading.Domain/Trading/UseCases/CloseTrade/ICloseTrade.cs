using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.CloseTrade;

public interface
    ICloseTrade : IInteractor<CloseTradeRequestModel,
    OneOf<Completed<CloseTradeResponseModel>, BadInput, NotFound,
        BusinessError>>
{
}