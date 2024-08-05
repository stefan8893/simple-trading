using FluentAssertions;
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

        prices.RiskRewardRatio.Should().BeNull();
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
        prices.RiskRewardRatio.Should().Be(expectedRrr);
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
        prices.RiskRewardRatio.Should().Be(expectedRrr);
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
        prices.RiskRewardRatio.Should().Be(expectedRrr);
    }

    [Fact]
    public void RRR_is_null_if_StopLoss_is_missing()
    {
        var prices = new PositionPrices
        {
            Entry = 1.08600m,
            TakeProfit = 1.08300m
        };

        prices.RiskRewardRatio.Should().BeNull();
    }

    [Fact]
    public void RRR_is_null_if_TakeProfit_is_missing()
    {
        var prices = new PositionPrices
        {
            Entry = 1.08600m,
            StopLoss = 1.08300m
        };

        prices.RiskRewardRatio.Should().BeNull();
    }
}