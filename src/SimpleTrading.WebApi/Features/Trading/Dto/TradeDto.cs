using SimpleTrading.Domain.Trading.UseCases.Shared;
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
    public DateTimeOffset Opened { get; init; }
    public DateTimeOffset? Finished { get; init; }
    public decimal? ProfitLoss { get; init; }
    public ResultDto? Result { get; init; }
    public short? Performance { get; init; }
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
    public required List<string> Warnings { get; init; }

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
            Opened = model.Opened,
            Finished = model.Finished,
            ProfitLoss = model.ProfitLoss,
            Result = MapToResultDto(model.Result),
            Performance = model.Performance,
            IsFinished = model.IsFinished,
            CurrencyId = model.CurrencyId,
            Currency = model.Currency,
            Entry = model.EntryPrice,
            StopLoss = model.StopLoss,
            TakeProfit = model.TakeProfit,
            ExitPrice = model.ExitPrice,
            RiskRewardRatio = model.RiskRewardRatio,
            References = model.References.Select(ReferenceDto.From).ToList(),
            Notes = model.Notes,
            Warnings = model.Warnings.ToList()
        };

        ResultDto? MapToResultDto(ResultModel? result)
        {
            return result switch
            {
                ResultModel.Loss => ResultDto.Loss,
                ResultModel.BreakEven => ResultDto.BreakEven,
                ResultModel.Mediocre => ResultDto.Mediocre,
                ResultModel.Win => ResultDto.Win,
                _ => null
            };
        }
    }
}