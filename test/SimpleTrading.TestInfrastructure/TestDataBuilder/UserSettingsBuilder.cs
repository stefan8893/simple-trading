using SimpleTrading.Domain;
using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.TestInfrastructure.TestDataBuilder;

public static partial class TestData
{
    public record UserSettings : ITestData<Domain.User.UserSettings, UserSettings>
    {
        public Guid Id { get; init; } = Constants.UserSettingsId;
        public Profile SelectedProfile { get; set; } = Profile.Default;
        public string Culture { get; set; } = Constants.DefaultCulture.Name;
        public string? Language { get; set; } = null;
        public string TimeZone { get; set; } = Constants.DefaultTimeZone;
        public DateTime UpdatedAt { get; set; } = DateTime.Parse("2024-08-03T14:00:00").ToUtcKind();

        public static UserSettings Default { get; } = new Lazy<UserSettings>(() => new UserSettings()).Value;

        public Domain.User.UserSettings Build()
        {
            var selectedProfile = SelectedProfile.Build();

            return new Domain.User.UserSettings
            {
                Id = Id,
                SelectedProfileId = selectedProfile.Id,
                SelectedProfile = selectedProfile,
                Culture = Culture,
                Language = Language,
                TimeZone = TimeZone,
                UpdatedAt = UpdatedAt
            };
        }
    }
}