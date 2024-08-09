namespace SimpleTrading.Domain.Trading.UseCases.GetTrade;

public class TradeResponseModel
{
    public required Guid Id { get; init; }
    public required Guid AssetId { get; init; }
    public required string Asset { get; init; }
    public required Guid ProfileId { get; init; }
    public required string Profile { get; init; }
    public required decimal Size { get; init; }
    public required DateTime Opened { get; init; }
    public required DateTime? Closed { get; init; }
    public required decimal? Balance { get; init; }
    public required Result? Result { get; init; }
    public required bool IsClosed { get; init; }
    public required Guid CurrencyId { get; init; }
    public required string Currency { get; init; }
    public required decimal Entry { get; init; }
    public required decimal? StopLoss { get; init; }
    public required decimal? TakeProfit { get; init; }
    public required decimal? ExitPrice { get; init; }
    public required double? RiskRewardRatio { get; init; }
    public required IReadOnlyList<ReferenceModel> References { get; init; }
    public required string? Notes { get; init; }

    public static TradeResponseModel From(Trade trade)
    {
        return new TradeResponseModel
        {
            Id = trade.Id,
            AssetId = trade.AssetId,
            Asset = trade.Asset.Symbol,
            ProfileId = trade.ProfileId,
            Profile = trade.Profile.Name,
            Size = trade.Size,
            Opened = trade.Opened,
            Closed = trade.Closed,
            Balance = trade.Outcome?.Balance,
            Result = trade.Outcome?.Result,
            IsClosed = trade.IsClosed,
            CurrencyId = trade.CurrencyId,
            Currency = trade.Currency.IsoCode,
            Entry = trade.PositionPrices.Entry,
            StopLoss = trade.PositionPrices.StopLoss,
            TakeProfit = trade.PositionPrices.TakeProfit,
            ExitPrice = trade.PositionPrices.ExitPrice,
            RiskRewardRatio = trade.RiskRewardRatio,
            References = trade.References
                .Select(ReferenceModel.From)
                .ToList(),
            Notes = trade.Notes
        };
    }

    public record ReferenceModel(Guid Id, ReferenceType Type, string Link, string? Notes = null)
    {
        public static ReferenceModel From(Reference reference)
        {
            return new ReferenceModel(reference.Id, reference.Type, reference.Link.AbsoluteUri, reference.Notes);
        }
    }
}