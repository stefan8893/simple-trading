using OneOf;
using SimpleTrading.Domain;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.TestInfrastructure.TestDataBuilder;

public static partial class TestData
{
    public record Trade : ITestData<Domain.Trading.Trade, Trade>
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public OneOf<Guid, Asset, Domain.Trading.Asset> AssetOrId { get; init; } = Asset.Default;
        public OneOf<Guid, Profile, Domain.Trading.Profile> ProfileOrId { get; init; } = Profile.Default;
        public decimal Size { get; init; } = 10_000m;
        public DateTime Opened { get; init; } = DateTime.Parse("2024-08-03T14:00:00").ToUtcKind();
        public DateTime? Closed { get; init; } = DateTime.Parse("2024-08-03T18:00:00").ToUtcKind();
        public Outcome? Outcome { get; init; } = null;
        public OneOf<Guid, Currency, Domain.Trading.Currency> CurrencyOrId { get; init; } = Currency.Default;

        // ReSharper disable once MemberHidesStaticFromOuterClass
        public OneOf<PositionPrices, Domain.Trading.PositionPrices> PositionPrices { get; init; } =
            TestData.PositionPrices.Default;

        public string Notes { get; init; } = "";
        public DateTime CreatedAt { get; init; } = DateTime.Parse("2024-08-03T14:00:00").ToUtcKind();

        public static Trade Default => new();

        public Domain.Trading.Trade Build()
        {
            var asset = AssetOrId.Match(
                id => (Asset.Default with {Id = id}).Build(),
                asset => asset.Build(),
                assetEntity => assetEntity
            );

            var profile = ProfileOrId.Match(
                id => (Profile.Default with {Id = id}).Build(),
                profile => profile.Build(),
                profileEntity => profileEntity
            );

            var currency = CurrencyOrId.Match(
                id => (Currency.Default with {Id = id}).Build(),
                currency => currency.Build(),
                currencyEntity => currencyEntity
            );

            var positionPrices = PositionPrices.Match(
                p => p.Build(),
                p => p
            );

            var trade = new Domain.Trading.Trade
            {
                Id = Id,
                AssetId = asset.Id,
                Asset = asset,
                ProfileId = profile.Id,
                Profile = profile,
                Size = Size,
                Opened = Opened,
                CurrencyId = currency.Id,
                Currency = currency,
                PositionPrices = positionPrices,
                References = [],
                Notes = Notes,
                CreatedAt = CreatedAt
            };

            if (Outcome is not null && Closed.HasValue && positionPrices.ExitPrice.HasValue)
                trade.Close(new Domain.Trading.Trade.FinishTradeDto(Outcome.Result,
                    Outcome.Balance,
                    positionPrices.ExitPrice.Value,
                    Closed.Value,
                    () => Opened,
                    Constants.DefaultTimeZone));

            return trade;
        }
    }
}