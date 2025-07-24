using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyzer.Decorators;

internal class BalanceDiffersFromPositionPricesAnalyzerDecorator(ITradeResultAnalyzer innerComponent)
    : ITradeResultAnalyzer
{
    public IEnumerable<string> AnalyzeResults(Trade trade, TradeResultAnalyzerConfiguration config)
    {
        var additionalWarnings = AnalyzeBalanceAndPositionPrices(trade);

        return innerComponent.AnalyzeResults(trade, config)
            .Concat(additionalWarnings);
    }

    private static IEnumerable<string> AnalyzeBalanceAndPositionPrices(Trade trade)
    {
        var prices = trade.PositionPrices;
        var isLongOrShortPosition = prices.IsLongPosition || prices.IsShortPosition;
        var balance = trade.Balance!.Value;

        if (isLongOrShortPosition)
            yield break;

        if (balance == 0m && prices.Exit.HasValue && prices.Exit != prices.Entry)
            yield return SimpleTradingStrings.BalanceZeroAndExitEntryPricesNotTheSame;

        if (balance != 0m && prices.Exit.HasValue && prices.Exit == prices.Entry)
            yield return SimpleTradingStrings.BalanceNotZeroAndExitEntryPricesSame;
    }
}