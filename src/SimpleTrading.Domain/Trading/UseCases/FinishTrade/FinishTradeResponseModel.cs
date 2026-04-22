using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.FinishTrade;

public record FinishTradeResponseModel(
    Guid TradeId,
    ResultModel? Result,
    short? Performance,
    IEnumerable<string> Warnings);