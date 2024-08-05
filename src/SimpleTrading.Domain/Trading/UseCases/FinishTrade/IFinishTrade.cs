using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.FinishTrade;

public interface
    IFinishTrade : IInteractor<FinishTradeRequestModel, OneOf<Completed, BadInput, NotFound, BusinessError>>
{
}