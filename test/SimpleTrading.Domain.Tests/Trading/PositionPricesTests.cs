using SimpleTrading.Domain.Trading;

namespace SimpleTrading.Domain.Tests.Trading;

public class PositionPricesTests
{
    [Fact]
    public void There_is_no_RRR_if_StopLoss_and_TakeProfit_are_not_set()
    {
        var prices = new PositionPrices
        {
            Entry = 1.08m
        };

        Assert.Null(prices.RiskRewardRatio);
    }

    [Fact]
    public void RRR_is_positive_if_going_long()
    {
        var prices = new PositionPrices
        {
            Entry = 1.08500m,
            StopLoss = 1.08400m,
            TakeProfit = 1.08800m
        };

        const double expectedRrr = 3d;
        Assert.Equal(expectedRrr, prices.RiskRewardRatio);
    }

    [Fact]
    public void RRR_is_positive_if_going_short()
    {
        var prices = new PositionPrices
        {
            Entry = 1.08600m,
            StopLoss = 1.08700m,
            TakeProfit = 1.08300m
        };

        const double expectedRrr = 3d;
        Assert.Equal(expectedRrr, prices.RiskRewardRatio);
    }

    [Fact]
    public void RRR_contains_two_decimal_places()
    {
        var prices = new PositionPrices
        {
            Entry = 1.08600m,
            StopLoss = 1.08706m,
            TakeProfit = 1.08300m
        };

        const double expectedRrr = 2.83;
        Assert.Equal(expectedRrr, prices.RiskRewardRatio);
    }

    [Fact]
    public void RRR_is_null_if_StopLoss_is_missing()
    {
        var prices = new PositionPrices
        {
            Entry = 1.08600m,
            TakeProfit = 1.08300m
        };

        Assert.Null(prices.RiskRewardRatio);
    }

    [Fact]
    public void RRR_is_null_if_TakeProfit_is_missing()
    {
        var prices = new PositionPrices
        {
            Entry = 1.08600m,
            StopLoss = 1.08300m
        };

        Assert.Null(prices.RiskRewardRatio);
    }

    [Fact]
    public void It_is_a_long_position_when_TP_is_above_entry()
    {
        var prices = new PositionPrices
        {
            StopLoss = 1.0m,
            Entry = 1.1m,
            TakeProfit = 1.4m
        };

        var positionType = prices.Type;

        Assert.NotNull(positionType);
        Assert.Equal(PositionType.Long, positionType);
    }

    [Fact]
    public void It_is_a_long_position_when_TP_is_above_entry_independent_of_the_SL_value()
    {
        var prices = new PositionPrices
        {
            StopLoss = 1.3m,
            Entry = 1.1m,
            TakeProfit = 1.4m
        };

        var positionType = prices.Type;

        Assert.NotNull(positionType);
        Assert.Equal(PositionType.Long, positionType);
    }

    [Fact]
    public void It_is_a_short_position_when_TP_is_below_entry()
    {
        var prices = new PositionPrices
        {
            StopLoss = 1.5m,
            Entry = 1.4m,
            TakeProfit = 1.1m
        };

        var positionType = prices.Type;

        Assert.NotNull(positionType);
        Assert.Equal(PositionType.Short, positionType);
    }

    [Fact]
    public void It_is_a_short_position_when_TP_is_below_entry_independent_of_the_SL()
    {
        var prices = new PositionPrices
        {
            StopLoss = 1.2m,
            Entry = 1.4m,
            TakeProfit = 1.1m
        };

        var positionType = prices.Type;

        Assert.NotNull(positionType);
        Assert.Equal(PositionType.Short, positionType);
    }

    [Fact]
    public void Short_position_win_result_gets_properly_calculated()
    {
        var prices = new PositionPrices
        {
            Entry = 1.0m,
            StopLoss = 1.1m,
            TakeProfit = 0.7m,
            Exit = 0.7m
        };

        var result = prices.CalculateResult();

        Assert.NotNull(result);
        Assert.Equal(Result.Win, result.Name);
        Assert.Equal((short)100, result.Performance);
    }

    [Fact]
    public void Short_position_without_SL_and_exit_above_entry_leads_to_no_result()
    {
        var prices = new PositionPrices
        {
            Entry = 1.0m,
            StopLoss = null,
            TakeProfit = 0.7m,
            Exit = 1.1m
        };

        var result = prices.CalculateResult();

        Assert.Null(result);
    }
}