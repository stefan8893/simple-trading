using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.TestInfrastructure.TestDataBuilder;

public static partial class TestData
{
    public record Asset : ITestData<Domain.Trading.Asset, Asset>
    {
        public Guid Id { get; init; } = Guid.Parse("767a33a3-555f-4a5f-829d-4ba7789bb920");
        public string Symbol { get; init; } = "EURUSD";
        public string Name { get; init; } = "EUR/USD";
        public DateTime CreatedAt { get; init; } = DateTime.Parse("2024-08-03T14:00:00").ToUtcKind();

        public static Asset Default { get; } = new Lazy<Asset>(() => new Asset()).Value;

        public Domain.Trading.Asset Build()
        {
            return new Domain.Trading.Asset
            {
                Id = Id,
                Symbol = Symbol,
                Name = Name,
                CreatedAt = CreatedAt
            };
        }
    }
}