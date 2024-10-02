namespace SimpleTrading.Domain.User.DataAccess;

public interface IUserSettingsRepository
{
    Task<UserSettings> GetUserSettings();
    public Task<UserSettings?> GetUserSettingsOrDefault();
}