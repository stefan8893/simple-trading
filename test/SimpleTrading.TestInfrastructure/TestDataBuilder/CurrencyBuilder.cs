using SimpleTrading.Domain.Infrastructure.Extensions;

namespace SimpleTrading.TestInfrastructure.TestDataBuilder;

public static partial class TestData
{
    public record Currency : ITestData<Domain.Trading.Currency, Currency>
    {
        private static short _currencyNumber = 1;

        public Guid Id { get; init; } = Guid.NewGuid();
        public string IsoCode { get; init; } = $"C{_currencyNumber++:00}";
        public string Name { get; init; } = "Test Currency";
        public DateTime Created { get; init; } = DateTime.Parse("2024-08-03T14:00:00").ToUtcKind();

        public static Currency Default => new();

        public Domain.Trading.Currency Build()
        {
            return new Domain.Trading.Currency
            {
                Id = Id,
                IsoCode = IsoCode,
                Name = Name,
                Created = Created
            };
        }
    }
}