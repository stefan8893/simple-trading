using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyser;

public class EnteredResultDiffersFromCalculatedAnalyser : ITradeResultAnalyser
{
    public IEnumerable<Warning> AnalyseResults(Trade trade, TradeResultAnalyserConfiguration config)
    {
        var hasManuallyEnteredResult = config.ManuallyEntered is not null;
        var hasCalculatedResult = config.CalculatedResult is not null;

        if (hasCalculatedResult && hasManuallyEnteredResult &&
            config.CalculatedResult!.Name != config.ManuallyEntered!.Name)
            yield return CreateWarning(config);
    }

    private static Warning CreateWarning(TradeResultAnalyserConfiguration config)
    {
        var calculatedResultName = SimpleTradingStrings.ResourceManager.GetString(config.CalculatedResult!.Name);
        var manuallyEnteredResultName = SimpleTradingStrings.ResourceManager.GetString(config.ManuallyEntered!.Name);

        var message = string.Format(SimpleTradingStrings.CalculatedAndManualResultMismatch,
            calculatedResultName, manuallyEnteredResultName);

        return new Warning(message);
    }
}