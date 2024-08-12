namespace SimpleTrading.Domain.Trading;

public enum TradingResultSource
{
    ManuallyEntered,
    CalculatedByBalance,
    CalculatedByPositionPrices
}

public interface ITradingResult
{
    string Name { get; }
    TradingResultSource Source { get; }

    /// <summary>
    ///     The trade's performance in percent.<br />
    ///     ExitPrice == StopLoss => -100%<br />
    ///     ExitPrice == TakeProfit => 100%<br />
    ///     ExitPrice == EntryPrice => 0%<br />
    /// </summary>
    short? Performance { get; }
}

public static class TradingResult
{
    public record Loss(TradingResultSource Source, short? Performance = null) : ITradingResult
    {
        public string Name { get; } = nameof(Loss);
        public short? Performance { get; } = Performance;
    }

    public record BreakEven(TradingResultSource Source, short? Performance = null) : ITradingResult
    {
        public string Name { get; } = nameof(BreakEven);
        public short? Performance { get; } = Performance;
    }

    public record Mediocre(TradingResultSource Source, short? Performance = null) : ITradingResult
    {
        public string Name { get; } = nameof(Mediocre);
        public short? Performance { get; } = Performance;
    }

    public record Win(TradingResultSource Source, short? Performance = null) : ITradingResult
    {
        public string Name { get; } = nameof(Win);
        public short? Performance { get; } = Performance;
    }
}