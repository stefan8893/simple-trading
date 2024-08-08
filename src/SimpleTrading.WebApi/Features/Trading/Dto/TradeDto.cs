using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.GetTrade;
using SimpleTrading.WebApi.Features.Trading.Dto.Reference;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public class TradeDto
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public string Asset { get; set; } = null!;
    public Guid ProfileId { get; set; }
    public string Profile { get; set; } = null!;
    public decimal Size { get; set; }
    public DateTime OpenedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public decimal? Balance { get; set; }
    public ResultDto? Result { get; set; }
    public bool IsFinished { get; set; }
    public Guid CurrencyId { get; set; }
    public string Currency { get; set; } = null!;
    public decimal Entry { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
    public double? RiskRewardRatio { get; set; }
    public IReadOnlyList<ReferenceDto> References { get; set; } = [];
    public string? Notes { get; set; }

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