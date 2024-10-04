using SimpleTrading.Domain.Trading.UseCases.CloseTrade;
using SimpleTrading.Domain.Trading.UseCases.RestoreCalculatedResult;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public record TradeResultDto(Guid TradeId, ResultDto? Result, short? Performance, IEnumerable<string> Warnings)
{
    public static TradeResultDto From(CloseTradeResponseModel model)
    {
        return new TradeResultDto(model.TradeId, model.Result.ToResultDto(), model.Performance, model.Warnings);
    }

    public static TradeResultDto From(RestoreCalculatedResultResponseModel model)
    {
        return new TradeResultDto(model.TradeId, model.Result.ToResultDto(), model.Performance, model.Warnings);
    }
}