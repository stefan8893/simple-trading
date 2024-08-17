using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.UpdateTrade;

public interface IUpdateTrade : IInteractor<UpdateTradeRequestModel,
    OneOf<Completed,
        BadInput,
        NotFound,
        BusinessError>>
{
}