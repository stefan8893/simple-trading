namespace SimpleTrading.Domain.Trading.TradeResultAnalyser;

internal record TradeResultAnalyserConfiguration
{
    public required Result? ManuallyEntered { get; init; }
    public required Result? CalculatedByBalance { get; init; }
    public required Result? CalculatedByPositionPrices { get; init; }

    /// <summary>
    ///     Either the result calculated by the balance or the position prices. <br />
    ///     This result will be taken if the user does not override it.
    /// </summary>
    public required Result? CalculatedResult { get; init; }
}