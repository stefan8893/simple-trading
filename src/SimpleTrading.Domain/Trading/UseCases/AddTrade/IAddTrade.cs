using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

public interface
    IAddTrade : IInteractor<AddTradeRequestModel,
    OneOf<Completed<AddTradeResponseModel>, 
        BadInput,
        NotFound,
        BusinessError>>
{
}