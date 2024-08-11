using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.CloseTrade;

public interface
    ICloseTrade : IInteractor<CloseTradeRequestModel,
    OneOf<Completed<CloseTradeResponseModel>, CompletedWithWarnings<CloseTradeResponseModel>, BadInput, NotFound,
        BusinessError>>
{
}