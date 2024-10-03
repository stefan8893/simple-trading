using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyzer.Decorators;

internal class ShortPositionTradeResultAnalyzerDecorator(ITradeResultAnalyzer innerComponent) : ITradeResultAnalyzer
{
    public IEnumerable<string> AnalyzeResults(Trade trade, TradeResultAnalyzerConfiguration config)
    {
        var additionalWarnings = AnalyzeShortPositionPrices(trade, config);

        return innerComponent.AnalyzeResults(trade, config)
            .Concat(additionalWarnings);
    }

    private static IEnumerable<string> AnalyzeShortPositionPrices(Trade trade, TradeResultAnalyzerConfiguration config)
    {
        var isShortPosition = trade.PositionPrices.IsShortPosition;
        var hasBalanceResult = config.CalculatedByBalance is not null;
        if (!isShortPosition || hasBalanceResult)
            yield break;

        var prices = trade.PositionPrices;
        var balance = trade.Balance!.Value;

        if (balance > 0 && prices.Exit.HasValue && prices.Exit > prices.Entry)
            yield return SimpleTradingStrings.ShortPositionExitGreaterThanEntryAndPositiveBalance;
    }
}