namespace SimpleTrading.Domain.Trading.TradeResultAnalyzer;

internal interface ITradeResultAnalyzer
{
    IEnumerable<string> AnalyzeResults(Trade trade, TradeResultAnalyzerConfiguration config);
}