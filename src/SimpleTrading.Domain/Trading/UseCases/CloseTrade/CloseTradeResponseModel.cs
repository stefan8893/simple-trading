namespace SimpleTrading.Domain.Trading.UseCases.CloseTrade;

public record CloseTradeResponseModel(Guid TradeId, ResultModel? Result, short? Performance);