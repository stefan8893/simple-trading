using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.DeleteTrade;

public interface IDeleteTrade : IInteractor<DeleteTradeRequestModel, OneOf<Completed, NotFound>>
{
}