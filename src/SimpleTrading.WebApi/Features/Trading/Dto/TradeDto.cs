using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.GetTrade;
using SimpleTrading.WebApi.Features.Trading.Dto.Reference;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public class TradeDto
{
    public Guid Id { get; init; }
    public Guid AssetId { get; init; }
    public required string Asset { get; init; }
    public Guid ProfileId { get; init; }
    public required string Profile { get; init; }
    public decimal Size { get; init; }
    public DateTime OpenedAt { get; init; }
    public DateTime? FinishedAt { get; init; }
    public decimal? Balance { get; init; }
    public ResultDto? Result { get; init; }
    public bool IsFinished { get; init; }
    public Guid CurrencyId { get; init; }
    public required string Currency { get; init; }
    public decimal Entry { get; init; }
    public decimal? StopLoss { get; init; }
    public decimal? TakeProfit { get; init; }
    public decimal? ExitPrice { get; init; }
    public double? RiskRewardRatio { get; init; }
    public required IReadOnlyList<ReferenceDto> References { get; init; }
    public string? Notes { get; init; }

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
            ExitPrice = model.ExitPrice,
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