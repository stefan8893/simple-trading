using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyser.Decorators;

internal class LongPositionAnalyserDecorator(ITradeResultAnalyser innerComponent) : ITradeResultAnalyser
{
    public IEnumerable<Warning> AnalyseResults(Trade trade, TradeResultAnalyserConfiguration config)
    {
        var additionalWarnings = AnalyseLongPositionPrices(trade, config);

        return innerComponent.AnalyseResults(trade, config)
            .Concat(additionalWarnings);
    }

    private static IEnumerable<Warning> AnalyseLongPositionPrices(Trade trade, TradeResultAnalyserConfiguration config)
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