using System.Collections.Immutable;

namespace SimpleTrading.Domain.Trading;

public enum ResultSource
{
    ManuallyEntered,
    CalculatedByBalance,
    CalculatedByPositionPrices
}

public record Result
{
    public const string Loss = nameof(Loss);
    public const string BreakEven = nameof(BreakEven);
    public const string Mediocre = nameof(Mediocre);
    public const string Win = nameof(Win);

    private static readonly ImmutableList<string> SupportedResults;

    static Result()
    {
        SupportedResults = new List<string>([Loss, BreakEven, Mediocre, Win])
            .ToImmutableList();
    }

    public Result(string name, ResultSource source, short? performance = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var result = SupportedResults
            .FirstOrDefault(x => x.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase));

        Name = result ?? throw new ArgumentException(
            $"Invalid result name. It must be one of '{string.Join(", ", SupportedResults)}'");
        Source = source;
        Performance = performance;
        Index = IndexOf(Name);
    }

    public int Index { get; init; }

    public string Name { get; init; }

    public ResultSource Source { get; init; }

    /// <summary>
    ///     The trade's performance in percent.<br />
    ///     ExitPrice == StopLoss => -100%<br />
    ///     ExitPrice == TakeProfit => 100%<br />
    ///     ExitPrice == EntryPrice => 0%<br />
    /// </summary>
    public short? Performance { get; init; }

    public static int IndexOf(string result)
    {
        if (string.IsNullOrWhiteSpace(result))
            return -1;

        return SupportedResults.IndexOf(result.Trim(),
            0,
            SupportedResults.Count,
            StringComparer.OrdinalIgnoreCase);
    }

    public static string GetName(int index)
    {
        if (index < 0 || index >= SupportedResults.Count)
            throw new ArgumentOutOfRangeException(nameof(index), index, null);

        return SupportedResults[index];
    }
}