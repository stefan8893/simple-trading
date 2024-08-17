using System.Collections.Immutable;

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

public record Result : ITradingResult
{
    public const string Loss = nameof(Loss);
    public const string BreakEven = nameof(BreakEven);
    public const string Mediocre = nameof(Mediocre);
    public const string Win = nameof(Win);

    public static readonly IImmutableList<string> SupportedResults;
    public static readonly int IndexOfLoss;
    public static readonly int IndexOfBreakEven;
    public static readonly int IndexOfMediocre;
    public static readonly int IndexOfWin;

    static Result()
    {
        SupportedResults = new List<string>([Loss, BreakEven, Mediocre, Win])
            .ToImmutableList();

        IndexOfLoss = GetIndexOf(Loss);
        IndexOfBreakEven = GetIndexOf(BreakEven);
        IndexOfMediocre = GetIndexOf(Mediocre);
        IndexOfWin = GetIndexOf(Win);
    }

    public Result(string name, TradingResultSource source, short? performance = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var result = SupportedResults
            .FirstOrDefault(x => x.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase));

        Name = result ?? throw new ArgumentException(
            $"Invalid result name. It must be one of '{string.Join(", ", SupportedResults)}'");
        Source = source;
        Performance = performance;
        Index = GetIndexOf(Name);
    }

    public int Index { get; init; }

    public string Name { get; init; }

    public TradingResultSource Source { get; init; }

    public short? Performance { get; init; }

    public static int GetIndexOf(string result)
    {
        if (string.IsNullOrWhiteSpace(result))
            return -1;

        return SupportedResults.IndexOf(result.Trim(),
            0,
            SupportedResults.Count,
            StringComparer.OrdinalIgnoreCase);
    }
}