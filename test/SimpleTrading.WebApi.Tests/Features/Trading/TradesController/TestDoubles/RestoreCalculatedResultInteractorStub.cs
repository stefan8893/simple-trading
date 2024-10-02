using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.RestoreCalculatedResult;

namespace SimpleTrading.WebApi.Tests.Features.Trading.TradesController.TestDoubles;

public class RestoreCalculatedResultInteractorStub : IRestoreCalculatedResult
{
    public OneOf<Completed<RestoreCalculatedResultResponseModel>, NotFound, BusinessError>? ResponseModel { get; set; }

    public ValueTask<OneOf<Completed<RestoreCalculatedResultResponseModel>, NotFound, BusinessError>> Execute(
        RestoreCalculatedResultRequestModel model)
    {
        return ValueTask.FromResult(ResponseModel ??
                                    throw new InvalidOperationException("Response model has not been initialized."));
    }
}