﻿using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyzer;

internal class EnteredResultDiffersFromCalculatedAnalyzer : ITradeResultAnalyzer
{
    public IEnumerable<Warning> AnalyzeResults(Trade trade, TradeResultAnalyzerConfiguration config)
    {
        var hasManuallyEnteredResult = config.ManuallyEntered is not null;
        var hasCalculatedResult = config.CalculatedResult is not null;

        if (hasCalculatedResult && hasManuallyEnteredResult &&
            config.CalculatedResult!.Name != config.ManuallyEntered!.Name)
            yield return CreateManuallyEnteredAndCalculatedResultMismatchWarning(config);

        var hasCalculatedByBalanceResult = config.CalculatedByBalance is not null;
        var hasCalculatedByPositionPricesResult = config.CalculatedByPositionPrices is not null;

        if (hasCalculatedByBalanceResult && hasCalculatedByPositionPricesResult &&
            config.CalculatedByBalance!.Name != config.CalculatedByPositionPrices!.Name)
            yield return CreateMismatchBetweenCalculatedResultsWarning(config);
    }

    private static Warning CreateMismatchBetweenCalculatedResultsWarning(TradeResultAnalyzerConfiguration config)
    {
        var balanceResultName = SimpleTradingStrings.ResourceManager.GetString(config.CalculatedByBalance!.Name);
        var positionResultName =
            SimpleTradingStrings.ResourceManager.GetString(config.CalculatedByPositionPrices!.Name);

        return new Warning(string.Format(SimpleTradingStrings.CalculatedResultsMismatch,
            positionResultName, balanceResultName));
    }

    private static Warning CreateManuallyEnteredAndCalculatedResultMismatchWarning(
        TradeResultAnalyzerConfiguration config)
    {
        var calculatedResultName = SimpleTradingStrings.ResourceManager.GetString(config.CalculatedResult!.Name);
        var manuallyEnteredResultName = SimpleTradingStrings.ResourceManager.GetString(config.ManuallyEntered!.Name);

        var message = string.Format(SimpleTradingStrings.CalculatedAndManualResultMismatch,
            calculatedResultName, manuallyEnteredResultName);

        return new Warning(message);
    }
}