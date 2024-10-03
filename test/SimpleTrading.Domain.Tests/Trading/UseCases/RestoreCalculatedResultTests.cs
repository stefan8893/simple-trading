using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.RestoreCalculatedResult;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;
using SimpleTrading.WebApi;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class RestoreCalculatedResultTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-09-22T20:00:00").ToUtcKind();
    
    private IRestoreCalculatedResult Interactor => ServiceLocator.GetRequiredService<IRestoreCalculatedResult>();
    
    [Fact]
    public async Task A_not_overriden_result_will_not_be_changed()
    {
        // arrange
        var tradeWithCalculatedMediocreResult = (TestData.Trade.Default with
        {
            Opened = _utcNow,
            Closed = _utcNow,
            Balance = 50,
            PositionPrices = new PositionPrices
            {
                Entry = 1.0m,
                StopLoss = 0.9m,
                TakeProfit = 1.3m,
                Exit = 1.25m
            }
        }).Build();

        DbContext.Trades.Add(tradeWithCalculatedMediocreResult);
        await DbContext.SaveChangesAsync();

        tradeWithCalculatedMediocreResult.Result!.Name.Should().Be(Result.Mediocre);
        tradeWithCalculatedMediocreResult.Result.Performance.Should().Be(83);

        // act
        var requestModel = new RestoreCalculatedResultRequestModel(tradeWithCalculatedMediocreResult.Id);
        var response = await Interactor.Execute(requestModel);

        // assert
        var responseModel = response.Value.Should().BeOfType<Completed<RestoreCalculatedResultResponseModel>>();
        responseModel.Which.Data.Warnings.Should().BeEmpty();
        responseModel.Which.Data.Result.Should().NotBeNull().And.Be(ResultModel.Mediocre);
        responseModel.Which.Data.Performance.Should().Be(83);
    }

    [Fact]
    public async Task You_cant_restore_a_result_of_a_not_existing_trade()
    {
        // arrange
        var notExistingTradeId = Guid.Parse("e4240058-fef0-4a15-bbf7-f5d8796a8187");

        // act
        var requestModel = new RestoreCalculatedResultRequestModel(notExistingTradeId);
        var response = await Interactor.Execute(requestModel);
        
        // assert
        var notFound = response.Value.Should().BeOfType<NotFound<Trade>>();
        notFound.Which.ResourceId.Should().Be(notExistingTradeId);
        notFound.Which.ResourceType.Should().Be("Trade");
    }

    [Fact]
    public async Task An_overriden_result_gets_successfully_reset()
    {
        // arrange
        var tradeWithCalculatedMediocreResult = (TestData.Trade.Default with
        {
            Opened = _utcNow,
            Closed = _utcNow,
            Balance = 50,
            PositionPrices = new PositionPrices
            {
                Entry = 1.0m,
                StopLoss = 0.9m,
                TakeProfit = 1.3m,
                Exit = 1.25m
            }
        }).Build();

        tradeWithCalculatedMediocreResult.Close(new CloseTradeConfiguration(
            tradeWithCalculatedMediocreResult.Closed!.Value, tradeWithCalculatedMediocreResult.Balance!.Value,
            () => _utcNow)
        {
            ManuallyEnteredResult = ResultModel.Loss
        });

        DbContext.Trades.Add(tradeWithCalculatedMediocreResult);
        await DbContext.SaveChangesAsync();
        
        tradeWithCalculatedMediocreResult.Result!.Name.Should().Be(Result.Loss);

        // act
        var requestModel = new RestoreCalculatedResultRequestModel(tradeWithCalculatedMediocreResult.Id);
        var response = await Interactor.Execute(requestModel);

        // assert
        var responseModel = response.Value.Should().BeOfType<Completed<RestoreCalculatedResultResponseModel>>();
        responseModel.Which.Data.Warnings.Should().BeEmpty();
        responseModel.Which.Data.Result.Should().NotBeNull().And.Be(ResultModel.Mediocre);
        responseModel.Which.Data.Performance.Should().Be(83);
    }
}