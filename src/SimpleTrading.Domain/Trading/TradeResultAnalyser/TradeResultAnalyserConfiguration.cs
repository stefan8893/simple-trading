namespace SimpleTrading.Domain.Trading.TradeResultAnalyser;

internal record TradeResultAnalyserConfiguration
{
    public required ITradingResult? ManuallyEntered { get; init; }
    public required ITradingResult? CalculatedByBalance { get; init; }
    public required ITradingResult? CalculatedByPositionPrices { get; init; }

    /// <summary>
    ///     Either the result calculated by the balance or the position prices. <br />
    ///     This result will be taken if the user does not override it.
    /// </summary>
    public required ITradingResult? CalculatedResult { get; init; }
}