using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.References.AddReference;

public interface IAddReference : IInteractor<AddReferenceRequestModel,
    OneOf<Completed<Guid>,
        BadInput,
        NotFound,
        BusinessError>>
{
}