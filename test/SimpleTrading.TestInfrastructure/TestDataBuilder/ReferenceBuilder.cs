using OneOf;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.TestInfrastructure.TestDataBuilder;

public static partial class TestData
{
    public record Reference : ITestData<Domain.Trading.Reference, Reference>
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public OneOf<Guid, Trade, Domain.Trading.Trade> TradeOrId { get; init; } = Trade.Default;
        public ReferenceType Type { get; init; } = ReferenceType.TradingView;
        public Uri Link { get; init; } = new("https://example.org");
        public string? Notes { get; init; } = null;
        public DateTime CreatedAt { get; init; } = DateTime.Parse("2024-08-03T14:00:00").ToUtcKind();

        public static Reference Default => new ();

        public Domain.Trading.Reference Build()
        {
            var trade = TradeOrId.Match(
                id => (Trade.Default with {Id = id}).Build(),
                trade => trade.Build(),
                tradeEntity => tradeEntity);

            return new Domain.Trading.Reference
            {
                Id = Id,
                TradeId = trade.Id,
                Trade = trade,
                Type = Type,
                Link = Link,
                Notes = Notes,
                Created = CreatedAt
            };
        }
    }
}