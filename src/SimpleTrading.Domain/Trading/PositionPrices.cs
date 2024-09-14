namespace SimpleTrading.Domain.Trading;

public enum PositionType
{
    Long,
    Short
}

public record PositionPrices
{
    private static readonly Result BreakEvenResult =
        new(Result.BreakEven, ResultSource.CalculatedByPositionPrices, 0);

    public required decimal Entry { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
    public decimal? Exit { get; set; }

    public double? RiskRewardRatio =>
        !StopLoss.HasValue || !TakeProfit.HasValue
            ? null
            : (double) Math.Round(Math.Abs((Entry - TakeProfit.Value) / (Entry - StopLoss.Value)), 2);

    public PositionType? Type
        => IsLongPosition
            ? PositionType.Long
            : IsShortPosition
                ? PositionType.Short
                : null;

    public bool IsLongPosition => TakeProfit > Entry;
    public bool IsShortPosition => TakeProfit < Entry;

    public Result? CalculateResult()
    {
        return Type switch
        {
            PositionType.Long => CalculateLongPositionResult(),
            PositionType.Short => CalculateShortPositionResult(),
            _ => null
        };
    }

    private Result? CalculateLongPositionResult()
    {
        if (Exit == Entry)
            return BreakEvenResult;

        if (Exit > Entry)
            return Exit < TakeProfit
                ? CalculateMediocreResult()
                : CalculateWinResult();

        if (StopLoss.HasValue && Exit < Entry)
            return CalculateLossResult();

        return null;
    }

    private Result? CalculateShortPositionResult()
    {
        if (Exit == Entry)
            return BreakEvenResult;

        if (Exit < Entry)
            return Exit > TakeProfit
                ? CalculateMediocreResult()
                : CalculateWinResult();

        if (Exit > Entry && StopLoss.HasValue)
            return CalculateLossResult();

        return null;
    }

    private Result CalculateLossResult()
    {
        var diffBetweenEntryAndStopLoss = Entry - StopLoss!.Value;
        var diffBetweenEntryAndExit = Entry - Exit!.Value;
        var performance = Math.Abs(diffBetweenEntryAndExit / diffBetweenEntryAndStopLoss * 100) * -1m;

        return new Result(Result.Loss, ResultSource.CalculatedByPositionPrices, (short) performance);
    }

    private Result CalculateWinResult()
    {
        var performance = CalculatePositivePerformance();

        return new Result(Result.Win, ResultSource.CalculatedByPositionPrices, (short) performance);
    }

    private Result CalculateMediocreResult()
    {
        var performance = CalculatePositivePerformance();

        return new Result(Result.Mediocre, ResultSource.CalculatedByPositionPrices, (short) performance);
    }

    private decimal CalculatePositivePerformance()
    {
        var diffBetweenEntryAndTakeProfit = Entry - TakeProfit!.Value;
        var diffBetweenEntryAndExit = Entry - Exit!.Value;
        var performance = Math.Abs(diffBetweenEntryAndExit / diffBetweenEntryAndTakeProfit * 100);

        return performance;
    }
}