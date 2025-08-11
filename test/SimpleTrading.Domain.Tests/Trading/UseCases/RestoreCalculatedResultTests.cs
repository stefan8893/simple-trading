using Autofac;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.RestoreCalculatedResult;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class RestoreCalculatedResultTests : DomainTests
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-09-22T20:00:00").ToUtcKind();

    private IRestoreCalculatedResult Interactor => ServiceLocator.Resolve<IRestoreCalculatedResult>();

    [Fact]
    public async Task A_not_overriden_result_will_not_be_changed()
    {
        var tradeWithCalculatedMediocreResult = (TestData.Trade.Default with
        {
            Opened = _utcNow,
            Closed = _utcNow,
            ProfitLoss = 50,
            PositionPrices = new PositionPrices
            {
                Entry = 1.0m,
                StopLoss = 0.9m,
                TakeProfit = 1.3m,
                Exit = 1.25m
            }
        }).Build();

        DbContext.Trades.Add(tradeWithCalculatedMediocreResult);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        Assert.Equal(Result.Mediocre, tradeWithCalculatedMediocreResult.Result!.Name);
        Assert.Equal((short) 83, tradeWithCalculatedMediocreResult.Result.Performance);

        var response = await Interactor.Execute(tradeWithCalculatedMediocreResult.Id, TestContext.Current.CancellationToken);

        var responseModel = Assert.IsType<Completed<RestoreCalculatedResultResponseModel>>(response.Value);
        Assert.Empty(responseModel.Data.Warnings);
        Assert.NotNull(responseModel.Data.Result);
        Assert.Equal(ResultModel.Mediocre, responseModel.Data.Result);
        Assert.Equal((short) 83, responseModel.Data.Performance);
    }

    [Fact]
    public async Task You_cant_restore_a_result_of_a_not_existing_trade()
    {
        var notExistingTradeId = Guid.Parse("e4240058-fef0-4a15-bbf7-f5d8796a8187");

        var response = await Interactor.Execute(notExistingTradeId, TestContext.Current.CancellationToken);

        var notFound = Assert.IsType<NotFound<Trade>>(response.Value);
        Assert.Equal(notExistingTradeId, notFound.ResourceId);
        Assert.Equal("Trade", notFound.ResourceType);
    }

    [Fact]
    public async Task An_overriden_result_gets_successfully_reset()
    {
        var tradeWithCalculatedMediocreResult = (TestData.Trade.Default with
        {
            Opened = _utcNow,
            Closed = _utcNow,
            ProfitLoss = 50,
            PositionPrices = new PositionPrices
            {
                Entry = 1.0m,
                StopLoss = 0.9m,
                TakeProfit = 1.3m,
                Exit = 1.25m
            }
        }).Build();

        tradeWithCalculatedMediocreResult.Close(new CloseTradeConfiguration(
            tradeWithCalculatedMediocreResult.Closed!.Value, tradeWithCalculatedMediocreResult.ProfitLoss!.Value,
            () => _utcNow)
        {
            ManuallyEnteredResult = ResultModel.Loss
        });

        DbContext.Trades.Add(tradeWithCalculatedMediocreResult);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        Assert.Equal(Result.Loss, tradeWithCalculatedMediocreResult.Result!.Name);

        var response = await Interactor.Execute(tradeWithCalculatedMediocreResult.Id, TestContext.Current.CancellationToken);

        var responseModel = Assert.IsType<Completed<RestoreCalculatedResultResponseModel>>(response.Value);
        Assert.Empty(responseModel.Data.Warnings);
        Assert.NotNull(responseModel.Data.Result);
        Assert.Equal(ResultModel.Mediocre, responseModel.Data.Result);
        Assert.Equal((short) 83, responseModel.Data.Performance);
    }
}