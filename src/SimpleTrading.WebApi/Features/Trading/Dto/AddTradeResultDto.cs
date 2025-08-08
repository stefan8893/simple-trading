using SimpleTrading.Domain.Trading.UseCases.AddTrade;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public class AddTradeResultDto
{
    public required Guid TradeId { get; init; }
    public required IEnumerable<string> Warnings { get; init; }
    public required bool DryRun { get; init; }
    public required ResultDto? Result { get; init; }
    public required short? Performance { get; init; }

    public static AddTradeResultDto From(AddTradeResponseModel model)
    {
        return new AddTradeResultDto
        {
            TradeId = model.TradeId,
            Warnings = model.Warnings,
            DryRun = model.DryRun,
            Result = model.Result.ToResultDto(),
            Performance = model.Performance
        };
    }
}