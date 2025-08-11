using System.Globalization;
using Autofac;
using FluentValidation.Results;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.CloseTrade;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class CloseTradeTests : DomainTests
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-08-03T14:00:00").ToUtcKind();

    private ICloseTrade Interactor => ServiceLocator.Resolve<ICloseTrade>();

    protected override void OverrideServices(ContainerBuilder builder)
    {
        builder.Register<UtcNow>(_ => () => _utcNow);
    }

    [Fact]
    public async Task Invalid_Result_input_with_a_different_ui_culture_returns_a_localized_error_message()
    {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-AT");

        var requestModel = new CloseTradeRequestModel(Guid.CreateVersion7(),
            DateTime.Parse("2024-08-03T16:00:00+00:00"),
            0m
        )
        {
            ManuallyEnteredResult = (ResultModel) 50,
            ExitPrice = 1.05m
        };

        var response = await Interactor.Execute(requestModel, TestContext.Current.CancellationToken);

        var validationResult = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(validationResult.Errors);
        Assert.Equal("'Ergebnis' hat einen Wertebereich, der '50' nicht enthält.", error.ErrorMessage);
        Assert.Equal("ManuallyEnteredResult", error.PropertyName);
    }

    [Fact]
    public async Task A_not_existing_trade_cannot_be_closed()
    {
        var tradeId = Guid.Parse("2b58e712-e7d4-4df2-8a62-c9baac5ee889");
        var requestModel =
            new CloseTradeRequestModel(tradeId, DateTime.Parse("2024-08-03T16:00:00Z"), 500)
            {
                ManuallyEnteredResult = ResultModel.Win,
                ExitPrice = 1.05m
            };

        var response = await Interactor.Execute(requestModel, TestContext.Current.CancellationToken);

        var notFound = Assert.IsType<NotFound<Trade>>(response.Value);
        Assert.Equal("Trade", notFound.ResourceType);
        Assert.Equal(tradeId, notFound.ResourceId);
    }

    [Fact]
    public async Task A_trades_exit_price_must_be_greater_than_zero()
    {
        var trade = (TestData.Trade.Default with {Opened = _utcNow}).Build();
        DbContext.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var requestModel =
            new CloseTradeRequestModel(trade.Id, _utcNow.AddHours(1), 500)
            {
                ManuallyEnteredResult = ResultModel.Win,
                ExitPrice = 0m
            };

        var response = await Interactor.Execute(requestModel, TestContext.Current.CancellationToken);

        var validationResult = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(validationResult.Errors);
        Assert.Equal("ExitPrice", error.PropertyName);
        Assert.Equal("'Exit Price' must be greater than '0'.", error.ErrorMessage);
    }

    [Fact]
    public async Task A_trade_can_be_closed_successfully()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices {EntryPrice = 1m, StopLoss = 0.9m, TakeProfit = 1.4m},
            Opened = _utcNow
        }).Build();
        DbContext.Add(trade);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var requestModel =
            new CloseTradeRequestModel(trade.Id, _utcNow.AddHours(1), 500)
                {ExitPrice = 1.2m};

        var response = await Interactor.Execute(requestModel, TestContext.Current.CancellationToken);

        var responseModel = Assert.IsType<Completed<CloseTradeResponseModel>>(response.Value);
        Assert.Equal((short) 50, responseModel.Data.Performance);
        Assert.Equal(ResultModel.Mediocre, responseModel.Data.Result);

        var closedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        Assert.NotNull(closedTrade);

        Assert.Equal(requestModel.ProfitLoss, closedTrade.ProfitLoss);
        Assert.NotNull(closedTrade.Closed);
        Assert.True(closedTrade.IsClosed);
        Assert.NotNull(closedTrade.PositionPrices.Exit);
        Assert.Equal(requestModel.ExitPrice, closedTrade.PositionPrices.Exit);
    }
}