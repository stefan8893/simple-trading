using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyser;

internal interface ITradeResultAnalyser
{
    IEnumerable<Warning> AnalyseResults(Trade trade, TradeResultAnalyserConfiguration config);
}