using SimpleTrading.Domain.Trading.UseCases.AddTrade;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public record TradeAddedDto(Guid TradeId, TradeResultDto TradeResult)
{
    public static TradeAddedDto From(AddTradeResponseModel model)
    {
        return new TradeAddedDto(model.TradeId, TradeResultDto.From(model));
    }
}