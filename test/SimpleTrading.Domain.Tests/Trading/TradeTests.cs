using FluentAssertions;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading;

public class TradeTests : TestBase
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-08-04T12:00").ToUtcKind();

    [Fact]
    public void You_cant_finish_a_trade_before_it_was_opened()
    {
        // arrange
        var openedAt = _utcNow.AddHours(-2);
        var finishedAt = _utcNow.AddHours(-3);

        var trade = (TestData.Trade.Default with {OpenedAt = openedAt}).Build();
        var finishTradeDto = new Trade.FinishTradeDto(Result.Win, 500m, finishedAt, UtcNowStub, "Europe/Vienna");

        // act
        var response = trade.Finish(finishTradeDto);

        // assert
        var businessError = response.Value.Should().BeOfType<BusinessError>();
        businessError.Which.ResourceId.Should().Be(trade.Id);
        businessError.Which.Reason.Should().Be("The 'Finished' date must be after the 'Opened' date.");
    }

    [Fact]
    public void The_finished_date_cannot_be_greater_than_one_day_in_the_future()
    {
        // arrange
        var openedAt = _utcNow.AddHours(-2);
        var finishedAt = _utcNow.AddDays(1).AddSeconds(1);

        var trade = (TestData.Trade.Default with {OpenedAt = openedAt}).Build();
        var finishTradeDto = new Trade.FinishTradeDto(Result.Win, 500m, finishedAt, UtcNowStub, "Europe/Vienna");

        // act
        var response = trade.Finish(finishTradeDto);

        // assert
        var businessError = response.Value.Should().BeOfType<BusinessError>();
        businessError.Which.ResourceId.Should().Be(trade.Id);
        businessError.Which.Reason.Should().Be("The 'Finished' date must not be greater than one day in the future.");
    }

    [Fact]
    public void The_finished_date_can_at_maximum_one_day_in_the_future()
    {
        // arrange
        var openedAt = _utcNow.AddHours(-2);
        var finishedAt = _utcNow.AddDays(1);

        var trade = (TestData.Trade.Default with {OpenedAt = openedAt}).Build();
        var finishTradeDto = new Trade.FinishTradeDto(Result.Win, 500m, finishedAt, UtcNowStub, "Europe/Vienna");

        // act
        var response = trade.Finish(finishTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>();
    }

    [Fact]
    public void Already_finished_trades_cant_be_finished_again()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            OpenedAt = _utcNow,
            FinishedAt = _utcNow,
            Outcome = new Outcome {Balance = 500, Result = Result.Win}
        }).Build();

        var finishTradeDto = new Trade.FinishTradeDto(Result.Win, 500m, _utcNow, UtcNowStub, Constants.DefaultTimeZone);

        // act
        var response = trade.Finish(finishTradeDto);

        // assert
        var businessError = response.Value.Should().BeOfType<BusinessError>();
        businessError.Which.ResourceId.Should().Be(trade.Id);
        businessError.Which.Reason.Should().Be("The trade has already been finished on '04.08.2024 14:00:00'.");
    }

    [Theory]
    [InlineData(Result.Win)]
    [InlineData(Result.Loss)]
    [InlineData(Result.Mediocre)]
    public void Zero_Balance_with_results_other_than_BreakEven_does_not_make_sense(Result invalidResult)
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var finishTradeDto =
            new Trade.FinishTradeDto(invalidResult, 0m, _utcNow, UtcNowStub, Constants.DefaultTimeZone);

        // act
        var response = trade.Finish(finishTradeDto);

        // assert
        var businessError = response.Value.Should().BeOfType<BusinessError>();
        businessError.Which.ResourceId.Should().Be(trade.Id);
        businessError.Which.Reason.Should().Be("The result must be 'Break-even' if the balance is zero.");
    }

    [Theory]
    [InlineData(Result.Win)]
    [InlineData(Result.Mediocre)]
    [InlineData(Result.BreakEven)]
    public void Balance_below_zero_can_only_have_Loss_as_result(Result invalidResult)
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var finishTradeDto =
            new Trade.FinishTradeDto(invalidResult, -1m, _utcNow, UtcNowStub, Constants.DefaultTimeZone);

        // act
        var response = trade.Finish(finishTradeDto);

        // assert
        var businessError = response.Value.Should().BeOfType<BusinessError>();
        businessError.Which.ResourceId.Should().Be(trade.Id);
        businessError.Which.Reason.Should()
            .Be("The result must be 'Loss' if the balance is below zero.");
    }

    [Theory]
    [InlineData(Result.Loss)]
    [InlineData(Result.BreakEven)]
    public void Balance_above_zero_can_only_be_the_results_Mediocre_and_Win(Result invalidResult)
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var finishTradeDto =
            new Trade.FinishTradeDto(invalidResult, 1m, _utcNow, UtcNowStub, Constants.DefaultTimeZone);

        // act
        var response = trade.Finish(finishTradeDto);

        // assert
        var businessError = response.Value.Should().BeOfType<BusinessError>();
        businessError.Which.ResourceId.Should().Be(trade.Id);
        businessError.Which.Reason.Should()
            .Be("The result must be 'Mediocre' or 'Win' if the balance is above zero.");
    }

    private DateTime UtcNowStub()
    {
        return _utcNow;
    }
}