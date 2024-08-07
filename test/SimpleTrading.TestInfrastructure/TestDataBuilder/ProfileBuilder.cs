using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.TestInfrastructure.TestDataBuilder;

public static partial class TestData
{
    public record Profile : ITestData<Domain.Trading.Profile, Profile>
    {
        public Guid Id { get; init; } = Guid.Parse("0d44b0c7-1f1e-4e0b-b08e-4449e1fb40c8");
        public string Name { get; set; } = "Default";
        public string? Description { get; set; } = "";
        public DateTime CreatedAt { get; init; } = DateTime.Parse("2024-08-03T14:00:00").ToUtcKind();

        public static Profile Default { get; } = new Lazy<Profile>(() => new Profile()).Value;

        public Domain.Trading.Profile Build()
        {
            return new Domain.Trading.Profile
            {
                Id = Id,
                Name = Name,
                Description = Description,
                CreatedAt = CreatedAt
            };
        }
    }
}