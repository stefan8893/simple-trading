using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

public record AddTradeResponseModel(Guid TradeId, ResultModel? Result, short? Performance, IEnumerable<string> Warnings)
{
    public static AddTradeResponseModel From(Trade trade, IEnumerable<string> warnings)
    {
        return new AddTradeResponseModel(trade.Id,
            trade.Result?.ToResultModel(),
            trade.Result?.Performance,
            warnings);
    }
}