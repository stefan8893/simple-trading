using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyzer.Decorators;

internal class ProfitLossDiffersFromPositionPricesAnalyzerDecorator(ITradeResultAnalyzer innerComponent)
    : ITradeResultAnalyzer
{
    public IEnumerable<string> AnalyzeResults(Trade trade, TradeResultAnalyzerConfiguration config)
    {
        var additionalWarnings = AnalyzeProfitLossAndPositionPrices(trade);

        return innerComponent.AnalyzeResults(trade, config)
            .Concat(additionalWarnings);
    }

    private static IEnumerable<string> AnalyzeProfitLossAndPositionPrices(Trade trade)
    {
        var prices = trade.PositionPrices;
        var isLongOrShortPosition = prices.IsLongPosition || prices.IsShortPosition;
        var profitLoss = trade.ProfitLoss!.Value;

        if (isLongOrShortPosition)
            yield break;

        if (profitLoss == 0m && prices.Exit.HasValue && prices.Exit != prices.Entry)
            yield return SimpleTradingStrings.ProfitLossZeroAndExitEntryPricesNotTheSame;

        if (profitLoss != 0m && prices.Exit.HasValue && prices.Exit == prices.Entry)
            yield return SimpleTradingStrings.ProfitLossNotZeroAndExitEntryPricesSame;
    }
}