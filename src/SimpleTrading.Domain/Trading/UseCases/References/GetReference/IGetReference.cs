using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.References.GetReference;

public interface IGetReference : IInteractor<GetReferenceRequestModel, OneOf<ReferenceModel, NotFound>>
{
}