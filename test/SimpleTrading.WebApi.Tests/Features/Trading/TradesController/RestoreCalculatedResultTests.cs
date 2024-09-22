using Autofac;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Moq;
using SimpleTrading.Client;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.RestoreCalculatedResult;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;

namespace SimpleTrading.WebApi.Tests.Features.Trading.TradesController;

public class RestoreCalculatedResultTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private static readonly Mock<IRestoreCalculatedResult> InteractorMock = new (MockBehavior.Strict);

    protected override void OverrideServices(HostBuilderContext ctx, ContainerBuilder builder)
    {
        builder.Register<IRestoreCalculatedResult>(_ => InteractorMock.Object);
    }

    [Fact]
    public async Task A_calculated_result_gets_successfully_restored()
    {
        // arrange
        var client = await CreateClient();
        var tradeId = Guid.Parse("8614528d-0d7b-4a62-b210-493eca25cf92");
        InteractorMock.Setup(interactor =>
                interactor.Execute(It.Is<RestoreCalculatedResultRequestModel>(x => x.TradeId == tradeId)))
            .ReturnsAsync(
                new Completed<RestoreCalculatedResultResponseModel>(
                    new RestoreCalculatedResultResponseModel(tradeId, ResultModel.Loss, 55)));

        // act
        var result = await client.RestoreCalculatedResultAsync(tradeId);

        // assert
        result.Data.Result.Should().Be(ResultDto.Loss);
        result.Data.Performance.Should().Be(55);
        result.Data.TradeId.Should().Be(tradeId);
        result.Warnings.Should().BeEmpty();
    }

    [Fact]
    public async Task Not_found_gets_returned_if_the_trade_does_not_exist()
    {
        // arrange
        var client = await CreateClient();
        var tradeId = Guid.Parse("8614528d-0d7b-4a62-b210-493eca25cf92");
        InteractorMock.Setup(interactor =>
                interactor.Execute(It.IsAny<RestoreCalculatedResultRequestModel>()))
            .ReturnsAsync(new NotFound<Trade>(tradeId));

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
        InteractorMock.Setup(interactor =>
                interactor.Execute(It.IsAny<RestoreCalculatedResultRequestModel>()))
            .ReturnsAsync(new BusinessError(tradeId, "Something went badly wrong."));

        // act
        var act = () => client.RestoreCalculatedResultAsync(tradeId);
        
        // assert
        var unprocessableEntity = await act.Should().ThrowExactlyAsync<SimpleTradingClientException<ErrorResponse>>();
        unprocessableEntity.Which.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        unprocessableEntity.Which.Result.Messages.Should().HaveCount(1)
            .And.Contain(x => x == "Something went badly wrong.");
    }
}