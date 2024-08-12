namespace SimpleTrading.TestInfrastructure.TestDataBuilder;

public static partial class TestData
{
    public record PositionPrices : ITestData<Domain.Trading.PositionPrices, PositionPrices>
    {
        public decimal EntryPrice { get; init; } = 1.10m;
        public decimal? StopLoss { get; init; } = null;
        public decimal? TakeProfit { get; init; } = null;
        public decimal? ExitPrice { get; init; } = null;

        public static PositionPrices Default => new();

        public Domain.Trading.PositionPrices Build()
        {
            return new Domain.Trading.PositionPrices
            {
                Entry = EntryPrice,
                StopLoss = StopLoss,
                TakeProfit = TakeProfit,
                Exit = ExitPrice
            };
        }
    }
}