using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.DeleteTrade;

public interface IDeleteTrade : IInteractor<Guid, OneOf<Completed, NotFound>>
{
}