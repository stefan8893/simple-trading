using OneOf;
using SimpleTrading.Domain;
using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.TestInfrastructure.TestDataBuilder;

public static partial class TestData
{
    public record UserSettings : ITestData<Domain.User.UserSettings, UserSettings>
    {
        public Guid Id { get; init; } = Constants.UserSettingsId;
        public OneOf<Guid, Profile, Domain.Trading.Profile> SelectedProfileOrId { get; init; } = Profile.Default;
        public string Culture { get; init; } = Constants.DefaultCulture.Name;
        public string? Language { get; init; } = null;
        public string TimeZone { get; init; } = Constants.DefaultTimeZone;
        public DateTime UpdatedAt { get; init; } = DateTime.Parse("2024-08-03T14:00:00").ToUtcKind();

        public static UserSettings Default => new ();

        public Domain.User.UserSettings Build()
        {
            var selectedProfile = SelectedProfileOrId.Match(
                id => (Profile.Default with {Id = id}).Build(),
                profile => profile.Build(),
                profileEntity => profileEntity
            );

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