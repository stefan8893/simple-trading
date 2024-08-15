using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.References.DeleteReferences;

public interface IDeleteReferences : IInteractor<DeleteReferencesRequestModel, OneOf<Completed<ushort>, NotFound>>
{
}