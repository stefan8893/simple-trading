using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyser.Decorators;

internal class ShortPositionTradeResultAnalyserDecorator(ITradeResultAnalyser innerComponent) : ITradeResultAnalyser
{
    public IEnumerable<Warning> AnalyseResults(Trade trade, TradeResultAnalyserConfiguration config)
    {
        var additionalWarnings = AnalyseShortPositionPrices(trade, config);

        return innerComponent.AnalyseResults(trade, config)
            .Concat(additionalWarnings);
    }

    private static IEnumerable<Warning> AnalyseShortPositionPrices(Trade trade, TradeResultAnalyserConfiguration config)
    {
        var isShortPosition = trade.PositionPrices.IsShortPosition;
        var hasBalanceResult = config.CalculatedByBalance is not null;
        if (!isShortPosition || hasBalanceResult)
            yield break;

        var prices = trade.PositionPrices;
        var balance = trade.Balance!.Value;

        if (balance > 0 && prices.Exit.HasValue && prices.Exit > prices.Entry)
            yield return new Warning(SimpleTradingStrings.ShortPositionExitGreaterThanEntryAndPositiveBalance);
    }
}