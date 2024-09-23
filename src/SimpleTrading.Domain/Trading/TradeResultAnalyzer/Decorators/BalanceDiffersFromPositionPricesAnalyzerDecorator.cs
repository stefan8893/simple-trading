using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyzer.Decorators;

internal class BalanceDiffersFromPositionPricesAnalyzerDecorator(ITradeResultAnalyzer innerComponent)
    : ITradeResultAnalyzer
{
    public IEnumerable<Warning> AnalyzeResults(Trade trade, TradeResultAnalyzerConfiguration config)
    {
        var additionalWarnings = AnalyzeBalanceAndPositionPrices(trade, config);

        return innerComponent.AnalyzeResults(trade, config)
            .Concat(additionalWarnings);
    }

    private static IEnumerable<Warning> AnalyzeBalanceAndPositionPrices(Trade trade,
        TradeResultAnalyzerConfiguration config)
    {
        var prices = trade.PositionPrices;
        var isLongOrShortPosition = prices.IsLongPosition || prices.IsShortPosition;
        var balance = trade.Balance!.Value;

        if (isLongOrShortPosition)
            yield break;

        if (balance == 0m && prices.Exit.HasValue && prices.Exit != prices.Entry)
            yield return new Warning(SimpleTradingStrings.BalanceZeroAndExitEntryPricesNotTheSame);

        if (balance != 0m && prices.Exit.HasValue && prices.Exit == prices.Entry)
            yield return new Warning(SimpleTradingStrings.BalanceNotZeroAndExitEntryPricesSame);
    }
}