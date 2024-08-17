using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.References.GetReferences;

public interface IGetReferences : IInteractor<GetReferencesRequestModel, OneOf<IReadOnlyList<ReferenceModel>, NotFound>>
{
}