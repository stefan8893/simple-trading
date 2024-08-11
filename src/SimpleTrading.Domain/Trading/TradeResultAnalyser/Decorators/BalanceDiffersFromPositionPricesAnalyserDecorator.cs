using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyser.Decorators;

public class BalanceDiffersFromPositionPricesAnalyserDecorator(ITradeResultAnalyser innerComponent)
    : ITradeResultAnalyser
{
    public IEnumerable<Warning> AnalyseResults(Trade trade, TradeResultAnalyserConfiguration config)
    {
        var additionalWarnings = AnalyseBalanceAndPositionPrices(trade, config);

        return innerComponent.AnalyseResults(trade, config)
            .Concat(additionalWarnings);
    }

    private static IEnumerable<Warning> AnalyseBalanceAndPositionPrices(Trade trade,
        TradeResultAnalyserConfiguration config)
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