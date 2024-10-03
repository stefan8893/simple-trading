using System.Globalization;
using Autofac;
using FluentAssertions;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.CloseTrade;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
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
        // arrange
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-AT");

        var requestModel = new CloseTradeRequestModel(Guid.NewGuid(),
            DateTime.Parse("2024-08-03T16:00:00+00:00"),
            0m
        )
        {
            ManuallyEnteredResult = (ResultModel) 50,
            ExitPrice = 1.05m
        };

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors
            .Should().Contain(x => x.ErrorMessage == "'Ergebnis' hat einen Wertebereich, der '50' nicht enthält.")
            .And.Contain(x => x.PropertyName == "ManuallyEnteredResult")
            .And.HaveCount(1);
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

        var response = await Interactor.Execute(requestModel);

        var notFound = response.Value.Should().BeOfType<NotFound<Trade>>();
        notFound.Which.ResourceType.Should().Be("Trade");
        notFound.Which.ResourceId.Should().Be(tradeId);
    }

    [Fact]
    public async Task A_trades_exit_price_must_be_greater_than_zero()
    {
        // arrange
        var trade = (TestData.Trade.Default with {Opened = _utcNow}).Build();
        DbContext.Add(trade);
        await DbContext.SaveChangesAsync();

        var requestModel =
            new CloseTradeRequestModel(trade.Id, _utcNow.AddHours(1), 500)
            {
                ManuallyEnteredResult = ResultModel.Win,
                ExitPrice = 0m
            };

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "ExitPrice" &&
                              x.ErrorMessage == "'Exit price' must be greater than '0'.");
    }

    [Fact]
    public async Task A_trade_can_be_closed_successfully()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices {EntryPrice = 1m, StopLoss = 0.9m, TakeProfit = 1.4m},
            Opened = _utcNow
        }).Build();
        DbContext.Add(trade);
        await DbContext.SaveChangesAsync();

        var requestModel =
            new CloseTradeRequestModel(trade.Id, _utcNow.AddHours(1), 500)
                {ExitPrice = 1.2m};

        // act
        var response = await Interactor.Execute(requestModel);

        // assert
        var responseModel = response.Value.Should().BeOfType<Completed<CloseTradeResponseModel>>();
        responseModel.Which.Data.Performance.Should().Be(50);
        responseModel.Which.Data.Result.Should().Be(ResultModel.Mediocre);

        var closedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        closedTrade.Should().NotBeNull();

        closedTrade!.Balance.Should().Be(requestModel.Balance);
        closedTrade.Closed.Should().NotBeNull();
        closedTrade.IsClosed.Should().BeTrue();
        closedTrade.PositionPrices.Exit.Should().NotBeNull().And.Be(requestModel.ExitPrice);
    }
}