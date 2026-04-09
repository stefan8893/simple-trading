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
        var client = await CreateClient();
        var tradeId = Guid.Parse("8614528d-0d7b-4a62-b210-493eca25cf92");

        restoreCalculatedResultInteractorStub.ResponseModel = new Completed<RestoreCalculatedResultResponseModel>(
            new RestoreCalculatedResultResponseModel(tradeId, ResultModel.Loss, 55, []));

        var result = await client.RestoreCalculatedResultAsync(tradeId, TestContext.Current.CancellationToken);

        Assert.Equal(ResultDto.Loss, result.Result);
        Assert.Equal(55, result.Performance);
        Assert.Equal(tradeId, result.TradeId);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public async Task Not_found_gets_returned_if_the_trade_does_not_exist()
    {
        var client = await CreateClient();
        var notExistingTradeId = Guid.Parse("8614528d-0d7b-4a62-b210-493eca25cf92");
        restoreCalculatedResultInteractorStub.ResponseModel = new NotFound<Trade>(notExistingTradeId);

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<TradeResultDto> Act()
        {
            return client.RestoreCalculatedResultAsync(notExistingTradeId);
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        Assert.Equal("Trade nicht gefunden.", exception.Result.Title);
        Assert.Equal($"Trade mit der ID '{notExistingTradeId}' nicht gefunden.", exception.Result.Detail);
    }

    [Fact]
    public async Task A_conflict_results_in_a_conflict_response()
    {
        var client = await CreateClient();
        var tradeId = Guid.Parse("8614528d-0d7b-4a62-b210-493eca25cf92");
        restoreCalculatedResultInteractorStub.ResponseModel = new Conflict(tradeId, "Something went badly wrong.");

        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<TradeResultDto> Act()
        {
            return client.RestoreCalculatedResultAsync(tradeId);
        }

        var exception = await Assert.ThrowsAsync<SimpleTradingClientException<ProblemDetails>>(Act);
        Assert.Equal(StatusCodes.Status409Conflict, exception.StatusCode);
        Assert.Equal("Something went badly wrong.", exception.Result.Detail);
    }
}