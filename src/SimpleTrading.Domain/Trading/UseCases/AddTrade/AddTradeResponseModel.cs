using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

public record AddTradeResponseModel(Guid TradeId, bool DryRun, ResultModel? Result, short? Performance, IEnumerable<string> Warnings)
{
    public static AddTradeResponseModel From(Trade trade, IEnumerable<string> warnings, bool dryRun)
    {
        return new AddTradeResponseModel(trade.Id,
            dryRun,
            trade.Result?.ToResultModel(),
            trade.Result?.Performance,
            warnings);
    }
}