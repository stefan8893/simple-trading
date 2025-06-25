using Autofac;
using AwesomeAssertions;
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
        result.Result.Should().Be(ResultDto.Loss);
        result.Performance.Should().Be(55);
        result.TradeId.Should().Be(tradeId);
        result.Warnings.Should().BeEmpty();
    }

    [Fact]
    public async Task Not_found_gets_returned_if_the_trade_does_not_exist()
    {
        // arrange
        var client = await CreateClient();
        var tradeId = Guid.Parse("8614528d-0d7b-4a62-b210-493eca25cf92");
        restoreCalculatedResultInteractorStub.ResponseModel = new NotFound<Trade>(tradeId);

        // act
        var act = () => client.RestoreCalculatedResultAsync(tradeId);

        // assert
        var notFound = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        notFound.Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFound.Which.Result.Messages.Should().HaveCount(1)
            .And.Contain("Trade nicht gefunden.");
    }

    [Fact]
    public async Task A_business_error_results_in_an_unprocessable_entity_response()
    {
        // arrange
        var client = await CreateClient();
        var tradeId = Guid.Parse("8614528d-0d7b-4a62-b210-493eca25cf92");
        restoreCalculatedResultInteractorStub.ResponseModel = new BusinessError(tradeId, "Something went badly wrong.");

        // act
        var act = () => client.RestoreCalculatedResultAsync(tradeId);

        // assert
        var unprocessableEntity = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        unprocessableEntity.Which.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        unprocessableEntity.Which.Result.Messages.Should().HaveCount(1)
            .And.Contain(x => x == "Something went badly wrong.");
    }
}