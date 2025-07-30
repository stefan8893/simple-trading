using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyzer;

internal class EnteredResultDiffersFromCalculatedAnalyzer : ITradeResultAnalyzer
{
    public IEnumerable<string> AnalyzeResults(Trade trade, TradeResultAnalyzerConfiguration config)
    {
        var hasManuallyEnteredResult = config.ManuallyEntered is not null;
        var hasCalculatedResult = config.CalculatedResult is not null;

        if (hasCalculatedResult && hasManuallyEnteredResult &&
            config.CalculatedResult!.Name != config.ManuallyEntered!.Name)
            yield return CreateManuallyEnteredAndCalculatedResultMismatchWarning(config);

        var hasCalculatedByProfitLossResult = config.CalculatedByProfitLoss is not null;
        var hasCalculatedByPositionPricesResult = config.CalculatedByPositionPrices is not null;

        if (hasCalculatedByProfitLossResult && hasCalculatedByPositionPricesResult &&
            config.CalculatedByProfitLoss!.Name != config.CalculatedByPositionPrices!.Name)
            yield return CreateMismatchBetweenCalculatedResultsWarning(config);
    }

    private static string CreateMismatchBetweenCalculatedResultsWarning(TradeResultAnalyzerConfiguration config)
    {
        var profitLossResultName = SimpleTradingStrings.ResourceManager.GetString(config.CalculatedByProfitLoss!.Name);
        var positionResultName =
            SimpleTradingStrings.ResourceManager.GetString(config.CalculatedByPositionPrices!.Name);

        return string.Format(SimpleTradingStrings.CalculatedResultsMismatch,
            positionResultName, profitLossResultName);
    }

    private static string CreateManuallyEnteredAndCalculatedResultMismatchWarning(
        TradeResultAnalyzerConfiguration config)
    {
        var calculatedResultName = SimpleTradingStrings.ResourceManager.GetString(config.CalculatedResult!.Name);
        var manuallyEnteredResultName = SimpleTradingStrings.ResourceManager.GetString(config.ManuallyEntered!.Name);

        return string.Format(SimpleTradingStrings.CalculatedAndManualResultMismatch,
            calculatedResultName, manuallyEnteredResultName);
    }
}