using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.References.UpdateReference;

public interface IUpdateReference : IInteractor<UpdateReferenceRequestModel, OneOf<Completed, BadInput, NotFound>>
{
}