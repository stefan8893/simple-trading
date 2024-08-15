using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.References.DeleteReference;

public interface IDeleteReference : IInteractor<DeleteReferenceRequestModel, OneOf<Completed, NotFound>>
{
}