namespace SimpleTrading.Domain.Trading;

public enum PositionType
{
    Long,
    Short
}

public record PositionPrices
{
    private static readonly TradingResult.BreakEven BreakEvenResult = new (TradingResultSource.CalculatedByPositionPrices, 0);
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

    public ITradingResult? CalculateResult()
    {
        return Type switch
        {
            PositionType.Long => CalculateLongPositionResult(),
            PositionType.Short => CalculateShortPositionResult(),
            _ => null
        };
    }

    private ITradingResult? CalculateLongPositionResult()
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

    private ITradingResult? CalculateShortPositionResult()
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

    private TradingResult.Loss CalculateLossResult()
    {
        var diffBetweenEntryAndStopLoss = Entry - StopLoss!.Value;
        var diffBetweenEntryAndExit = Entry - Exit!.Value;
        var performance = Math.Abs(diffBetweenEntryAndExit / diffBetweenEntryAndStopLoss * 100) * -1m;

        return new TradingResult.Loss(TradingResultSource.CalculatedByPositionPrices, (short) performance);
    }

    private TradingResult.Win CalculateWinResult()
    {
        var performance = CalculatePositivePerformance();

        return new TradingResult.Win(TradingResultSource.CalculatedByPositionPrices, (short) performance);
    }

    private TradingResult.Mediocre CalculateMediocreResult()
    {
        var performance = CalculatePositivePerformance();

        return new TradingResult.Mediocre(TradingResultSource.CalculatedByPositionPrices, (short) performance);
    }

    private decimal CalculatePositivePerformance()
    {
        var diffBetweenEntryAndTakeProfit = Entry - TakeProfit!.Value;
        var diffBetweenEntryAndExit = Entry - Exit!.Value;
        var performance = Math.Abs(diffBetweenEntryAndExit / diffBetweenEntryAndTakeProfit * 100);

        return performance;
    }
}