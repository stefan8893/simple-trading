using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.RestoreCalculatedResult;

namespace SimpleTrading.WebApi.Tests.Features.Trading.TradesController.TestDoubles;

[UsedImplicitly]
public class RestoreCalculatedResultInteractorStub : IRestoreCalculatedResult
{
    public OneOf<Completed<RestoreCalculatedResultResponseModel>, NotFound, BusinessError>? ResponseModel { get; set; }

    public Task<OneOf<Completed<RestoreCalculatedResultResponseModel>, NotFound, BusinessError>> Execute(
        RestoreCalculatedResultRequestModel model)
    {
        return Task.FromResult(ResponseModel ??
                                    throw new InvalidOperationException("Response model has not been initialized."));
    }
}