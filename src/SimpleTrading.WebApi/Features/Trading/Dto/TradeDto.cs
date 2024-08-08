using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.GetTrade;
using SimpleTrading.WebApi.Features.Trading.Dto.Reference;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public class TradeDto
{
    public required Guid Id { get; init; }
    public required Guid AssetId { get; init; }
    public required string Asset { get; init; }
    public required Guid ProfileId { get; init; }
    public required string Profile { get; init; }
    public required decimal Size { get; init; }
    public required DateTime OpenedAt { get; init; }
    public required DateTime? FinishedAt { get; init; }
    public required decimal? Balance { get; init; }
    public required ResultDto? Result { get; init; }
    public required bool IsFinished { get; init; }
    public required Guid CurrencyId { get; init; }
    public required string Currency { get; init; }
    public required decimal Entry { get; init; }
    public required decimal? StopLoss { get; init; }
    public required decimal? TakeProfit { get; init; }
    public required double? RiskRewardRatio { get; init; }
    public required IReadOnlyList<ReferenceDto> References { get; init; }
    public required string? Notes { get; init; }

    public static TradeDto From(TradeResponseModel model)
    {
        return new TradeDto
        {
            Id = model.Id,
            AssetId = model.AssetId,
            Asset = model.Asset,
            ProfileId = model.ProfileId,
            Profile = model.Profile,
            Size = model.Size,
            OpenedAt = model.OpenedAt,
            FinishedAt = model.FinishedAt,
            Balance = model.Balance,
            Result = MapToResultDto(model.Result),
            IsFinished = model.IsFinished,
            CurrencyId = model.CurrencyId,
            Currency = model.Currency,
            Entry = model.Entry,
            StopLoss = model.StopLoss,
            TakeProfit = model.TakeProfit,
            RiskRewardRatio = model.RiskRewardRatio,
            References = model.References.Select(ReferenceDto.From).ToList(),
            Notes = model.Notes
        };

        ResultDto? MapToResultDto(Result? result)
        {
            return result switch
            {
                Domain.Trading.Result.Loss => ResultDto.Loss,
                Domain.Trading.Result.BreakEven => ResultDto.BreakEven,
                Domain.Trading.Result.Mediocre => ResultDto.Mediocre,
                Domain.Trading.Result.Win => ResultDto.Win,
                _ => null
            };
        }
    }
}