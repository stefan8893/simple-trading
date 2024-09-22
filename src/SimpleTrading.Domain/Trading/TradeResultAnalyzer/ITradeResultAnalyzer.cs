using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.TradeResultAnalyzer;

internal interface ITradeResultAnalyzer
{
    IEnumerable<Warning> AnalyzeResults(Trade trade, TradeResultAnalyzerConfiguration config);
}