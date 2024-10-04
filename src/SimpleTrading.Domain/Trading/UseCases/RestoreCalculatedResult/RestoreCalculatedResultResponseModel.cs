using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.RestoreCalculatedResult;

public record RestoreCalculatedResultResponseModel(
    Guid TradeId,
    ResultModel? Result,
    short? Performance,
    IEnumerable<string> Warnings);