namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

public record AddTradeResponseModel(Guid TradeId, ResultModel? Result, short? Performance);