namespace SimpleTrading.Domain.User.UseCases.GetUserSettings;

public record UserSettingsResponseModel(
    string Culture,
    string? Language,
    string TimeZone,
    DateTimeOffset LastModified,
    Guid ActiveProfileId,
    string ActiveProfileName
);