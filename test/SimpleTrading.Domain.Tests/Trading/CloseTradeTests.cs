using FluentAssertions;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading;

public class CloseTradeTests : TestBase
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-08-04T12:00").ToUtcKind();

    [Fact]
    public void You_cant_close_a_trade_before_it_was_opened()
    {
        // arrange
        var opened = _utcNow.AddHours(-2);
        var closed = _utcNow.AddHours(-3);

        var trade = (TestData.Trade.Default with {Opened = opened}).Build();
        var closeTradeDto = new Trade.CloseTradeDto(closed, 500m, UtcNowStub)
        {
            ExitPrice = 1.05m,
            Result = ResultModel.Win
        };

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        var businessError = response.Value.Should().BeOfType<BusinessError>();
        businessError.Which.ResourceId.Should().Be(trade.Id);
        businessError.Which.Reason.Should().Be("'Closed' must be after 'Opened'.");
    }

    [Fact]
    public void A_trade_is_considered_as_closed_when_closed_date_and_balance_are_specified()
    {
        var trade = (TestData.Trade.Default with {Opened = _utcNow, Closed = _utcNow, Balance = 50m}).Build();

        var isClosed = trade.IsClosed;

        isClosed.Should().BeTrue();
    }

    [Fact]
    public void The_closed_date_cannot_be_greater_than_one_day_in_the_future()
    {
        // arrange
        var opened = _utcNow;
        var closed = _utcNow.AddDays(1).AddSeconds(1);

        var trade = (TestData.Trade.Default with {Opened = opened}).Build();
        var closeTradeDto = new Trade.CloseTradeDto(closed, 500m, UtcNowStub)
        {
            ExitPrice = 1.05m,
            Result = ResultModel.Win
        };

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        var businessError = response.Value.Should().BeOfType<BusinessError>();
        businessError.Which.ResourceId.Should().Be(trade.Id);
        businessError.Which.Reason.Should().Be("'Closed' must not be greater than one day in the future.");
    }

    [Fact]
    public void The_closed_date_can_at_maximum_one_day_in_the_future()
    {
        // arrange
        var opened = _utcNow;
        var closed = _utcNow.AddDays(1);

        var trade = (TestData.Trade.Default with {Opened = opened}).Build();
        var closeTradeDto = new Trade.CloseTradeDto(closed, 500m, UtcNowStub)
        {
            ExitPrice = 1.05m
        };

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>();
    }

    [Fact]
    public void
        The_result_gets_calculated_by_the_balance_if_ExitPrice_SL_and_TP_are_missing_and_the_user_has_not_entered_the_result_manually()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 0m, UtcNowStub);

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>();
        trade.IsClosed.Should().BeTrue();
        trade.Result.Should().NotBeNull();
        trade.Result!.Name.Should().Be(Result.BreakEven);
        trade.Result!.Source.Should().Be(ResultSource.CalculatedByBalance);
    }

    [Fact]
    public void
        The_result_gets_calculated_by_the_balance_if_SL_and_TP_are_missing_and_the_user_has_not_entered_the_result_manually()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with {ExitPrice = 0.1m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, -10m, UtcNowStub);

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>();
        trade.IsClosed.Should().BeTrue();
        trade.Result.Should().NotBeNull();
        trade.Result!.Name.Should().Be(Result.Loss);
        trade.Result!.Source.Should().Be(ResultSource.CalculatedByBalance);
    }

    [Fact]
    public void
        If_the_balance_is_zero_and_the_entry_and_exit_prices_are_not_equal_the_trade_gets_closed_but_returns_a_warning()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with {EntryPrice = 1.1m, ExitPrice = 1.0m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 0m, UtcNowStub);

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>()
            .Which.Warnings.Should().HaveCount(1)
            .And.Contain(x => x.Reason == "The balance is 0, but the position indicates a balance not equal to 0.");
        trade.IsClosed.Should().BeTrue();
    }

    [Fact]
    public void
        If_the_balance_is_below_zero_and_the_entry_and_exit_prices_are_equal_the_trade_gets_closed_but_returns_a_warning()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with {EntryPrice = 1.1m, ExitPrice = 1.1m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, -10m, UtcNowStub);

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>()
            .Which.Warnings.Should().HaveCount(1)
            .And.Contain(x => x.Reason == "The balance is not 0, but the position indicates a balance equal to 0.");
        trade.IsClosed.Should().BeTrue();
    }

    [Fact]
    public void A_short_position_can_be_successfully_closed()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with {EntryPrice = 1.1m, ExitPrice = 1.4m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, -10m, UtcNowStub);

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>();
        trade.IsClosed.Should().BeTrue();
    }

    [Fact]
    public void
        The_result_gets_calculated_by_the_balance_if_ExitPrice_and_SL_are_set_and_the_user_has_not_entered_the_result_manually()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with {StopLoss = 1.0m, EntryPrice = 1.1m, ExitPrice = 1.1m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 0m, UtcNowStub);

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>();
        trade.IsClosed.Should().BeTrue();
        trade.Result.Should().NotBeNull();
        trade.Result!.Name.Should().Be(Result.BreakEven);
        trade.Result!.Source.Should().Be(ResultSource.CalculatedByBalance);
    }

    [Fact]
    public void
        The_result_gets_calculated_by_the_position_prices_if_ExitPrice_and_TP_are_set_and_the_user_has_not_entered_the_result_manually()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.1m, ExitPrice = 1.2m, TakeProfit = 1.4m
            }
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 10m, UtcNowStub);

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>();
        trade.IsClosed.Should().BeTrue();
        trade.Result.Should().NotBeNull();
        trade.Result!.Name.Should().Be(Result.Mediocre);
        trade.Result!.Source.Should().Be(ResultSource.CalculatedByPositionPrices);
    }

    [Fact]
    public void A_BreakEven_result_given_as_input_overrides_all_calculated_values()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 0m, UtcNowStub)
        {
            Result = ResultModel.BreakEven
        };

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>();
        trade.Result.Should().NotBeNull();
        trade.Result!.Name.Should().Be(Result.BreakEven);
        trade.Result!.Source.Should().Be(ResultSource.ManuallyEntered);
    }

    [Fact]
    public void
        A_BreakEven_result_given_as_input_overrides_all_calculated_results_but_returns_a_warning_because_the_result_differs_from_the_calculated_by_balance_result()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, -10m, UtcNowStub)
        {
            Result = ResultModel.BreakEven
        };

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>()
            .Which.Warnings.Should().HaveCount(1)
            .And.Contain(x => x.Reason == "Your trade indicates a 'Loss' result, but you have entered 'Break-even'.");
        trade.Result.Should().NotBeNull();
        trade.Result!.Name.Should().Be(Result.BreakEven);
        trade.Result!.Source.Should().Be(ResultSource.ManuallyEntered);
    }

    [Fact]
    public void
        A_BreakEven_result_given_as_input_overrides_all_calculated_results_and_a_warning_gets_returned_because_the_result_differs_from_the_calculated_by_position_prices_result()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.1m, StopLoss = 1.2m, TakeProfit = 0.9m
            }
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 10m, UtcNowStub)
        {
            Result = ResultModel.BreakEven,
            ExitPrice = 0.9m
        };

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>()
            .Which.Warnings.Should().HaveCount(1)
            .And.Contain(x => x.Reason == "Your trade indicates a 'Win' result, but you have entered 'Break-even'.");
        trade.Result.Should().NotBeNull();
        trade.Result!.Name.Should().Be(Result.BreakEven);
        trade.Result!.Source.Should().Be(ResultSource.ManuallyEntered);
    }

    [Fact]
    public void
        A_Mediocre_result_given_as_input_overrides_all_calculated_results_but_returns_a_warning_because_the_result_differs_from_the_calculated_by_position_prices_result()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m
            }
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, -10m, UtcNowStub)
        {
            Result = ResultModel.Mediocre,
            ExitPrice = 1.0m
        };

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>()
            .Which.Warnings.Should().HaveCount(1)
            .And.Contain(x => x.Reason == "Your trade indicates a 'Loss' result, but you have entered 'Mediocre'.");
        trade.Result.Should().NotBeNull();
        trade.Result!.Name.Should().Be(Result.Mediocre);
        trade.Result!.Source.Should().Be(ResultSource.ManuallyEntered);
    }

    [Fact]
    public void
        The_result_gets_calculated_by_the_positions_prices_even_the_SL_is_missing_but_only_if_it_equal_to_the_balance_result()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.1m, ExitPrice = 1.1m, TakeProfit = 1.4m
            }
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 0m, UtcNowStub);

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>();
        trade.IsClosed.Should().BeTrue();
        trade.Result.Should().NotBeNull();
        trade.Result!.Name.Should().Be(Result.BreakEven);
        trade.Result!.Source.Should().Be(ResultSource.CalculatedByPositionPrices);
    }

    [Fact]
    public void
        The_result_gets_calculated_by_the_position_prices_if_SL_is_missing_and_remaining_prices_indicate_a_win_result()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.1m, ExitPrice = 1.4m, TakeProfit = 1.4m
            }
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 10m, UtcNowStub);

        // act
        var _ = trade.Close(closeTradeDto);

        // assert
        trade.IsClosed.Should().BeTrue();
        trade.Result.Should().NotBeNull();
        trade.Result!.Name.Should().Be(Result.Win);
        trade.Result!.Source.Should().Be(ResultSource.CalculatedByPositionPrices);
    }

    [Fact]
    public void The_result_get_calculated_by_the_position_prices_if_SL_TP_and_ExitPrice_are_set()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m, ExitPrice = 1.0m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, -10m, UtcNowStub);

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>();
        trade.Result!.Name.Should().Be(Result.Loss);
        trade.Result!.Source.Should().Be(ResultSource.CalculatedByPositionPrices);
    }

    [Fact]
    public void PositionPrices_are_present_and_lead_to_a_minus_50_percent_loss_result()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m, ExitPrice = 1.05m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, -10m, UtcNowStub);

        // act
        var _ = trade.Close(closeTradeDto);

        // assert
        trade.Result!.Name.Should().Be(Result.Loss);
        trade.Result!.Source.Should().Be(ResultSource.CalculatedByPositionPrices);
        trade.Result!.Performance.Should().Be(-50);
    }

    [Fact]
    public void PositionPrices_are_present_and_lead_to_a_minus_150_percent_loss_result()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m, ExitPrice = 0.95m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, -10m, UtcNowStub);

        // act
        var _ = trade.Close(closeTradeDto);

        // assert
        trade.Result!.Name.Should().Be(Result.Loss);
        trade.Result!.Source.Should().Be(ResultSource.CalculatedByPositionPrices);
        trade.Result!.Performance.Should().Be(-150);
    }

    [Fact]
    public void PositionPrices_are_present_and_lead_to_a_25_percent_mediocre_result()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m, ExitPrice = 1.175m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 5m, UtcNowStub);

        // act
        var _ = trade.Close(closeTradeDto);

        // assert
        trade.Result!.Name.Should().Be(Result.Mediocre);
        trade.Result!.Source.Should().Be(ResultSource.CalculatedByPositionPrices);
        trade.Result!.Performance.Should().Be(25);
    }

    [Fact]
    public void PositionPrices_are_present_and_lead_to_a_99_percent_mediocre_result()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m, ExitPrice = 1.397m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 30m, UtcNowStub);

        // act
        var _ = trade.Close(closeTradeDto);

        // assert
        trade.Result!.Name.Should().Be(Result.Mediocre);
        trade.Result!.Source.Should().Be(ResultSource.CalculatedByPositionPrices);
        trade.Result!.Performance.Should().Be(99);
    }

    [Fact]
    public void PositionPrices_are_present_and_lead_to_a_100_percent_win_result()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m, ExitPrice = 1.4m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 30m, UtcNowStub);

        // act
        var _ = trade.Close(closeTradeDto);

        // assert
        trade.Result!.Name.Should().Be(Result.Win);
        trade.Result!.Source.Should().Be(ResultSource.CalculatedByPositionPrices);
        trade.Result!.Performance.Should().Be(100);
    }

    [Fact]
    public void PositionPrices_are_present_and_lead_to_a_120_percent_win_result()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m, ExitPrice = 1.46m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 30m, UtcNowStub);

        // act
        var _ = trade.Close(closeTradeDto);

        // assert
        trade.Result!.Name.Should().Be(Result.Win);
        trade.Result!.Source.Should().Be(ResultSource.CalculatedByPositionPrices);
        trade.Result!.Performance.Should().Be(120);
    }

    [Fact]
    public void A_loss_result_calculated_by_position_prices_and_a_positive_balances_is_acceptable_but_returns_warnings()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 30m, UtcNowStub)
        {
            Balance = 10m,
            ExitPrice = 1.0m
        };

        // act
        var response = trade.Close(closeTradeDto);

        response.Value.Should().BeOfType<Completed>()
            .Which.Warnings.Should().HaveCount(1);
    }

    [Fact]
    public void
        If_position_prices_indicate_a_mediocre_result_but_balance_is_negative_the_trade_gets_negatively_closed_but_warnings_are_returned()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.1m, TakeProfit = 1.4m, ExitPrice = 1.25m
            }
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, -30m, UtcNowStub);

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        trade.IsClosed.Should().BeTrue();
        trade.Result!.Name.Should().Be(Result.Loss);
        response.Value.Should().BeOfType<Completed>()
            .Which.Warnings.Should().HaveCount(1)
            .And.Contain(x =>
                x.Reason == "Your position indicates the result 'Mediocre', but based on the balance it is 'Loss'.");
    }

    [Fact]
    public void
        If_position_prices_indicate_a_loss_but_balance_is_positive_the_trade_gets_closed_without_a_result_and_a_warning_gets_returned()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.2m, TakeProfit = 0.8m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 30m, UtcNowStub)
        {
            ExitPrice = 1.2m
        };

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        trade.Result.Should().BeNull();
        response.Value.Should().BeOfType<Completed>()
            .Which.Warnings.Should().HaveCount(1)
            .And.Contain(x => x.Reason == "The balance is positive, but your short position indicates a loss.");
    }

    [Fact]
    public void
        If_position_prices_indicate_a_loss_of_a_long_position_but_balance_is_positive_the_trade_gets_closed_without_a_result_and_a_warning_gets_returned()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.8m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 30m, UtcNowStub)
        {
            ExitPrice = 1.0m
        };

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        trade.Result.Should().BeNull();
        response.Value.Should().BeOfType<Completed>()
            .Which.Warnings.Should().HaveCount(1)
            .And.Contain(x => x.Reason == "The balance is positive, but your long position indicates a loss.");
    }

    [Fact]
    public void Performance_is_zero_if_the_result_break_even_calculated_by_balance()
    {
        var trade = TestData.Trade.Default.Build();
        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 0, UtcNowStub);

        var response = trade.Close(closeTradeDto);

        response.Value.Should().BeOfType<Completed>();
        trade.Result.Should().NotBeNull();
        trade.Result!.Performance.Should().Be(0);
    }

    [Fact]
    public void
        A_short_position_with_an_exit_price_below_entry_and_a_negative_balance_has_no_result_and_a_warning_gets_returned()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.0m, StopLoss = 1.1m, TakeProfit = 0.7m
            }
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, -10m, UtcNowStub)
        {
            ExitPrice = 0.9m
        };

        // act
        var response = trade.Close(closeTradeDto);

        //assert
        response.Value.Should().BeOfType<Completed>()
            .Which.Warnings.Should().HaveCount(1)
            .And.Contain(x =>
                x.Reason == "Your position indicates the result 'Mediocre', but based on the balance it is 'Loss'.");
    }

    [Fact]
    public void
        The_given_result_is_BreakEven_the_balance_is_negative_and_the_position_prices_indicate_a_mediocre_result_in_this_case_the_given_result_is_taken_and_warnings_will_be_returned()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices {EntryPrice = 1m, StopLoss = 0.95m, TakeProfit = 1.4m}
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, -10m, UtcNowStub)
        {
            ExitPrice = 1.1m,
            Result = ResultModel.BreakEven
        };

        // act
        var response = trade.Close(closeTradeDto);

        // assert
        response.Value.Should().BeOfType<Completed>()
            .Which.Warnings.Should().HaveCount(2)
            .And.Contain(x => x.Reason == "Your trade indicates a 'Loss' result, but you have entered 'Break-even'.")
            .And.Contain(x =>
                x.Reason == "Your position indicates the result 'Mediocre', but based on the balance it is 'Loss'.");
    }

    [Fact]
    public void
        Long_position_indicates_a_mediocre_result_but_the_balance_is_zero_the_trade_gets_closed_as_BreakEven_but_a_warning_gets_returned()
    {
        // arrange        
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.0m, StopLoss = 0.9m, TakeProfit = 1.3m
            }
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 0, UtcNowStub)
        {
            ExitPrice = 1.25m
        };

        // act
        var response = trade.Close(closeTradeDto);


        // assert
        response.Value.Should().BeOfType<Completed>()
            .Which.Warnings.Should().HaveCount(1)
            .And.Contain(x =>
                x.Reason ==
                "Your position indicates the result 'Mediocre', but based on the balance it is 'Break-even'.");
    }

    [Fact]
    public void
        Long_position_indicates_a_loss_result_but_the_balance_is_zero_the_trade_gets_closed_as_BreakEven_but_a_warning_gets_returned()
    {
        // arrange        
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.0m, StopLoss = 0.9m, TakeProfit = 1.3m
            }
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 0, UtcNowStub)
        {
            ExitPrice = 0.9m
        };

        // act
        var response = trade.Close(closeTradeDto);


        // assert
        response.Value.Should().BeOfType<Completed>()
            .Which.Warnings.Should().HaveCount(1)
            .And.Contain(x =>
                x.Reason == "Your position indicates the result 'Loss', but based on the balance it is 'Break-even'.");
    }

    [Fact]
    public void
        Short_position_indicates_a_win_result_but_the_balance_is_zero_the_trade_gets_closed_as_BreakEven_but_a_warning_gets_returned()
    {
        // arrange        
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.0m, StopLoss = 1.1m, TakeProfit = 0.7m
            }
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 0, UtcNowStub)
        {
            ExitPrice = 0.7m
        };

        // act
        var response = trade.Close(closeTradeDto);


        // assert
        response.Value.Should().BeOfType<Completed>()
            .Which.Warnings.Should().HaveCount(1)
            .And.Contain(x =>
                x.Reason == "Your position indicates the result 'Win', but based on the balance it is 'Break-even'.");
    }

    [Fact]
    public void
        Short_position_indicates_a_loss_result_but_the_balance_is_zero_the_trade_gets_closed_as_BreakEven_but_a_warning_gets_returned()
    {
        // arrange        
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.0m, StopLoss = 1.1m, TakeProfit = 0.7m
            }
        }).Build();

        var closeTradeDto = new Trade.CloseTradeDto(_utcNow, 0, UtcNowStub)
        {
            ExitPrice = 1.1m
        };

        // act
        var response = trade.Close(closeTradeDto);


        // assert
        response.Value.Should().BeOfType<Completed>()
            .Which.Warnings.Should().HaveCount(1)
            .And.Contain(x =>
                x.Reason == "Your position indicates the result 'Loss', but based on the balance it is 'Break-even'.");
    }

    private DateTime UtcNowStub()
    {
        return _utcNow;
    }
}