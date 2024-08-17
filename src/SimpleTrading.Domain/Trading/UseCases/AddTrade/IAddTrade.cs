using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

public interface
    IAddTrade : IInteractor<AddTradeRequestModel,
    OneOf<Completed<Guid>,
        BadInput,
        NotFound,
        BusinessError>>
{
}