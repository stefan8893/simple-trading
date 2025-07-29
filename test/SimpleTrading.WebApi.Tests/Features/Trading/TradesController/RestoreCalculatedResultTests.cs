using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using SimpleTrading.Client;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.RestoreCalculatedResult;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.WebApi.Tests.Features.Trading.TradesController.TestDoubles;

namespace SimpleTrading.WebApi.Tests.Features.Trading.TradesController;

public class RestoreCalculatedResultTests(
    TestingWebApplicationFactory<Program> factory,
    RestoreCalculatedResultInteractorStub restoreCalculatedResultInteractorStub)
    : WebApiTests(factory), IClassFixture<RestoreCalculatedResultInteractorStub>
{
    protected override void OverrideServices(HostBuilderContext ctx, ContainerBuilder builder)
    {
        builder.Register<IRestoreCalculatedResult>(_ => restoreCalculatedResultInteractorStub);
    }

    [Fact]
    public async Task A_calculated_result_gets_successfully_restored()
    {
        // arrange
        var client = await CreateClient();
        var tradeId = Guid.Parse("8614528d-0d7b-4a62-b210-493eca25cf92");

        restoreCalculatedResultInteractorStub.ResponseModel = new Completed<RestoreCalculatedResultResponseModel>(
            new RestoreCalculatedResultResponseModel(tradeId, ResultModel.Loss, 55, []));

        // act
        var result = await client.RestoreCalculatedResultAsync(tradeId);

        // assert
        Assert.Equal(ResultDto.Loss, result.Result);
        Assert.Equal(55, result.Performance);
        Assert.Equal(tradeId, result.TradeId);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public async Task Not_found_gets_returned_if_the_trade_does_not_exist()
    {
        // arrange
        var client = await CreateClient();
        var tradeId = Guid.Parse("8614528d-0d7b-4a62-b210-493eca25cf92");
        restoreCalculatedResultInteractorStub.ResponseModel = new NotFound<Trade>(tradeId);

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<TradeResultDto> Act() => client.RestoreCalculatedResultAsync(tradeId);

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ErrorResponse>>(Act);
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        var singleError = Assert.Single(exception.Result.Messages);
        Assert.Equal("Trade nicht gefunden.", singleError);
    }

    [Fact]
    public async Task A_business_error_results_in_an_unprocessable_entity_response()
    {
        // arrange
        var client = await CreateClient();
        var tradeId = Guid.Parse("8614528d-0d7b-4a62-b210-493eca25cf92");
        restoreCalculatedResultInteractorStub.ResponseModel = new BusinessError(tradeId, "Something went badly wrong.");

        // act
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<TradeResultDto> Act() => client.RestoreCalculatedResultAsync(tradeId);

        // assert
        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ErrorResponse>>(Act);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, exception.StatusCode);
        var singleError = Assert.Single(exception.Result.Messages);
        Assert.Equal("Something went badly wrong.", singleError);
    }
}