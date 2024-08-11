using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyser.Decorators;

public class ShortPositionTradeResultAnalyserDecorator(ITradeResultAnalyser innerComponent) : ITradeResultAnalyser
{
    public IEnumerable<Warning> AnalyseResults(Trade trade, TradeResultAnalyserConfiguration config)
    {
        var additionalWarnings = AnalyseShortPositionPrices(trade);

        return innerComponent.AnalyseResults(trade, config)
            .Concat(additionalWarnings);
    }

    private static IEnumerable<Warning> AnalyseShortPositionPrices(Trade trade)
    {
        var isShortPosition = trade.PositionPrices.IsShortPosition;
        if (!isShortPosition)
            yield break;

        var prices = trade.PositionPrices;
        var balance = trade.Balance!.Value;

        switch (balance)
        {
            case 0m when prices.Exit.HasValue && prices.Exit < prices.Entry:
                yield return new Warning(SimpleTradingStrings.ShortPositionExitLessThanEntryAndZeroBalance);
                break;
            case 0m when prices.Exit.HasValue && prices.Exit > prices.Entry:
                yield return new Warning(SimpleTradingStrings.ShortPositionExitGreaterThanEntryAndZeroBalance);
                break;
            case > 0 when prices.Exit.HasValue && prices.Exit > prices.Entry:
                yield return new Warning(SimpleTradingStrings.ShortPositionExitGreaterThanEntryAndPositiveBalance);
                break;
            case < 0 when prices.Exit.HasValue && prices.Exit < prices.Entry:
                yield return new Warning(SimpleTradingStrings.ShortPositionExitLessThanEntryAndNegativeBalance);
                break;
        }
    }
}