﻿using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyzer.Decorators;

internal class LongPositionAnalyzerDecorator(ITradeResultAnalyzer innerComponent) : ITradeResultAnalyzer
{
    public IEnumerable<Warning> AnalyzeResults(Trade trade, TradeResultAnalyzerConfiguration config)
    {
        var additionalWarnings = AnalyzeLongPositionPrices(trade, config);

        return innerComponent.AnalyzeResults(trade, config)
            .Concat(additionalWarnings);
    }

    private static IEnumerable<Warning> AnalyzeLongPositionPrices(Trade trade, TradeResultAnalyzerConfiguration config)
    {
        var isLongPosition = trade.PositionPrices.IsLongPosition;
        var hasBalanceResult = config.CalculatedByBalance is not null;
        if (!isLongPosition || hasBalanceResult)
            yield break;

        var prices = trade.PositionPrices;
        var balance = trade.Balance!.Value;

        if (balance > 0 && prices.Exit.HasValue && prices.Exit < prices.Entry)
            yield return new Warning(SimpleTradingStrings.LongPositionExitLessThanEntryAndPositiveBalance);
    }
}