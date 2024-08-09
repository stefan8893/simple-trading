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
    public void You_cant_close_a_trade_before_it_was_opened()
    {
        // arrange
        var opened = _utcNow.AddHours(-2);
        var closed = _utcNow.AddHours(-3);

        var trade = (TestData.Trade.Default with {Opened = opened}).Build();
        var closeTradeDto = new Trade.CloseTradeDto(Result.Win, 500m, 1.05m, closed, UtcNowStub, "Europe/Vienna");

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        var businessError = response.Value.Should().BeOfType<BusinessError>();
        businessError.Which.ResourceId.Should().Be(trade.Id);
        businessError.Which.Reason.Should().Be("The 'Closed' date must be after the 'Opened' date.");
    }

    [Fact]
    public void The_closed_date_cannot_be_greater_than_one_day_in_the_future()
    {
        // arrange
        var opened = _utcNow.AddHours(-2);
        var closed = _utcNow.AddDays(1).AddSeconds(1);

        var trade = (TestData.Trade.Default with {Opened = opened}).Build();
        var closeTradeDto = new Trade.CloseTradeDto(Result.Win, 500m, 1.05m, closed, UtcNowStub, "Europe/Vienna");

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        var businessError = response.Value.Should().BeOfType<BusinessError>();
        businessError.Which.ResourceId.Should().Be(trade.Id);
        businessError.Which.Reason.Should().Be("The 'Closed' date must not be greater than one day in the future.");
    }

    [Fact]
    public void The_closed_date_can_at_maximum_one_day_in_the_future()
    {
        // arrange
        var opened = _utcNow.AddHours(-2);
        var closed = _utcNow.AddDays(1);

        var trade = (TestData.Trade.Default with {Opened = opened}).Build();
        var closeTradeDto = new Trade.CloseTradeDto(Result.Win, 500m, 1.05m, closed, UtcNowStub, "Europe/Vienna");

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>();
    }

    [Fact]
    public void Already_closed_trades_cant_be_closed_again()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            Opened = _utcNow,
            Closed = _utcNow,
            PositionPrices = TestData.PositionPrices.Default with {ExitPrice = 1.05m},
            Outcome = new Outcome {Balance = 500, Result = Result.Win}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(Result.Win, 500m, 1.05m, _utcNow, UtcNowStub, Constants.DefaultTimeZone);

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        var businessError = response.Value.Should().BeOfType<BusinessError>();
        businessError.Which.ResourceId.Should().Be(trade.Id);
        businessError.Which.Reason.Should().Be("The trade has already been closed on '04.08.2024 14:00:00'.");
    }

    [Theory]
    [InlineData(Result.Win)]
    [InlineData(Result.Loss)]
    [InlineData(Result.Mediocre)]
    public void Zero_Balance_with_results_other_than_BreakEven_does_not_make_sense(Result invalidResult)
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var closeTradeDto =
            new Trade.CloseTradeDto(invalidResult, 0m, 1.05m, _utcNow, UtcNowStub, Constants.DefaultTimeZone);

        // act
        var response = trade.Close(closeTradeDto);

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
        var closeTradeDto =
            new Trade.CloseTradeDto(invalidResult, -1m, 1.05m, _utcNow, UtcNowStub, Constants.DefaultTimeZone);

        // act
        var response = trade.Close(closeTradeDto);

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
        var closeTradeDto =
            new Trade.CloseTradeDto(invalidResult, 1m, 1.05m, _utcNow, UtcNowStub, Constants.DefaultTimeZone);

        // act
        var response = trade.Close(closeTradeDto);

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