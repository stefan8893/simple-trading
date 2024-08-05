namespace SimpleTrading.Domain.Trading;

public record PositionPrices
{
    public required decimal Entry { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }

    public double? RiskRewardRatio =>
        !StopLoss.HasValue || !TakeProfit.HasValue
            ? null
            : (double) Math.Round(Math.Abs((Entry - TakeProfit.Value) / (Entry - StopLoss.Value)), 2);
}