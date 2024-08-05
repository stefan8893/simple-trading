using SimpleTrading.Domain;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.TestInfrastructure.TestDataBuilder;

public static partial class TestData
{
    public record Trade : ITestData<Domain.Trading.Trade, Trade>
    {
        public Guid Id { get; init; } = Guid.Parse("f2cea6f6-3ce7-40f7-a901-b04a6feff5e8");
        public Asset Asset { get; init; } = Asset.Default;
        public Profile Profile { get; init; } = Profile.Default;
        public decimal Size { get; init; } = 10_000m;
        public DateTime OpenedAt { get; init; } = DateTime.Parse("2024-08-03T14:00:00").ToUtcKind();
        public DateTime? FinishedAt { get; init; } = DateTime.Parse("2024-08-03T18:00:00").ToUtcKind();
        public Outcome? Outcome { get; init; } = null;
        public Currency Currency { get; init; } = Currency.Default;
        public PositionPrices PositionPrices { get; init; } = new() {Entry = 1.0m};
        public ICollection<Reference> Reference { get; init; } = [];
        public string Notes { get; set; } = "";

        public static Trade Default => new();

        public Domain.Trading.Trade Build()
        {
            var asset = Asset.Build();
            var profile = Profile.Build();
            var currency = Currency.Build();

            var trade = new Domain.Trading.Trade
            {
                Id = Id,
                AssetId = asset.Id,
                Asset = asset,
                ProfileId = profile.Id,
                Profile = profile,
                Size = Size,
                OpenedAt = OpenedAt,
                CurrencyId = currency.Id,
                Currency = currency,
                PositionPrices = PositionPrices,
                Reference = Reference,
                Notes = Notes
            };

            if (Outcome is not null && FinishedAt.HasValue)
                trade.Finish(new Domain.Trading.Trade.FinishTradeDto(Outcome.Result,
                    Outcome.Balance,
                    FinishedAt.Value,
                    () => OpenedAt,
                    Constants.DefaultTimeZone));

            return trade;
        }
    }
}