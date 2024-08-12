using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.TestInfrastructure.TestDataBuilder;

public static partial class TestData
{
    public record Asset : ITestData<Domain.Trading.Asset, Asset>
    {
        private static short _assetNumber = 1;

        public Guid Id { get; init; } = Guid.NewGuid();
        public string Symbol { get; init; } = $"ASSET{_assetNumber++:000}";
        public string Name { get; init; } = "TestAsset";
        public DateTime CreatedAt { get; init; } = DateTime.Parse("2024-08-03T14:00:00").ToUtcKind();

        public static Asset Default => new();

        public Domain.Trading.Asset Build()
        {
            return new Domain.Trading.Asset
            {
                Id = Id,
                Symbol = Symbol,
                Name = Name,
                Created = CreatedAt
            };
        }
    }
}