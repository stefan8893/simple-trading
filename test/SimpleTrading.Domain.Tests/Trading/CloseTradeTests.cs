using OneOf.Types;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading;

public class CloseTradeTests : TestBase
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-08-04T12:00").ToUtcKind();

    [Fact]
    public void You_cant_finish_a_trade_before_it_was_opened()
    {
        var opened = _utcNow.AddHours(-2);
        var finished = _utcNow.AddHours(-3);

        var trade = (TestData.Trade.Default with {Opened = opened}).Build();
        var finishTradeDto = new FinishTradeConfiguration(finished, 500m, UtcNowStub)
        {
            ExitPrice = 1.05m,
            ManuallyEnteredResult = ResultModel.Win
        };

        var response = trade.Finish(finishTradeDto);

        var conflict = Assert.IsType<Conflict>(response.Value);
        Assert.Equal(trade.Id, conflict.ResourceId);
        Assert.Equal("'Finished' must be after 'Opened'.", conflict.Details);
    }

    [Fact]
    public void A_trade_is_considered_as_finished_when_finished_date_and_profitLoss_are_specified()
    {
        var trade = (TestData.Trade.Default with {Opened = _utcNow, Finished = _utcNow, ProfitLoss = 50m}).Build();

        var isFinished = trade.IsFinished;

        Assert.True(isFinished);
    }

    [Fact]
    public void The_finished_date_cannot_be_greater_than_one_day_in_the_future()
    {
        var opened = _utcNow;
        var finished = _utcNow.AddDays(1).AddSeconds(1);

        var trade = (TestData.Trade.Default with {Opened = opened}).Build();
        var finishTradeDto = new FinishTradeConfiguration(finished, 500m, UtcNowStub)
        {
            ExitPrice = 1.05m,
            ManuallyEnteredResult = ResultModel.Win
        };

        var response = trade.Finish(finishTradeDto);

        var conflict = Assert.IsType<Conflict>(response.Value);

        Assert.Equal(trade.Id, conflict.ResourceId);
        Assert.Equal("'Finished' must not be greater than one day in the future.", conflict.Details);
    }

    [Fact]
    public void The_finished_date_can_at_maximum_one_day_in_the_future_for_trades_that_were_opened_in_the_past()
    {
        var opened = _utcNow.AddHours(-5);
        var finished = opened.AddDays(1);

        var trade = (TestData.Trade.Default with {Opened = opened}).Build();
        var finishTradeDto = new FinishTradeConfiguration(finished, 500m, UtcNowStub)
        {
            ExitPrice = 1.05m
        };

        var response = trade.Finish(finishTradeDto);

        Assert.IsType<Completed<FinishTradeResult>>(response.Value);
    }

    [Fact]
    public void
        The_finished_date_can_at_maximum_one_day_in_the_future_based_on_the_opened_date_if_the_trade_was_opened_in_the_future()
    {
        var opened = _utcNow.AddHours(5);
        var finished = opened.AddDays(1);

        var trade = (TestData.Trade.Default with {Opened = opened}).Build();
        var finishTradeDto = new FinishTradeConfiguration(finished, 500m, UtcNowStub)
        {
            ExitPrice = 1.05m
        };

        var response = trade.Finish(finishTradeDto);

        Assert.IsType<Completed<FinishTradeResult>>(response.Value);
    }

    [Fact]
    public void
        The_result_gets_calculated_by_the_profitLoss_if_ExitPrice_SL_and_TP_are_missing_and_the_user_has_not_entered_the_result_manually()
    {
        var trade = TestData.Trade.Default.Build();
        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 0m, UtcNowStub);

        var response = trade.Finish(finishTradeDto);

        Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        Assert.True(trade.IsFinished);
        Assert.Equal(Result.BreakEven, trade.Result?.Name);
        Assert.Equal(ResultSource.CalculatedByProfitLoss, trade.Result?.Source);
    }

    [Fact]
    public void
        The_result_gets_calculated_by_the_profitLoss_if_SL_and_TP_are_missing_and_the_user_has_not_entered_the_result_manually()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with {ExitPrice = 0.1m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, -10m, UtcNowStub);

        var response = trade.Finish(finishTradeDto);

        Assert.IsType<Completed<FinishTradeResult>>(response.Value);

        Assert.True(trade.IsFinished);
        Assert.Equal(Result.Loss, trade.Result?.Name);
        Assert.Equal(ResultSource.CalculatedByProfitLoss, trade.Result?.Source);
    }

    [Fact]
    public void
        If_the_profitLoss_is_zero_and_the_entry_and_exit_prices_are_not_equal_the_trade_gets_finished_but_returns_a_warning()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with {EntryPrice = 1.1m, ExitPrice = 1.0m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 0m, UtcNowStub);

        var response = trade.Finish(finishTradeDto);

        var result = Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        var singleWarning = Assert.Single(result.Data.Warnings);
        Assert.Equal("Profit/loss is 0, but the position indicates a profit or a loss.", singleWarning);
        Assert.True(trade.IsFinished);
    }

    [Fact]
    public void
        If_the_profitLoss_is_below_zero_and_the_entry_and_exit_prices_are_equal_the_trade_gets_finished_but_returns_a_warning()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with {EntryPrice = 1.1m, ExitPrice = 1.1m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, -10m, UtcNowStub);

        var response = trade.Finish(finishTradeDto);

        var result = Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        var singleWarning = Assert.Single(result.Data.Warnings);
        Assert.Equal("Profit/loss is not 0, but the position indicates a break-even trade.", singleWarning);
        Assert.True(trade.IsFinished);
    }

    [Fact]
    public void A_short_position_can_be_successfully_finished()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with {EntryPrice = 1.1m, ExitPrice = 1.4m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, -10m, UtcNowStub);

        var response = trade.Finish(finishTradeDto);

        Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        Assert.True(trade.IsFinished);
    }

    [Fact]
    public void
        The_result_gets_calculated_by_the_profitLoss_if_ExitPrice_and_SL_are_specified_and_the_user_has_not_entered_the_result_manually()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with {StopLoss = 1.0m, EntryPrice = 1.1m, ExitPrice = 1.1m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 0m, UtcNowStub);

        var response = trade.Finish(finishTradeDto);

        Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        Assert.True(trade.IsFinished);
        Assert.Equal(Result.BreakEven, trade.Result?.Name);
        Assert.Equal(ResultSource.CalculatedByProfitLoss, trade.Result?.Source);
    }

    [Fact]
    public void
        The_result_gets_calculated_by_the_position_prices_if_ExitPrice_and_TP_are_specified_and_the_user_has_not_entered_the_result_manually()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.1m, ExitPrice = 1.2m, TakeProfit = 1.4m
            }
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 10m, UtcNowStub);

        var response = trade.Finish(finishTradeDto);

        Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        Assert.True(trade.IsFinished);
        Assert.Equal(Result.Mediocre, trade.Result?.Name);
        Assert.Equal(ResultSource.CalculatedByPositionPrices, trade.Result?.Source);
    }

    [Fact]
    public void A_BreakEven_result_given_as_input_overrides_all_calculated_values()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 0m, UtcNowStub)
        {
            ManuallyEnteredResult = ResultModel.BreakEven
        };

        var response = trade.Finish(finishTradeDto);

        Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        Assert.Equal(Result.BreakEven, trade.Result?.Name);
        Assert.Equal(ResultSource.ManuallyEntered, trade.Result?.Source);
    }

    [Fact]
    public void
        A_BreakEven_result_given_as_input_overrides_all_calculated_results_but_returns_a_warning_because_the_result_differs_from_the_calculated_by_profitLoss_result()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, -10m, UtcNowStub)
        {
            ManuallyEnteredResult = ResultModel.BreakEven
        };

        var response = trade.Finish(finishTradeDto);

        var result = Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        var singleWarning = Assert.Single(result.Data.Warnings);
        Assert.Equal("Your trade indicates a 'Loss' result, but you have entered 'Break-Even'.", singleWarning);
        Assert.Equal(Result.BreakEven, trade.Result?.Name);
        Assert.Equal(ResultSource.ManuallyEntered, trade.Result?.Source);
    }

    [Fact]
    public void
        A_BreakEven_result_given_as_input_overrides_all_calculated_results_and_a_warning_gets_returned_because_the_result_differs_from_the_calculated_by_position_prices_result()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.1m, StopLoss = 1.2m, TakeProfit = 0.9m
            }
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 10m, UtcNowStub)
        {
            ManuallyEnteredResult = ResultModel.BreakEven,
            ExitPrice = 0.9m
        };

        var response = trade.Finish(finishTradeDto);

        var result = Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        var singleWarning = Assert.Single(result.Data.Warnings);
        Assert.Equal("Your trade indicates a 'Win' result, but you have entered 'Break-Even'.", singleWarning);
        Assert.Equal(Result.BreakEven, trade.Result?.Name);
        Assert.Equal(ResultSource.ManuallyEntered, trade.Result?.Source);
    }

    [Fact]
    public void
        A_Mediocre_result_given_as_input_overrides_all_calculated_results_but_returns_a_warning_because_the_result_differs_from_the_calculated_by_position_prices_result()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m
            }
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, -10m, UtcNowStub)
        {
            ManuallyEnteredResult = ResultModel.Mediocre,
            ExitPrice = 1.0m
        };

        var response = trade.Finish(finishTradeDto);

        var result = Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        var singleWarning = Assert.Single(result.Data.Warnings);
        Assert.Equal("Your trade indicates a 'Loss' result, but you have entered 'Mediocre'.", singleWarning);
        Assert.Equal(Result.Mediocre, trade.Result?.Name);
        Assert.Equal(ResultSource.ManuallyEntered, trade.Result?.Source);
    }

    [Fact]
    public void
        The_result_gets_calculated_by_the_positions_prices_even_the_SL_is_missing_but_only_if_it_equal_to_the_profitLoss_result()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.1m, ExitPrice = 1.1m, TakeProfit = 1.4m
            }
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 0m, UtcNowStub);

        var response = trade.Finish(finishTradeDto);

        Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        Assert.True(trade.IsFinished);
        Assert.Equal(Result.BreakEven, trade.Result?.Name);
        Assert.Equal(ResultSource.CalculatedByPositionPrices, trade.Result?.Source);
    }

    [Fact]
    public void
        The_result_gets_calculated_by_the_position_prices_if_SL_is_missing_and_remaining_prices_indicate_a_win_result()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.1m, ExitPrice = 1.4m, TakeProfit = 1.4m
            }
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 10m, UtcNowStub);

        _ = trade.Finish(finishTradeDto);

        Assert.True(trade.IsFinished);
        Assert.Equal(Result.Win, trade.Result?.Name);
        Assert.Equal(ResultSource.CalculatedByPositionPrices, trade.Result?.Source);
    }

    [Fact]
    public void The_result_get_calculated_by_the_position_prices_if_SL_TP_and_ExitPrice_are_specified()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m, ExitPrice = 1.0m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, -10m, UtcNowStub);

        var response = trade.Finish(finishTradeDto);

        Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        Assert.Equal(Result.Loss, trade.Result?.Name);
        Assert.Equal(ResultSource.CalculatedByPositionPrices, trade.Result?.Source);
    }

    [Fact]
    public void PositionPrices_are_present_and_lead_to_a_minus_50_percent_loss_result()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m, ExitPrice = 1.05m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, -10m, UtcNowStub);

        _ = trade.Finish(finishTradeDto);

        Assert.Equal(Result.Loss, trade.Result?.Name);
        Assert.Equal(ResultSource.CalculatedByPositionPrices, trade.Result?.Source);
        Assert.Equal((short) -50, trade.Result?.Performance);
    }

    [Fact]
    public void PositionPrices_are_present_and_lead_to_a_minus_150_percent_loss_result()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m, ExitPrice = 0.95m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, -10m, UtcNowStub);

        _ = trade.Finish(finishTradeDto);

        Assert.Equal(Result.Loss, trade.Result?.Name);
        Assert.Equal(ResultSource.CalculatedByPositionPrices, trade.Result?.Source);
        Assert.Equal((short) -150, trade.Result?.Performance);
    }

    [Fact]
    public void PositionPrices_are_present_and_lead_to_a_25_percent_mediocre_result()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m, ExitPrice = 1.175m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 5m, UtcNowStub);

        _ = trade.Finish(finishTradeDto);

        Assert.Equal(Result.Mediocre, trade.Result?.Name);
        Assert.Equal(ResultSource.CalculatedByPositionPrices, trade.Result?.Source);
        Assert.Equal((short) 25, trade.Result?.Performance);
    }

    [Fact]
    public void PositionPrices_are_present_and_lead_to_a_99_percent_mediocre_result()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m, ExitPrice = 1.397m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 30m, UtcNowStub);

        _ = trade.Finish(finishTradeDto);

        Assert.Equal(Result.Mediocre, trade.Result?.Name);
        Assert.Equal(ResultSource.CalculatedByPositionPrices, trade.Result?.Source);
        Assert.Equal((short) 99, trade.Result?.Performance);
    }

    [Fact]
    public void PositionPrices_are_present_and_lead_to_a_100_percent_win_result()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m, ExitPrice = 1.4m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 30m, UtcNowStub);

        _ = trade.Finish(finishTradeDto);

        Assert.Equal(Result.Win, trade.Result?.Name);
        Assert.Equal(ResultSource.CalculatedByPositionPrices, trade.Result?.Source);
        Assert.Equal((short) 100, trade.Result?.Performance);
    }

    [Fact]
    public void PositionPrices_are_present_and_lead_to_a_120_percent_win_result()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m, ExitPrice = 1.46m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 30m, UtcNowStub);

        _ = trade.Finish(finishTradeDto);

        Assert.Equal(Result.Win, trade.Result?.Name);
        Assert.Equal(ResultSource.CalculatedByPositionPrices, trade.Result?.Source);
        Assert.Equal((short) 120, trade.Result?.Performance);
    }

    [Fact]
    public void
        A_loss_result_calculated_by_position_prices_and_a_positive_profitLosss_is_acceptable_but_returns_warnings()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.4m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 30m, UtcNowStub)
        {
            ProfitLoss = 10m,
            ExitPrice = 1.0m
        };

        var response = trade.Finish(finishTradeDto);

        var result = Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        var singleWarning = Assert.Single(result.Data.Warnings);
        Assert.Equal("You have entered a profit, but your long position indicates a loss.", singleWarning);
    }

    [Fact]
    public void
        If_position_prices_indicate_a_mediocre_result_but_profitLoss_is_negative__the_trade_gets_negatively_finished_and_a_warnings_is_returned()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.1m, TakeProfit = 1.4m, ExitPrice = 1.25m
            }
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, -30m, UtcNowStub);

        var response = trade.Finish(finishTradeDto);

        var result = Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        var singleWarning = Assert.Single(result.Data.Warnings);
        Assert.Equal("Your position indicates the result 'Mediocre', but based on your Profit/Loss it is 'Loss'.",
            singleWarning);

        Assert.True(trade.IsFinished);
        Assert.Equal(Result.Loss, trade.Result?.Name);
    }

    [Fact]
    public void
        If_position_prices_indicate_a_loss_but_profitLoss_is_positive__the_trade_gets_finished_without_a_result_and_a_warning_is_returned()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.2m, TakeProfit = 0.8m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 30m, UtcNowStub)
        {
            ExitPrice = 1.2m
        };

        var response = trade.Finish(finishTradeDto);

        var result = Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        var singleWarning = Assert.Single(result.Data.Warnings);
        Assert.Equal("You have entered a profit, but your short position indicates a loss.", singleWarning);

        Assert.Null(trade.Result);
    }

    [Fact]
    public void
        If_position_prices_indicate_a_loss_of_a_long_position_but_profitLoss_is_positive__the_trade_gets_finished_without_a_result_and_a_warning_is_returned()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices
                {EntryPrice = 1.1m, StopLoss = 1.0m, TakeProfit = 1.8m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 30m, UtcNowStub)
        {
            ExitPrice = 1.0m
        };

        var response = trade.Finish(finishTradeDto);

        var result = Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        var singleWarning = Assert.Single(result.Data.Warnings);
        Assert.Equal("You have entered a profit, but your long position indicates a loss.", singleWarning);

        Assert.Null(trade.Result);
    }

    [Fact]
    public void Performance_is_zero_if_the_result_break_even_calculated_by_profitLoss()
    {
        var trade = TestData.Trade.Default.Build();
        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 0, UtcNowStub);

        var response = trade.Finish(finishTradeDto);

        Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        Assert.Equal((short) 0, trade.Result?.Performance);
    }

    [Fact]
    public void
        A_short_position_with_an_exit_price_below_entry_and_a_negative_profitLoss_has_no_result_and_a_warning_is_returned()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.0m, StopLoss = 1.1m, TakeProfit = 0.7m
            }
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, -10m, UtcNowStub)
        {
            ExitPrice = 0.9m
        };

        var response = trade.Finish(finishTradeDto);

        //assert
        var result = Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        var singleWarning = Assert.Single(result.Data.Warnings);
        Assert.Equal("Your position indicates the result 'Mediocre', but based on your Profit/Loss it is 'Loss'.",
            singleWarning);
    }

    [Fact]
    public void
        The_given_result_is_BreakEven_the_profitLoss_is_negative_and_the_position_prices_indicate_a_mediocre_result_in_this_case_the_given_result_is_taken_and_warnings_will_be_returned()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = new TestData.PositionPrices {EntryPrice = 1m, StopLoss = 0.95m, TakeProfit = 1.4m}
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, -10m, UtcNowStub)
        {
            ExitPrice = 1.1m,
            ManuallyEnteredResult = ResultModel.BreakEven
        };

        var response = trade.Finish(finishTradeDto);

        var warnings = Assert.IsType<Completed<FinishTradeResult>>(response.Value).Data.Warnings.ToList();
        Assert.Equal(2, warnings.Count);
        Assert.Contains("Your trade indicates a 'Loss' result, but you have entered 'Break-Even'.", warnings);
        Assert.Contains("Your position indicates the result 'Mediocre', but based on your Profit/Loss it is 'Loss'.",
            warnings);
    }

    [Fact]
    public void
        Long_position_indicates_a_mediocre_result_but_the_profitLoss_is_zero__the_trade_gets_finished_as_BreakEven_but_a_warning_is_returned()
    {
        // arrange        
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.0m, StopLoss = 0.9m, TakeProfit = 1.3m
            }
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 0, UtcNowStub)
        {
            ExitPrice = 1.25m
        };

        var response = trade.Finish(finishTradeDto);

        var result = Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        var singleWarning = Assert.Single(result.Data.Warnings);
        Assert.Equal("Your position indicates the result 'Mediocre', but based on your Profit/Loss it is 'Break-Even'.",
            singleWarning);
    }

    [Fact]
    public void
        Long_position_indicates_a_loss_result_but_the_profitLoss_is_zero__the_trade_gets_finished_as_BreakEven_and_a_warning_is_returned()
    {
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.0m, StopLoss = 0.9m, TakeProfit = 1.3m
            }
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 0, UtcNowStub)
        {
            ExitPrice = 0.9m
        };

        var response = trade.Finish(finishTradeDto);

        var result = Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        var singleWarning = Assert.Single(result.Data.Warnings);
        Assert.Equal("Your position indicates the result 'Loss', but based on your Profit/Loss it is 'Break-Even'.",
            singleWarning);
    }

    [Fact]
    public void
        Short_position_indicates_a_win_result_but_the_profitLoss_is_zero__the_trade_gets_finished_as_BreakEven_and_a_warning_is_returned()
    {
        // arrange        
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.0m, StopLoss = 1.1m, TakeProfit = 0.7m
            }
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 0, UtcNowStub)
        {
            ExitPrice = 0.7m
        };

        var response = trade.Finish(finishTradeDto);

        var result = Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        var singleWarning = Assert.Single(result.Data.Warnings);
        Assert.Equal("Your position indicates the result 'Win', but based on your Profit/Loss it is 'Break-Even'.",
            singleWarning);
    }

    [Fact]
    public void
        Short_position_indicates_a_loss_result_but_the_profitLoss_is_zero__the_trade_gets_finished_as_BreakEven_and_a_warning_is_returned()
    {
        // arrange        
        var trade = (TestData.Trade.Default with
        {
            PositionPrices = TestData.PositionPrices.Default with
            {
                EntryPrice = 1.0m, StopLoss = 1.1m, TakeProfit = 0.7m
            }
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 0, UtcNowStub)
        {
            ExitPrice = 1.1m
        };

        var response = trade.Finish(finishTradeDto);

        var result = Assert.IsType<Completed<FinishTradeResult>>(response.Value);
        var singleWarning = Assert.Single(result.Data.Warnings);
        Assert.Equal("Your position indicates the result 'Loss', but based on your Profit/Loss it is 'Break-Even'.",
            singleWarning);
    }

    [Fact]
    public void The_result_is_null_if_it_was_overriden_with_a_null_value()
    {
        // arrange        
        var trade = (TestData.Trade.Default with
        {
            Opened = _utcNow,
            Finished = _utcNow,
            ProfitLoss = 0m
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 0, UtcNowStub)
        {
            ManuallyEnteredResult = null
        };

        _ = trade.Finish(finishTradeDto);

        Assert.Null(trade.Result);
    }

    [Fact]
    public void The_result_is_not_overriden_if_manually_entered_result_is_none()
    {
        // arrange        
        var trade = (TestData.Trade.Default with
        {
            Opened = _utcNow,
            Finished = _utcNow,
            ProfitLoss = 0m
        }).Build();

        var finishTradeDto = new FinishTradeConfiguration(_utcNow, 0, UtcNowStub)
        {
            ManuallyEnteredResult = new None()
        };

        _ = trade.Finish(finishTradeDto);

        Assert.Equal(Result.BreakEven, trade.Result?.Name);
        ;
    }

    private DateTime UtcNowStub()
    {
        return _utcNow;
    }
}