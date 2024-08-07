using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.TestInfrastructure.TestDataBuilder;

public static partial class TestData
{
    public record Currency : ITestData<Domain.Trading.Currency, Currency>
    {
        public Guid Id { get; init; } = Guid.Parse("a2adcda4-abb4-409b-a45c-97f5275dbfbc");
        public string IsoCode { get; init; } = "EUR";
        public string Name { get; set; } = "Euro";
        public DateTime CreatedAt { get; init; } = DateTime.Parse("2024-08-03T14:00:00").ToUtcKind();
        
        public static Currency Default { get; } = new Lazy<Currency>(() => new Currency()).Value;

        public Domain.Trading.Currency Build()
        {
            return new Domain.Trading.Currency
            {
                Id = Id,
                IsoCode = IsoCode,
                Name = Name,
                CreatedAt = CreatedAt
            };
        }
    }
}