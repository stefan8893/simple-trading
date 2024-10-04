using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.UpdateTrade;

public record UpdateTradeResponseModel(
    Guid TradeId,
    ResultModel? Result,
    short? Performance,
    IEnumerable<string> Warnings)
{
    public static UpdateTradeResponseModel From(Trade trade, IEnumerable<string> warnings)
    {
        return new UpdateTradeResponseModel(trade.Id,
            trade.Result?.ToResultModel(),
            trade.Result?.Performance,
            warnings);
    }
}