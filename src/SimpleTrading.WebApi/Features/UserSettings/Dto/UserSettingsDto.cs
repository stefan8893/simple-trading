using SimpleTrading.Domain.User.UseCases.GetUserSettings;

namespace SimpleTrading.WebApi.Features.UserSettings.Dto;

public class UserSettingsDto
{
    public required string Culture { get; set; }
    public required string? Language { get; set; }
    public required string TimeZone { get; set; }
    public required DateTimeOffset LastModified { get; set; }

    public required Guid SelectedProfileId { get; set; }

    public required string SelectedProfileName { get; set; }

    public static UserSettingsDto From(UserSettingsResponseModel userSettings)
    {
        return new UserSettingsDto
        {
            Culture = userSettings.Culture,
            Language = userSettings.Language,
            TimeZone = userSettings.TimeZone,
            LastModified = userSettings.LastModified,
            SelectedProfileId = userSettings.SelectedProfileId,
            SelectedProfileName = userSettings.SelectedProfileName
        };
    }
}