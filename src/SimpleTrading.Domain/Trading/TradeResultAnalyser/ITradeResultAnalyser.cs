using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyser;

public interface ITradeResultAnalyser
{
    IEnumerable<Warning> AnalyseResults(Trade trade, TradeResultAnalyserConfiguration config);
}