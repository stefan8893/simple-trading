namespace SimpleTrading.Domain.User.DataAccess;

public interface IUserSettingsRepository
{
    ValueTask<UserSettings> GetUserSettings();
    public ValueTask<UserSettings?> GetUserSettingsOrDefault();
}