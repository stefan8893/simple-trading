using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.RestoreCalculatedResult;

public interface IRestoreCalculatedResult : IInteractor<RestoreCalculatedResultRequestModel,
    OneOf<Completed<RestoreCalculatedResultResponseModel>, NotFound, BusinessError>
>
{
}