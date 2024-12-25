using SimpleTrading.Domain.Infrastructure.Extensions;

namespace SimpleTrading.Domain.Trading.UseCases.Shared;

public class TradeResponseModel
{
    public required Guid Id { get; init; }
    public required Guid AssetId { get; init; }
    public required string Asset { get; init; }
    public required Guid ProfileId { get; init; }
    public required string Profile { get; init; }
    public required decimal Size { get; init; }
    public required DateTimeOffset Opened { get; init; }
    public required DateTimeOffset? Closed { get; init; }
    public required decimal? Balance { get; init; }
    public required ResultModel? Result { get; init; }
    public required short? Performance { get; init; }
    public required bool IsClosed { get; init; }
    public required Guid CurrencyId { get; init; }
    public required string Currency { get; init; }
    public required decimal EntryPrice { get; init; }
    public required decimal? StopLoss { get; init; }
    public required decimal? TakeProfit { get; init; }
    public required decimal? ExitPrice { get; init; }
    public required double? RiskRewardRatio { get; init; }
    public required IReadOnlyList<ReferenceModel> References { get; init; }
    public required string? Notes { get; init; }

    public static TradeResponseModel From(Trade trade, string timeZone)
    {
        return new TradeResponseModel
        {
            Id = trade.Id,
            AssetId = trade.AssetId,
            Asset = trade.Asset.Symbol,
            ProfileId = trade.ProfileId,
            Profile = trade.Profile.Name,
            Size = trade.Size,
            Opened = trade.Opened.ToLocal(timeZone),
            Closed = trade.Closed?.ToLocal(timeZone),
            Balance = trade.Balance,
            Result = trade.Result?.ToResultModel(),
            Performance = trade.Result?.Performance,
            IsClosed = trade.IsClosed,
            CurrencyId = trade.CurrencyId,
            Currency = trade.Currency.IsoCode,
            EntryPrice = trade.PositionPrices.Entry,
            StopLoss = trade.PositionPrices.StopLoss,
            TakeProfit = trade.PositionPrices.TakeProfit,
            ExitPrice = trade.PositionPrices.Exit,
            RiskRewardRatio = trade.RiskRewardRatio,
            References = trade.References
                .Select(ReferenceModel.From)
                .ToList(),
            Notes = trade.Notes
        };
    }
}