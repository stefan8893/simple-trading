using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyser.Decorators;

public class LongPositionAnalyserDecorator(ITradeResultAnalyser innerComponent) : ITradeResultAnalyser
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
        if (!isLongPosition)
            yield break;

        var prices = trade.PositionPrices;
        var balance = trade.Balance!.Value;

        switch (balance)
        {
            case 0m when prices.Exit.HasValue && prices.Exit > prices.Entry:
                yield return new Warning(SimpleTradingStrings.LongPositionExitGreaterThanEntryAndZeroBalance);
                break;
            case 0m when prices.Exit.HasValue && prices.Exit < prices.Entry:
                yield return new Warning(SimpleTradingStrings.LongPositionExitLessThanEntryAndZeroBalance);
                break;
            case < 0 when prices.Exit.HasValue && prices.Exit > prices.Entry:
                yield return new Warning(
                    SimpleTradingStrings.LongPositionExitGreaterThanEntryAndNegativeBalance);
                break;
            case > 0 when prices.Exit.HasValue && prices.Exit < prices.Entry:
                yield return new Warning(SimpleTradingStrings.LongPositionExitLessThanEntryAndPositiveBalance);
                break;
        }
    }
}