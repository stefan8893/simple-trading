using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.CloseTrade;

public record CloseTradeResponseModel(
    Guid TradeId,
    ResultModel? Result,
    short? Performance,
    IEnumerable<string> Warnings);